using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction
{
    public interface IWorkflowEngine
    {
        /// <summary>
        /// Find all WorkflowInstances,EventProviders and EventEventDataConverter and register them all.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        Task RegisterAssembly(string assemblyName);
        Task RegisterWorkflow<InstanceData>(WorkflowInstance<InstanceData> workflowInstance);
        Task RegisterEventProvider(IEventProvider eventProvider);
        Task RegisterEventEventDataConverter(IEventDataConverter eventDataConverter);

        /// <summary>
        ///pushed event  comes to the engine from event provider <br/>
        ///pushed event contains properties (ProviderName,EventType,EventData)<br/>
        ///engine search active event list with (ProviderName,EventType) and pass payload to match expression<br/>
        ///engine now know related instances list<br/>
        ///load context data and start/resume active instance workflow<br/>
        ///call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
        /// </summary>
        /// <param name="pushedEvent"></param>
        /// <returns></returns>
        Task WhenEventProviderPushEvent(PushedEvent pushedEvent);

        /// <summary>
        /// Will execueted when a workflow instance run and return new EventWaiting result.<br/>
        /// * Find event provider or load it.<br/>
        /// * Start event provider if not started <br/>
        /// * Call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
        /// * Save event to IActiveEventsRepository
        /// ** todo:important ?? must we send some of SingleEventWaiting props to event provider?? this will make filtering more accurate
        /// </summary>
        /// <param name="eventWaiting"></param>
        Task WorkflowRequestEvent(SingleEventWaiting eventWaiting);
        Task WorkflowRequestEvent(AllEventWaiting eventWaiting);
        Task WorkflowRequestEvent(AnyEventWaiting eventWaiting);
    }
}