using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;

namespace ResumableFunction.Engine
{
    public class FunctionEngine
    {
        private readonly IWaitsRepository _waitsRepository;
        private readonly IFunctionRepository _functionRepository;
        private readonly IEventProviderRepository _eventProviderRepository;

        public FunctionEngine(
            IWaitsRepository waitsRepository,
            IFunctionRepository functionRepository, 
            IEventProviderRepository eventProviderRepository)
        {
            _waitsRepository = waitsRepository;
            _functionRepository = functionRepository;
            _eventProviderRepository = eventProviderRepository;
        }

        public Task ScanFunctionsFolder()
        {
            //find event providers and call RegisterEventProvider
            //find functions and call RegisterFunction
            //find event data converters and call RegisterEventDataConverter
            return Task.CompletedTask;
        }

        public Task RequestWait<FunctionData>(SingleEventWaiting eventWaiting,ResumableFunction<FunctionData> function)
        {
            //todo:rerwite match expression and replace every FunctionData.Prop with constant value

            /// Will execueted when a Function instance run and return ask for EventWaiting.<br/>
            /// * Find event provider or load it.<br/>
            /// * Start event provider if not started <br/>
            /// * Call SubscribeToEvent with current paylaod type (eventWaiting.EventData)
            /// * Save event to IActiveEventsRepository <br/>
            /// ** important ?? must we send some of SingleEventWaiting props to event provider?? this will make filtering more accurate
            /// but the provider will send this data back
            return Task.CompletedTask;
        }


        public async Task WhenProviderPushEvent(PushedEvent pushedEvent)
        {
            //pushed event  comes to the engine from event provider 
            //pushed event contains properties (ProviderName,EventDataType,EventData,DataConverterName)
            //* engine search event list with (ProviderName,EventType) and pass payload to match expression
            //* engine now know related function instances list
            //* load context data and start/resume active instance Function
            //* call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
            var matchedEvents = await _waitsRepository.GetEventWaits(pushedEvent);
            //pushedEvent.Data must be IEventData or can convert to IEventData
            matchedEvents = matchedEvents.Where(x => x.IsMatch(pushedEvent)).ToList();
            //foreach (var eventWaiting in matchedEvents)
            //{
            //    eventWaiting.FunctionDataType;
            //}
            //return Task.CompletedTask;
        }
    }
}
