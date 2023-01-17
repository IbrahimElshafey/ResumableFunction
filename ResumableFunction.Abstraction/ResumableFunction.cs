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
        public ResumableFunction(ContextData data)
        {
            RuntimeData = new FunctionRuntimeData() { InstanceId = Guid.NewGuid() };
            FunctionData = data;
        }
        protected AnyEventWaiting WaitFirstEvent(
            params SingleEventWaiting[] events)
        {
            Array.ForEach(events, SetCommonProps);
            return new AnyEventWaiting { Events = events };
        }


        protected AllEventWaiting WaitEvents(
            params SingleEventWaiting[] events)
        {
            if (events.Count(x => x.IsOptional) == events.Count())
                throw new Exception("When use WaitEvents at least one event must be mandatory.");
            Array.ForEach(events, SetCommonProps);
            return new AllEventWaiting { WaitingEvents = events };
        }

        protected SingleEventWaiting WaitEvent(Type eventType, string eventName, [CallerMemberName] string callerName = "")
        {
            var eventWaiting = new SingleEventWaiting(eventType, eventName) { InitiatedByFunction = callerName };
            SetCommonProps(eventWaiting);
            return eventWaiting;
        }

        private void SetCommonProps(SingleEventWaiting eventWaiting)
        {
            eventWaiting.InitiatedByType = GetType();
            eventWaiting.FunctionInstanceId = RuntimeData.InstanceId;
            eventWaiting.FunctionDataType = FunctionData.GetType();
            eventWaiting.Id = Guid.NewGuid();
        }

        protected SingleEventWaiting WaitFirstSubFunction(params Func<IAsyncEnumerable<SingleEventWaiting>>[] subFunctions)
        {
            return null;
        }
        protected SingleEventWaiting WaitSubFunctions(params Func<IAsyncEnumerable<SingleEventWaiting>>[] subFunctions)
        {
            return null;
        }
        protected SingleEventWaiting WaitSubFunction(Func<IAsyncEnumerable<EventWaitingResult>> subFunction)
        {
            return null;
        }

        public FunctionRuntimeData RuntimeData { get; private set; }
        public ContextData FunctionData { get; protected set; }

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
