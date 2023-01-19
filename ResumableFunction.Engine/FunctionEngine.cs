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
    public class FunctionEngine : IFunctionEngine
    {
        private readonly IEventsRepository _eventsList;
        private readonly IFunctionRepository _functionRepository;
        private readonly IEventProviderRepository _eventProviderRepository;

        public FunctionEngine(
            IEventsRepository eventsRepository,
            IFunctionRepository functionRepository, 
            IEventProviderRepository eventProviderRepository)
        {
            _eventsList = eventsRepository;
            _functionRepository = functionRepository;
            _eventProviderRepository = eventProviderRepository;
        }

        public Task RequestEventWait(SingleEventWaiting eventWaiting)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task RequestEventWait(AllEventWaiting eventWaiting)
        {
            return Task.CompletedTask;
        }

        public Task RequestEventWait(AnyEventWaiting eventWaiting)
        {
            return Task.CompletedTask;
        }

        public Task RegisterAssembly(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public Task RegisterFunction(Type functionType)
        {
            throw new NotImplementedException();
        }

        public Task RegisterEventProvider(Type eventProviderType)
        {
            throw new NotImplementedException();
        }

        public Task RegisterEventDataConverter(Type converterType)
        {
            throw new NotImplementedException();
        }

        public async Task WhenEventProviderPushEvent(PushedEvent pushedEvent)
        {
            //pushed event  comes to the engine from event provider 
            //pushed event contains properties (ProviderName,EventDataType,EventData,DataConverterName)
            //* engine search event list with (ProviderName,EventType) and pass payload to match expression
            //* engine now know related function instances list
            //* load context data and start/resume active instance Function
            //* call EventProvider.UnSubscribeEvent(pushedEvent.EventData) if no other intances waits this type for the same provider
            var matchedEvents = await _eventsList.GetEventWaits(pushedEvent);
            //pushedEvent.Data must be IEventData or can convert to IEventData
            matchedEvents = matchedEvents.Where(x => x.IsMatch((IEventData)pushedEvent.Data)).ToList();
            //foreach (var eventWaiting in matchedEvents)
            //{
            //    eventWaiting.FunctionDataType;
            //}
            //return Task.CompletedTask;
        }
    }
}
