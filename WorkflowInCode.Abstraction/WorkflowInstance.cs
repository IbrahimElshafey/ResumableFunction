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
        public WorkflowInstance()
        {
            RuntimeData = new WorkflowInstanceRuntimeData() { InstanceDataType = GetType() };
        }
        protected AnyEventWaiting WaitFirstEvent(
            params EventWaitingResult[] events)
        {
            return null;
        }


        protected AllEventWaiting WaitEvents(
            params EventWaitingResult[] events)
        {
            return null;
        }

        protected SingleEventWaiting WaitEvent(IEventData eventToWait, [CallerMemberName] string callerName = "")
        {
            var eventWaiting = new SingleEventWaiting(eventToWait);
            eventWaiting.InitiatedByMethod = callerName;
            eventWaiting.InitiatedByType = GetType();
            return eventWaiting;
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

        public WorkflowInstanceRuntimeData RuntimeData { get; private set; }
        public ContextData InstanceData { get; protected set; }

        public async Task SaveInstanceData()
        {
        }


        public async Task<SingleEventWaiting> Run()
        {
            //this method will run based on the activated workflow method
            //may be the main workflow "in RunWorkflow method" or any method that return "IAsyncEnumerable<WorkflowEvent>"
            var workflowRunner = GetActiveRunner();
            if (workflowRunner is null) return null;
            if (await workflowRunner.MoveNextAsync())
            {
                var incommingEvent = workflowRunner.Current;
                SetContextData(InstanceData, incommingEvent.SetPropExpression, incommingEvent.EventData);
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
