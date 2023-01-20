using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using ResumableFunction.WebApiEventProvider;
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
        public CatchInOutsActionFilter(IEventsData eventsData)
        {
            this.eventsData = eventsData;
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            if (await IsDisabled(context.ActionDescriptor)) return;
            if (await eventsData.IsSubscribedToAction(context.HttpContext.GetEventIdentifier()))
                eventsData.ActiveCalls.Add(
                    context.HttpContext.TraceIdentifier,
                    new ApiCallEvent().AddArgs(context.ActionArguments));
        }

        public async void OnActionExecuted(ActionExecutedContext context)
        {
            //if (await IsDisabled(context.ActionDescriptor)) return;
            //if (!await eventsData.IsSubscribedToAction(context.HttpContext.GetEventIdentifier())) return;
          
            string traceIdentifier = context.HttpContext.TraceIdentifier;
            if (!eventsData.ActiveCalls.ContainsKey(traceIdentifier)) return;

            ApiCallEvent pushedEvent = eventsData.ActiveCalls[traceIdentifier];
            pushedEvent.Add($"__Result", (context.Result as ObjectResult)?.Value);
            pushedEvent.EventProviderName = Extensions.GetEventProviderName();
            pushedEvent.EventIdentifier = $"{context.HttpContext.Request.Method}#{context.HttpContext.Request.Path}";
            pushedEvent.EventIdentifier = context.HttpContext.GetEventIdentifier();

            using (var client = new HttpClient())
            {
                try
                {
                    //todo:put url in config
                    await client.PostAsync(
                    "https://localhost:7295/EventReceiver",
                    new StringContent(JsonConvert.SerializeObject(pushedEvent), Encoding.UTF8, "application/json"));
                }
                catch (Exception)
                {

                    //todo:log exception
                }

            };

            eventsData.ActiveCalls.Remove(traceIdentifier);
        }

        private async Task<bool> IsDisabled(ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ControllerActionDescriptor cad)
            {
                if (cad.MethodInfo.GetCustomAttributes(true).Any(a => a.GetType().Equals(typeof(DisableEventProviderAttribute))))
                    return true;
            }
            return !await eventsData.IsStarted();
        }

    }
}
