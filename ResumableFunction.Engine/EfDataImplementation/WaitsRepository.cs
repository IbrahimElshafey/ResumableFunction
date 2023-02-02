using Microsoft.EntityFrameworkCore;
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
    internal class WaitsRepository : RepositoryBase, IWaitsRepository
    {
        public WaitsRepository(EngineDataContext dbContext) : base(dbContext)
        {
        }

        public async Task AddWait(Wait eventWait)
        {
            //if alerady exist don't add it
            eventWait.FunctionRuntimeInfo.FunctionWaits.Add(eventWait);
            switch (eventWait)
            {
                case EventWait wait:
                    _context.EventWaits.Add(wait);
                    break;
                case AllEventsWait wait:
                    _context.AllEventsWaits.Add(wait);
                    break;
                case AnyEventWait wait:
                    _context.AnyEventWaits.Add(wait);
                    break;
                case FunctionWait wait:
                    _context.FunctionWaits.Add(wait);
                    break;
                case AllFunctionsWait wait:
                    _context.AllFunctionsWaits.Add(wait);
                    break;
                case AnyFunctionWait wait:
                    _context.AnyFunctionWaits.Add(wait);
                    break;
            }
        }

        public Task<Wait> GetParentFunctionWait(Guid? functionWaitId)
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
