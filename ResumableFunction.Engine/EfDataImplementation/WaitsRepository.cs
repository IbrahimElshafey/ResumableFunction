using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.Helpers;
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

        public Task AddWait(Wait waitToAdd)
        {
            _context.Waits.Add(waitToAdd);
            //if alerady exist don't add it
            //switch (waitToAdd)
            //{
            //    case EventWait wait:
            //        _context.EventWaits.Add(wait);
            //        break;
            //    case AllEventsWait wait:
            //        _context.AllEventsWaits.Add(wait);
            //        break;
            //    case AnyEventWait wait:
            //        _context.AnyEventWaits.Add(wait);
            //        break;
            //    case FunctionWait wait:
            //        _context.FunctionWaits.Add(wait);
            //        break;
            //    case AllFunctionsWait wait:
            //        _context.AllFunctionsWaits.Add(wait);
            //        break;
            //    case AnyFunctionWait wait:
            //        _context.AnyFunctionWaits.Add(wait);
            //        break;
            //}
            waitToAdd.FunctionRuntimeInfo.FunctionWaits.Add(waitToAdd);
            return Task.CompletedTask;
        }

        public async Task<Wait> GetParentFunctionWait(int? functionWaitId)
        {
            var result = await _context.FunctionWaits.FindAsync(functionWaitId);
            if (result == null)
            {
                var manyFunc = await _context.ManyFunctionsWaits
                        .Include(x => x.WaitingFunctions)
                        .FirstOrDefaultAsync(x => x.Id == functionWaitId);
                return manyFunc!;
            }
            return result;
        }

        public async Task<List<EventWait>> GetMatchedWaits(PushedEvent pushedEvent)
        {
            var matchedWaits =
                await _context.EventWaits
                .Include(x => x.FunctionRuntimeInfo)
                .Where(x =>
                    x.EventProviderName == pushedEvent.EventProviderName &&
                    x.EventIdentifier == pushedEvent.EventIdentifier)
                .ToListAsync();
            foreach (var wait in matchedWaits)
            {
                wait.EventData = pushedEvent.ToObject(wait.EventDataType);
            }
            matchedWaits = matchedWaits.Where(x => x.IsMatch()).ToList();
            return matchedWaits;
        }

        public async Task<ManyEventsWait> GetWaitGroup(int? parentGroupId)
        {
            var result = await _context.ManyEventsWaits
                        .Include(x => x.WaitingEvents)
                        .FirstOrDefaultAsync(x => x.Id == parentGroupId);
            return result!;
        }

        public async Task DuplicateWaitIfFirst(EventWait currentWait)
        {
            if (currentWait.IsFirst)
                DuplicateEventWait(currentWait);
            else if (currentWait.ParentGroupId is not null)
            {
                var waitGroup = await GetWaitGroup(currentWait.ParentGroupId);
                if (waitGroup.IsFirst)
                    DuplicateWaitGroup(waitGroup);
            }
            else if (currentWait.ParentFunctionWaitId is not null)
            {
                var functionWait = await GetParentFunctionWait(currentWait.ParentFunctionWaitId);
                if (functionWait.IsFirst)
                    DuplicateFunctionWait(functionWait);
            }

            void DuplicateEventWait(EventWait eventWait)
            {
                var result = new EventWait
                {
                    Status = WaitStatus.Waiting,
                    MatchExpression = eventWait.MatchExpression,
                    SetDataExpression = eventWait.SetDataExpression,
                    InitiatedByFunctionName = eventWait.InitiatedByFunctionName,
                    EventDataType = eventWait.EventDataType,
                    EventIdentifier = eventWait.EventIdentifier,
                    EventProviderName = eventWait.EventProviderName,
                    FunctionRuntimeInfo = new FunctionRuntimeInfo
                    {
                        //FunctionId = Guid.NewGuid(),
                        InitiatedByClassType = eventWait.FunctionRuntimeInfo.InitiatedByClassType
                    },
                    IsFirst = true,
                    IsNode = eventWait.IsNode,
                    IsOptional = eventWait.IsOptional,
                    ReplayType = eventWait.ReplayType,
                    NeedFunctionDataForMatch = eventWait.NeedFunctionDataForMatch,
                    StateAfterWait = eventWait.StateAfterWait,
                };
                _context.EventWaits.Add(result);
            }
            void DuplicateWaitGroup(ManyEventsWait waitGroup)
            {

            }
            void DuplicateFunctionWait(Wait functionWait)
            {
            }
        }

    }
}
