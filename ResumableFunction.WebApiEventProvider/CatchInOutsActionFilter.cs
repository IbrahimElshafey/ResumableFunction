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
            eventsData.ActiveCalls.Add(context.HttpContext.TraceIdentifier, 
                new ApiInOutResult { Args = context.ActionArguments });
        }

        public async void OnActionExecuted(ActionExecutedContext context)
        {
            if (await IsDisabled(context.ActionDescriptor)) return;
            //todo:push event to function engine
            if (eventsData.ActiveCalls.ContainsKey(context.HttpContext.TraceIdentifier))
            {
                eventsData.ActiveCalls[context.HttpContext.TraceIdentifier].Result = (context.Result as ObjectResult)?.Value;
                var pushedEvent = new PushedEvent
                {
                    EventProvider = $"WebApiEventProvider-{Assembly.GetEntryAssembly().GetName().Name}",
                    Data = eventsData.ActiveCalls[context.HttpContext.TraceIdentifier],
                };
                var client = new HttpClient();
                await client.PostAsync(
                    "https://localhost:7295/EventReceiver",
                    new StringContent(JsonConvert.SerializeObject(pushedEvent), Encoding.UTF8, "application/json"));
                eventsData.ActiveCalls.Remove(context.HttpContext.TraceIdentifier);
            }
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
