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
    public abstract partial class ResumableFunction<ContextData>
    {
        private readonly IFunctionEngine _engine;
        public ResumableFunction(ContextData data, IFunctionEngine engine)
        {
            InstanceId = Guid.NewGuid();
            FunctionData = data;
            _engine = engine;
            CurrentEvents = new List<SingleEventWaiting>();
        }
        protected AnyEventWaiting WaitAnyEvent(
            params SingleEventWaiting[] events)
        {
            Array.ForEach(events, SetCommonProps);
            var result = new AnyEventWaiting { Events = events };
            _engine.RequestEventWait(result);
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
            _engine.RequestEventWait(result);
            return result;
        }

        protected SingleEventWaiting WaitEvent(Type eventType, string eventName, [CallerMemberName] string callerName = "")
        {
            var result = new SingleEventWaiting(eventType, eventName) { InitiatedByFunction = callerName };
            SetCommonProps(result);
            _engine.RequestEventWait(result);
            return result;
        }

        private void SetCommonProps(SingleEventWaiting eventWaiting)
        {
            eventWaiting.InitiatedByClass = GetType();
            eventWaiting.FunctionId = InstanceId;
            eventWaiting.FunctionDataType = FunctionData?.GetType();
            CurrentEvents.Add(eventWaiting);
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
        public ContextData FunctionData { get; protected set; }

        /// <summary>
        /// To be used by engine to translate match function.
        /// </summary>
        public List<SingleEventWaiting> CurrentEvents { get; }

        public async Task SaveFunctionData()
        {
        }


        public async Task<EventWaitingResult> Run()
        {
            //this method will run based on the activated Function method
            //may be the main Function "in RunFunction method" or any method that return "IAsyncEnumerable<FunctionEvent>"
            var FunctionRunner = GetActiveRunner();
            if (FunctionRunner is null) return null;
            if (await FunctionRunner.MoveNextAsync())
            {
                var incommingEvent = FunctionRunner.Current;
                //SetContextData(FunctionData, incommingEvent.SetPropExpression, incommingEvent.EventData);
                //todo:update runtime data active runner status and waiting list
                await SaveFunctionData();
                return incommingEvent;
            }
            else
            {
                //if current Function runner name is "RunFunction"
                await OnFunctionEnd();
                return null;
            }
        }

        protected abstract IAsyncEnumerable<EventWaitingResult> RunFunction();
        protected virtual Task OnFunctionEnd()
        {
            return Task.CompletedTask;
        }

    }


}
