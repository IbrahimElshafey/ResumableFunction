using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.InOuts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResumableFunction.Engine
{
    public partial class FunctionEngine
    {

        public async Task<bool> WhenProviderPushEvent(PushedEvent pushedEvent)
        {
            //engine search waits list with(ProviderName, EventType)
            //matched waits contains Data from pushed event
            //and FunctionRuntimeInfo loaded from database
            var matchedWaits = await _waitsRepository.GetMatchedWaits(pushedEvent);
            foreach (var currentWait in matchedWaits)
            {
                var functionClass = new ResumableFunctionWrapper(currentWait);
                SetData(currentWait, functionClass);

                if (IsSingleEvent(currentWait) || await IsGroupLastWait(currentWait))
                {
                    //get next event wait
                    var nextWaitResult = await functionClass.GetNextWait();
                    return await HandleNextWait(nextWaitResult, currentWait, functionClass);



                    //* call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
                }
                await _functionRepository.SaveFunctionState(currentWait.FunctionRuntimeInfo);

            }
            return false;
        }

        private async Task<bool> HandleNextWait(NextWaitResult nextWaitResult, Wait currentWait, ResumableFunctionWrapper functionClass)
        {
            if (IsFinalExit(nextWaitResult))
                return await _functionRepository.MoveFunctionToRecycleBin(functionClass.FunctionRuntimeInfo);
            else if (IsSubFunctionExit(nextWaitResult))
            {
                return await SubFunctionExit(currentWait, functionClass);
            }
            else if (nextWaitResult.Result is not null)
            {
                nextWaitResult.Result.FunctionWaitId = currentWait.FunctionWaitId;
                return await WaitRequest(nextWaitResult.Result, functionClass);
            }
            currentWait.Status = WaitStatus.Completed;
            return false;
        }

        private static void SetData(EventWait currentWait, ResumableFunctionWrapper functionClass)
        {
            currentWait.SetData();
            //functionClass.Data = currentWait.FunctionRuntimeInfo.Data;
            //functionClass.FunctionRuntimeInfo.Data = functionClass.Data;
        }

        private async Task<bool> SubFunctionExit(Wait lastFunctionWait, ResumableFunctionWrapper functionClass)
        {
            var functionWait = await _waitsRepository.GetFunctionWait(lastFunctionWait.FunctionWaitId);
            var backToCaller = false;
            switch (functionWait)
            {
                //one sub function -> return to caller after function end
                case FunctionWait oneFunctionWait:
                    backToCaller = true;
                    break;
                //many sub functions -> wait function group to complete and return to caller
                case AllFunctionsWait allFunctionsWait:
                    allFunctionsWait.MoveToMatched(lastFunctionWait.FunctionWaitId);
                    if (allFunctionsWait.Status == WaitStatus.Completed)
                        backToCaller = true;
                    break;
                case AnyFunctionWait anyFunctionWait:
                    anyFunctionWait.SetMatchedFunction(lastFunctionWait.FunctionWaitId);
                    if (anyFunctionWait.Status == WaitStatus.Completed)
                        backToCaller = true;
                    break;
            }

            if (backToCaller)
            {
                var nextWaitResult = await functionClass.BackToCaller(functionWait);
                return await HandleNextWait(nextWaitResult, lastFunctionWait, functionClass);
            }

            return true;
        }

        private bool IsFinalExit(NextWaitResult nextWait)
        {
            return nextWait.Result is null && nextWait.IsFinalExit;
        }
        private bool IsSubFunctionExit(NextWaitResult nextWait)
        {
            return nextWait.Result is null && nextWait.IsSubFunctionExit;
        }

        private async Task<bool> IsGroupLastWait(EventWait currentWait)
        {
            var group = await _waitsRepository.GetWaitGroup(currentWait.ParentGroupId);
            switch (group)
            {
                case AllEventsWait allEventsWait:
                    allEventsWait.MoveToMatched(currentWait);
                    return allEventsWait.Status == WaitStatus.Completed;

                case AnyEventWait anyEventWait:
                    anyEventWait.SetMatchedEvent(currentWait); ;
                    return true;
            }
            return false;
        }

        private bool IsSingleEvent(EventWait currentWait)
        {
            return currentWait.ParentGroupId is null;
        }

        private bool Validate(Wait nextWait)
        {
            return true;
        }

        /// <summary>
        /// Will execueted when a function instance run and ask for EventWaiting.
        /// </summary>
        private async Task<bool> WaitRequest(Wait newWait, ResumableFunctionWrapper functionClass)
        {
            if (Validate(newWait) is false) return false;
            switch (newWait)
            {
                case EventWait eventWait:
                    await EventWaitRequested(eventWait, functionClass);
                    break;
                case ManyWaits manyWaits:
                    await ManyWaitsRequested(manyWaits, functionClass);
                    break;
                case FunctionWait functionWait:
                    await FunctionWaitRequested(functionWait, functionClass);
                    break;
                case ManyFunctionsWait manyFunctionsWait:
                    await ManyFunctionsWaitRequested(manyFunctionsWait, functionClass);
                    break;
            }
            return false;
        }

        private async Task ManyFunctionsWaitRequested(ManyFunctionsWait functionsWait, ResumableFunctionWrapper functionClass)
        {
            foreach (var functionWait in functionsWait.WaitingFunctions)
            {
                await FunctionWaitRequested(functionWait, functionClass);
            }
            await _waitsRepository.AddWait(functionsWait);
        }

        private async Task FunctionWaitRequested(FunctionWait functionWait, ResumableFunctionWrapper functionClass)
        {
            await WaitRequest(functionWait.CurrentWait, functionClass);
            await _waitsRepository.AddWait(functionWait);
        }

        private async Task ManyWaitsRequested(ManyWaits manyWaits, ResumableFunctionWrapper functionClass)
        {
            foreach (var eventWait in manyWaits.WaitingEvents)
            {
                await EventWaitRequested(eventWait, functionClass);
            }
            await _waitsRepository.AddWait(manyWaits);
        }

        private async Task EventWaitRequested(EventWait eventWait, ResumableFunctionWrapper functionClass)
        {
            // * Rerwite match expression and set prop expresssion
            eventWait.MatchExpression = new RewriteMatchExpression(eventWait).Result;
            eventWait.SetDataExpression = new RewriteSetDataExpression(eventWait).Result;
            // * Find event provider handler or load it.
            var eventProviderHandler = await _eventProviderRepository.GetByName(eventWait.EventProviderName);
            // * Start event provider if not started 
            await eventProviderHandler.Start();
            // * Call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
            await eventProviderHandler.SubscribeToEvent(eventWait.EventData);
            // * Save event to IActiveEventsRepository 
            await _waitsRepository.AddWait(eventWait);
            // ** important ?? must we send some of SingleEventWaiting props to event provider?? this will make filtering more accurate
            // but the provider will send this data back
        }


    }
}
