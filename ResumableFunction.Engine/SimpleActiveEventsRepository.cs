﻿using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine
{
    public class SimpleActiveEventsRepository : IEventsRepository
    {
        public Task<IQueryable<SingleEventWaiting>> GetActiveEvents(string providerName, string eventType, object eventData)
        {
            throw new NotImplementedException();
        }

        public Task<List<SingleEventWaiting>> GetEvents(PushedEvent pushedEvent)
        {
            throw new NotImplementedException();
        }
    }
}
