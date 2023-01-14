using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Engine
{
    public class WorkflowEngine
    {

        public void RegisterWorkflow(string assemblyName)
        {
            /*
             *  ## Load Event Providers
             */
            /*
             * ## LoadWorkflows
            * find classes that inherit from WorkflowInstance
            * for each class check if active definition is alerady exist or not
            * create database table or collection for InstanceData type
            * 
            * Load Instance data and run workflow
            * subscribe to the first EventWaiting 
            */
        }
        

        /// <summary>
        /// Will execueted when a workflow instance run and return new EventWaiting result
        /// </summary>
        /// <param name="eventWaiting"></param>
        public Task Wait(SingleEventWaiting eventWaiting)//todo: add methods for all and any
        {
            //find event provider or load it
            //start event provider if not started
            //call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
            void test(EventProvider eventProvider)
            {
                eventProvider.SubscribeToEvent(eventWaiting.EventData);
                //save eventWaiting to active event list

                //todo:important ?? must we send some of SingleEventWaiting props to event provider?? this will make filtering more accurate
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Remote Call
        /// </summary>
        /// <param name="pushedEvent"></param>
        public Task EventReceived(PushedEvent pushedEvent)
        {
            //pushed event comes to the engine from event provider
            //pushed event contains properties (ProviderName,EventType,Payload)
            //engine search active event list with (ProviderName,EventType) and pass payload to match expression
            //engine now know related instances list
            //load context data and start/resume active instance workflow
            //call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
            return Task.CompletedTask;
        }
    }
}
