using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction;
using WorkflowInCode.Abstraction.InOuts;
using WorkflowInCode.Engine.Data;
using WorkflowInCode.Engine.Data.InOuts;

namespace WorkflowInCode.Engine
{
    public class WorkflowEngine
    {
        private readonly IWorkflowData _workflowData;

        public WorkflowEngine(IWorkflowData workflowData)
        {
            _workflowData = workflowData;
        }
        public void RegisterWorkflow(string assemblyName)
        {
            //find classes that inherit from WorkflowInstance
            //for each class check if active definition is alerady exist or not
            //create database table or collection for InstanceData type
            //
            //Load and run
            //subscribe to the EventWaiting 
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

        /// <summary>
        /// Will execueted when a workflow instance run and return new EventWaiting result
        /// </summary>
        /// <param name="eventWaiting"></param>
        public void SunscribeToEvent(EventWaiting eventWaiting)
        {
            //find event provider or load it
            //start event provider if not started
            //call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
        }

        /// <summary>
        /// Remote Call
        /// </summary>
        /// <param name="pushedEvent"></param>
        public void EventReceived(PushEvent pushedEvent)
        {
            //event comes to the engine from event provider
            //engine contains properties (ProviderName,EventType,payload)
            //engine search workflow instances table to find if any workflow waits for this event type
            //engine now know related instances list
            //query for matched workflows instances and find matching function database query
            //search with query and find active instances
            //load context data and start/resume active instance workflow
        }
    }
}
