using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using ResumableFunction.WebApiEventProvider;
using ResumableFunction.WebApiEventProvider.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Abstraction.WebApiEventProvider
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-7.0#action-filters
    /// </summary>
    public class CatchInOutsActionFilter : IActionFilter
    {
        private readonly IEventsData eventsData;
        private readonly ResumableFunctionSettings config;
        public CatchInOutsActionFilter(IEventsData eventsData, IOptions<ResumableFunctionSettings> options)
        {
            this.eventsData = eventsData;
            config = options.Value;
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {

            if (await IsEnabled(context) is false) return;
            if (await eventsData.IsSubscribedToAction(context.HttpContext.GetEventIdentifier()))
                eventsData.ActiveCalls.Add(
                    context.HttpContext.TraceIdentifier,
                    new ApiCallEvent().AddArgs(context.ActionArguments, config.FlatObject));
        }

        public async void OnActionExecuted(ActionExecutedContext context)
        {
            if (IsSuccess(context) is false) return;
            string traceIdentifier = context.HttpContext.TraceIdentifier;
            if (!eventsData.ActiveCalls.ContainsKey(traceIdentifier)) return;

            ApiCallEvent pushedEvent = eventsData.ActiveCalls[traceIdentifier];
            pushedEvent.AddResult((context.Result as ObjectResult)?.Value, config.FlatObject);
            pushedEvent.EventProviderName = Extensions.GetEventProviderName();
            pushedEvent.EventIdentifier = context.HttpContext.GetEventIdentifier();

            using (var client = new HttpClient())
            {
                try
                {
                    var slash = config.EngineServiceUrl.EndsWith('/') ? string.Empty : "/";
                    await client.PostAsync(
                        $"{config.EngineServiceUrl}{slash}EventReceiver",
                        new StringContent(JsonConvert.SerializeObject(pushedEvent), Encoding.UTF8, "application/json"));
                }
                catch (Exception)
                {

                    //todo:log exception and retry
                }

            };

            eventsData.ActiveCalls.Remove(traceIdentifier);
        }

        private bool IsSuccess(ActionExecutedContext context)
        {
            var statusCodeIsSuccess = (context.Result as ObjectResult)!
                .StatusCode?.ToString().StartsWith("2");
            return statusCodeIsSuccess == true;
        }

        private async Task<bool> IsEnabled(ActionExecutingContext context)
        {
            if (await eventsData.IsStarted() is false)
                return false;

            var classEnabled = AttributeIsEnable(context.Controller.GetType()) != false;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var methodEnabled = AttributeIsEnable(controllerActionDescriptor.MethodInfo);

                var isEnabled = methodEnabled == true || (methodEnabled == null && classEnabled == true);
                return isEnabled;
            }
            return false;

            bool? AttributeIsEnable(MemberInfo memberInfo)
            {
                return memberInfo
                  .GetCustomAttributes(true)
                  .FirstOrDefault(
                        a =>
                        a.GetType().Equals(typeof(EnableEventProviderAttribute)) ||
                        a.GetType().Equals(typeof(DisableEventProviderAttribute))
                        )?.Equals(new EnableEventProviderAttribute());
            }
        }

    }
}
