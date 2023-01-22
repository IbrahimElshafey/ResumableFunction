using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    /// <summary>
    /// The function that paused when WaitEvent requested and resumed when event come.
    /// `FunctionData` Must be a class with parameter less constructor.
    /// </summary>
    //Can i make this class not generic? we use generic for match and set property expressions only?
    public abstract partial class ResumableFunction<FunctionData>
    {
        //will be set by the engine after load the instance
        protected AnyEventWaiting WaitAnyEvent(
            params SingleEventWaiting[] events)
        {
            Array.ForEach(events, SetCommonProps);
            var result = new AnyEventWaiting { Events = events };
            return result;
        }


        protected AllEventWaiting WaitEvents(
            params SingleEventWaiting[] events)
        {
            if (events.Count(x => x.IsOptional) == events.Count())
                throw new Exception($"When use {WaitEvents} at least one event must be mandatory.");

            var result = new AllEventWaiting { WaitingEvents = events };
            Array.ForEach(result.WaitingEvents, SetCommonProps);
            Array.ForEach(result.WaitingEvents, x => x.ParentGroupId = result.Id);
            return result;
        }

        protected SingleEventWaiting WaitEvent(Type eventType, string eventName, [CallerMemberName] string callerName = "")
        {
            var result = new SingleEventWaiting(eventType, eventName) { InitiatedByFunction = callerName };
            SetCommonProps(result);
            return result;
        }

        private void SetCommonProps(SingleEventWaiting eventWaiting)
        {
            eventWaiting.InitiatedByClass = GetType();
            eventWaiting.FunctionId = InstanceId;
            eventWaiting.FunctionDataType = this.Data?.GetType();
        }

        protected async Task<AnyFunctionWaiting> AnyFunction(params Expression<Func<IAsyncEnumerable<EventWaitingResult>>>[] subFunctions)
        {
            var result = new AnyFunctionWaiting { Functions = new FunctionWaitingResult[subFunctions.Length] };
            for (int i = 0; i < subFunctions.Length; i++)
            {
                var currentFunction = subFunctions[i];
                var currentFuncResult = await Function(currentFunction);
                result.Functions[i] = currentFuncResult;
            }
            return result;
        }

        protected async Task<AllFunctionWaiting> Functions(params Expression<Func<IAsyncEnumerable<EventWaitingResult>>>[] subFunctions)
        {
            var result = new AllFunctionWaiting { WaitingFunctions = new FunctionWaitingResult[subFunctions.Length] };
            for (int i = 0; i < subFunctions.Length; i++)
            {
                var currentFunction = subFunctions[i];
                var currentFuncResult = await Function(currentFunction);
                result.WaitingFunctions[i] = currentFuncResult;
            }
            return result;
        }

        protected async Task<FunctionWaitingResult> Function(Expression<Func<IAsyncEnumerable<EventWaitingResult>>> subFunction)
        {
            var result = new FunctionWaitingResult();
            var methodCall = subFunction.Body as MethodCallExpression;
            if (methodCall != null)
            {
                result.FunctionName = methodCall.Method.Name;
            }
            var funcEvents = subFunction.Compile()();
            if (funcEvents != null)
            {
                var asyncEnumerator = funcEvents.GetAsyncEnumerator();
                await asyncEnumerator.MoveNextAsync();
                var firstEvent = asyncEnumerator.Current;
                result.CurrentEvent = firstEvent;
            }
            return result;
        }

        public Guid InstanceId { get; set; }
        public FunctionData Data { get; protected set; }



        public async Task SaveFunctionData()
        {
            //await _engine.SaveFunctionData(Data, InstanceId);
        }


        protected abstract IAsyncEnumerable<EventWaitingResult> Start();
        public virtual Task OnFunctionEnd()
        {
            return Task.CompletedTask;
        }

    }


}
