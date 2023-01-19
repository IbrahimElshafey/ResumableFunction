using ResumableFunction.Abstraction.InOuts;
using System.Reflection;

namespace ResumableFunction.Abstraction
{
    public interface IFunctionEngine
    {
        /// <summary>
        /// Find all FunctionInstances,EventProviders and EventEventDataConverter and register them all.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        Task RegisterAssembly(Assembly assembly);
        Task RegisterFunction(Type functionType);
        Task RegisterEventProvider(Type eventProviderType);
        Task RegisterEventDataConverter(Type converterType);

        /// <summary>
        ///pushed event  comes to the engine from event provider <br/>
        ///pushed event contains properties (ProviderName,EventType,EventData)<br/>
        ///* engine search event list with (ProviderName,EventType) and pass payload to match expression<br/>
        ///* engine now know related instances list<br/>
        ///* load context data and start/resume active instance Function<br/>
        ///* call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
        /// </summary>
        /// <param name="pushedEvent"></param>
        /// <returns></returns>
        Task WhenEventProviderPushEvent(PushedEvent pushedEvent);

        /// <summary>
        /// Will execueted when a Function instance run and return ask for EventWaiting result.<br/>
        /// * Find event provider or load it.<br/>
        /// * Start event provider if not started <br/>
        /// * Call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
        /// * Save event to IActiveEventsRepository <br/>
        /// ** important ?? must we send some of SingleEventWaiting props to event provider?? this will make filtering more accurate
        /// but the provider will send this data back
        /// </summary>
        /// <param name="eventWaiting"></param>
        Task RequestEventWait(SingleEventWaiting eventWaiting);
        Task RequestEventWait(AllEventWaiting eventWaiting);
        Task RequestEventWait(AnyEventWaiting eventWaiting);
    }
}