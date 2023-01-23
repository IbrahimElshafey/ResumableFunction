using Microsoft.Extensions.DependencyInjection;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.EfDataImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ResumableFunction.Engine
{
    public static class Extensions
    {
        public static void AddFunctionEngine(this IServiceCollection services)
        {
            services.AddScoped<IWaitsRepository, WaitsRepository>();
            services.AddScoped<IFunctionRepository, FunctionRepository>();
            services.AddScoped<IEventProviderRepository, EventProviderRepository>();
            services.AddScoped<FunctionEngine, FunctionEngine>();
        }

        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

        public static object ToObject(this PushedEvent element,Type toConvertTo)
        {
            var json = JsonSerializer.Serialize(element);
            return JsonSerializer.Deserialize(json, toConvertTo, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
