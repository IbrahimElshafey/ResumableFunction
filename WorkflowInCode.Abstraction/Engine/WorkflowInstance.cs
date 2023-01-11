using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Engine
{
    public abstract partial class WorkflowInstance<ContextData>
    {
        protected WaitAnyEvent WaitFirstEvent<IEvent>(
            params EventWaiting<IEvent>[] events) => null;


        protected WaitAllEvent WaitEvents<IEvent>(
            params EventWaiting<IEvent>[] events)
        {
            return null;
        }

        protected WorkflowEvent WaitEvent<IEvent>(
            IEvent eventToWait,
            Expression<Func<IEvent, bool>> matchFunction,
            Expression<Func<IEvent>> contextProp)
        {
            var result = new WorkflowEvent();
            result.MatchFunction = matchFunction;
            result.ContextProp = contextProp;
            result.EventData = eventToWait;
            return result;
        }


        protected WorkflowEvent WaitFirstSubWorkflow(params Func<IAsyncEnumerable<WorkflowEvent>>[] subWorkflows)
        {
            return null;
        }
        protected WorkflowEvent WaitSubWorkflows(params Func<IAsyncEnumerable<WorkflowEvent>>[] subWorkflows)
        {
            return null;
        }
        protected WorkflowEvent WaitSubWorkflow(Func<IAsyncEnumerable<WorkflowEvent>> subWorkflow)
        {
            return null;
        }

        public WorkflowInstanceRuntimeData RuntimeData { get; private set; }
        public ContextData InstanceData { get; protected set; }
        
        public async Task SaveInstanceData()
        {
        }

        
        public async Task<WorkflowEvent> Run()
        {
            //this method will run based on the activated workflow method
            //may be the main workflow "in RunWorkflow method" or any method that return "IAsyncEnumerable<WorkflowEvent>"
            var workflowRunner = GetActiveRunner();
            if (workflowRunner is null) return null;
            if (await workflowRunner.MoveNextAsync())
            {
                var incommingEvent = workflowRunner.Current;
                SetContextData(InstanceData, incommingEvent.ContextProp, incommingEvent.EventData);
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
        
        protected abstract IAsyncEnumerable<WorkflowEvent> RunWorkflow();
        protected virtual async Task OnWorkflowEnd()
        {

        }

    }

   
}
