using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ResumableFunction.Abstraction.WebApiEventProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.WebApiEventProvider
{
    public static class Extensions
    {
        public static void AddEventProvider(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(typeof(EventProviderController).Assembly).AddControllersAsServices();
            mvcBuilder.AddMvcOptions(x => x.Filters.Add(typeof(CatchInOutsActionFilter)));
            mvcBuilder.Services.AddSingleton<IEventsData, EventsDataJsonFile>();
        }

        public static string GetEventProviderName() => $"WebApiEventProvider-{Assembly.GetEntryAssembly().GetName().Name}";
        public static string GetEventIdentifier(this HttpContext context)
            => $"{context.Request.Method}#{context.Request.Path}";
    }
}
