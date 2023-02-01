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
            //if alerady exist don't add it
            eventWait.FunctionRuntimeInfo.FunctionWaits.Add(eventWait);
            //switch (eventWait)
            //{
            //    case AllFunctionsWait allFunctionsWait:
            //        break;
            //    case AnyFunctionWait anyFunctionWait:
            //        break;
            //}
        }

        public Task<Wait> GetFunctionWait(Guid? functionWaitId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EventWait>> GetMatchedWaits(PushedEvent pushedEvent)
        {

            var matchedWaits = new List<EventWait>();
            //pass payload to match expression
            matchedWaits = matchedWaits.Where(x => x.IsMatch()).ToList();
            return matchedWaits;
        }

        public Task<ManyWaits> GetWaitGroup(Guid? parentGroupId)
        {
            throw new NotImplementedException();
        }
    }
}
