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

    public abstract partial class ResumableFunction<FunctionData> where FunctionData : class, new()
    {
        //will be set by the engine after load the instance
        protected AnyEventWait WaitAnyEvent(
            params SingleEventWait[] events)
        {
            Array.ForEach(events, SetCommonProps);
            var result = new AnyEventWait { Events = events };
            return result;
        }


        protected SingleEventWait<T> WaitEvent<T>(string eventIdentifier, [CallerMemberName] string callerName = "") where T : class, IEventData, new()
        {
            var result = new SingleEventWait<T>(eventIdentifier) { InitiatedByFunction = callerName };
            SetCommonProps(result);
            return result;
        }

        protected AllEventWaits WaitEvents(params SingleEventWait[] events)
        {
            var result = new AllEventWaits { WaitingEvents = events };
            foreach (var item in result.WaitingEvents)
            {
                SetCommonProps(item);
                item.ParentGroupId = result.Id;
            }
            return result;
        }

        private void SetCommonProps(SingleEventWait eventWaiting)
        {
            eventWaiting.InitiatedByClass = GetType();
            eventWaiting.FunctionId = InstanceId;
            eventWaiting.FunctionDataType = Data?.GetType();
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


        public FunctionData Data { get; set; }
        public Guid InstanceId { get; private set; }

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
