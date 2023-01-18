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
        private readonly IEventsRepository _activeEvents;
        private readonly IFunctionRepository _functionRepository;
        private readonly IEventProviderRepository _eventProviderRepository;

        public FunctionEngine(
            IEventsRepository activeEventsRepository,
            IFunctionRepository simpleFunctionRepository, 
            IEventProviderRepository eventProviderRepository)
        {
            _activeEvents = activeEventsRepository;
            _functionRepository = simpleFunctionRepository;
            _eventProviderRepository = eventProviderRepository;
        }

        public Task RequestEventWait(SingleEventWaiting eventWaiting)
        {
            throw new NotImplementedException();
        }

        public Task RequestEventWait(AllEventWaiting eventWaiting)
        {
            throw new NotImplementedException();
        }

        public Task RequestEventWait(AnyEventWaiting eventWaiting)
        {
            throw new NotImplementedException();
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

        public Task WhenEventProviderPushEvent(PushedEvent pushedEvent)
        {
            throw new NotImplementedException();
        }
    }
}
