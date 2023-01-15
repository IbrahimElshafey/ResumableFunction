using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction
{
    public abstract partial class WorkflowInstance<ContextData>
    {
        public WorkflowInstance(ContextData data)
        {
            RuntimeData = new WorkflowRuntimeData() { InstanceId = Guid.NewGuid() };
            InstanceData = data;
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
            var eventWaiting = new SingleEventWaiting(eventType, eventName) { InitiatedByMethod = callerName };
            SetCommonProps(eventWaiting);
            return eventWaiting;
        }

        private void SetCommonProps(SingleEventWaiting eventWaiting)
        {
            eventWaiting.InitiatedByType = GetType();
            eventWaiting.WorkflowInstanceId = RuntimeData.InstanceId;
            eventWaiting.WorkflowInstanceDataType = InstanceData.GetType();
            eventWaiting.Id = Guid.NewGuid();
        }

        protected SingleEventWaiting WaitFirstSubWorkflow(params Func<IAsyncEnumerable<SingleEventWaiting>>[] subWorkflows)
        {
            return null;
        }
        protected SingleEventWaiting WaitSubWorkflows(params Func<IAsyncEnumerable<SingleEventWaiting>>[] subWorkflows)
        {
            return null;
        }
        protected SingleEventWaiting WaitSubWorkflow(Func<IAsyncEnumerable<EventWaitingResult>> subWorkflow)
        {
            return null;
        }

        public WorkflowRuntimeData RuntimeData { get; private set; }
        public ContextData InstanceData { get; protected set; }

        public async Task SaveInstanceData()
        {
        }


        public async Task<EventWaitingResult> Run()
        {
            //this method will run based on the activated workflow method
            //may be the main workflow "in RunWorkflow method" or any method that return "IAsyncEnumerable<WorkflowEvent>"
            var workflowRunner = GetActiveRunner();
            if (workflowRunner is null) return null;
            if (await workflowRunner.MoveNextAsync())
            {
                var incommingEvent = workflowRunner.Current;
                //SetContextData(InstanceData, incommingEvent.SetPropExpression, incommingEvent.EventData);
                //todo:update runtime data active runner status and waiting list
                await SaveInstanceData();
                return incommingEvent;
            }
            else
            {
                //if current workflow runner name is "RunWorkflow"
                await OnWorkflowEnd();
                return null;
            }
        }

        protected abstract IAsyncEnumerable<EventWaitingResult> RunWorkflow();
        protected virtual Task OnWorkflowEnd()
        {
            return Task.CompletedTask;
        }

    }


}
