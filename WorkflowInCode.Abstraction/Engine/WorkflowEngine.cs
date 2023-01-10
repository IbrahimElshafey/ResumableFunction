using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Data;
using WorkflowInCode.Abstraction.Data.InOuts;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Engine
{
    public class WorkflowEngine
    {
        private readonly IWorkflowData _workflowData;

        public WorkflowEngine(IWorkflowData workflowData)
        {
            _workflowData=workflowData;
        }
        public void RegisterWorkflow(string assemblyName)
        {
            
        }   
        public async Task RegisterWorkflow<T>(WorkflowInstance<T> workflowInstance)
        {
            //find only one subclass that start with "RunWorkflow" and implement 
            using (_workflowData)
            {
                await _workflowData.WorkflowInstanceRepository.IsWorkflowRegistred(new CheckWorkflowArgs());
                //initate instance and run workflow
                //wait for the first event
                //create empty instance and set waiting list to the event/s expected
                //save state
            }

        }

        public void EventReceived(Event subscribedEvent)
        {
            //event comes to the engine
            //engine search workflow instances table to find if any workflow waits for this event type
            //engine load-related query for matched workflows instance and find matching function database query
            //search with query and find active instances
            //load context data and start/resume active instance workflow
            ActivateWorkflowInstance();
        }

        protected void ActivateWorkflowInstance()
        {

        }
    }
}
