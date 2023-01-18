﻿using Microsoft.Extensions.DependencyInjection;
using ResumableFunction.Abstraction;
using ResumableFunction.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine
{
    public static class Extensions
    {
        public static void AddFunctionEngine(this IServiceCollection services)
        {
            services.AddScoped<IEventsRepository, SimpleActiveEventsRepository>();
            services.AddScoped<IFunctionRepository, SimpleFunctionRepository>();
            services.AddScoped<IEventProviderRepository, EventProviderRepository>();
            services.AddScoped<IFunctionEngine, FunctionEngine>();
        }
    }
}
