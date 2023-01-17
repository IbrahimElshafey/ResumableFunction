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
            RuntimeData = new FunctionRuntimeData() { InstanceId = Guid.NewGuid() };
            FunctionData = data;
            _engine = engine;
        }
        protected AnyEventWaiting WaitFirstEvent(
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
                throw new Exception("When use WaitEvents at least one event must be mandatory.");

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
            eventWaiting.InitiatedByType = GetType();
            eventWaiting.FunctionInstanceId = RuntimeData.InstanceId;
            eventWaiting.FunctionDataType = FunctionData.GetType();
        }

        protected SingleEventWaiting WaitFirstFunction(params Func<IAsyncEnumerable<SingleEventWaiting>>[] subFunctions)
        {
            return null;
        }
        protected SingleEventWaiting WaitFunctions(params Func<IAsyncEnumerable<SingleEventWaiting>>[] subFunctions)
        {
            return null;
        }
        protected SingleEventWaiting WaitFunction(Func<IAsyncEnumerable<EventWaitingResult>> subFunction)
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
