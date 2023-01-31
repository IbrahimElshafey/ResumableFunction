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
        public async Task AddWait(Wait eventWait)
        {
            eventWait.FunctionRuntimeInfo.FunctionWaits.Add(eventWait);
            switch (eventWait)
            {
                case AllFunctionsWait allFunctionsWait:
                    break;
                case AnyFunctionWait anyFunctionWait:
                    break;
            }
        }

        public Task<List<EventWait>> GetMatchedWaits(PushedEvent pushedEvent)
        {
            throw new NotImplementedException();
        }
    }
}
