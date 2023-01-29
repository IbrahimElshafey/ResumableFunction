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

    public abstract partial class ResumableFunction<FunctionData> : IResumableFunction<FunctionData> where FunctionData : class, new()
    {
        public ResumableFunction()
        {
            Data = new FunctionData();
            FunctionState = new ResumableFunctionState
            {
                DataType = typeof(FunctionData),
                FunctionId = Guid.NewGuid(),
                InitiatedByClass = GetType(),
                Data = Data
            };
        }

        //will be set by the engine after load the instance
        protected AnyEventWait WaitAnyEvent(
            string name, params EventWait[] events)
        {
            Array.ForEach(events, SetCommonProps);
            var result = new AnyEventWait { WaitingEvents = events, Name = name };
            return result;
        }


        protected EventWait<T> WaitEvent<T>(string eventIdentifier, [CallerMemberName] string callerName = "") where T : class, IEventData, new()
        {
            var result = new EventWait<T>(eventIdentifier) { InitiatedByFunction = callerName };
            SetCommonProps(result);
            return result;
        }

        protected AllEventsWait WaitEvents(string name, params EventWait[] events)
        {
            var result = new AllEventsWait { WaitingEvents = events, Name = name };
            foreach (var item in result.WaitingEvents)
            {
                SetCommonProps(item);
                item.ParentGroupId = result.Id;
            }
            return result;
        }

        protected ReplayWait Replay(string name)
        {
            return new ReplayWait(name);
        }

        private void SetCommonProps(EventWait eventWaiting)
        {
            eventWaiting.ParentFunctionState = FunctionState;
        }

        protected async Task<AnyFunctionWait> AnyFunction(params Expression<Func<IAsyncEnumerable<Wait>>>[] subFunctions)
        {
            var result = new AnyFunctionWait { WaitingFunctions = new FunctionWait[subFunctions.Length] };
            for (int i = 0; i < subFunctions.Length; i++)
            {
                var currentFunction = subFunctions[i];
                var currentFuncResult = await Function(currentFunction);
                result.WaitingFunctions[i] = currentFuncResult;
            }
            return result;
        }

        protected async Task<AllFunctionsWait> Functions(params Expression<Func<IAsyncEnumerable<Wait>>>[] subFunctions)
        {
            var result = new AllFunctionsWait { WaitingFunctions = new FunctionWait[subFunctions.Length] };
            for (int i = 0; i < subFunctions.Length; i++)
            {
                var currentFunction = subFunctions[i];
                var currentFuncResult = await Function(currentFunction);
                result.WaitingFunctions[i] = currentFuncResult;
            }
            return result;
        }

        protected async Task<FunctionWait> Function(Expression<Func<IAsyncEnumerable<Wait>>> subFunction)
        {
            var result = new FunctionWait();
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

        public ResumableFunctionState FunctionState { get; set; }



        public abstract IAsyncEnumerable<Wait> Start();
        public virtual Task OnFunctionEnd()
        {
            return Task.CompletedTask;
        }

    }


}
