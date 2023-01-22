using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.EfDataImplementation
{
    public class WaitsRepository : IWaitsRepository
    {
        public Task<IQueryable<SingleEventWait>> GetWaits(string providerName, string eventType, object eventData)
        {
            throw new NotImplementedException();
        }

        public Task<List<SingleEventWait>> GetEventWaits(PushedEvent pushedEvent)
        {
            throw new NotImplementedException();
        }
    }
}
