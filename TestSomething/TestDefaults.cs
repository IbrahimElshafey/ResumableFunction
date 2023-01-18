using ResumableFunction.Abstraction;
using ResumableFunction.Engine;
using ResumableFunction.Engine.Abstraction;

namespace Test
{
    public class TestDefaults
    {
        public IEventsRepository ActiveEventsRepository => new SimpleActiveEventsRepository();
        public IFunctionRepository SimpleFunctionRepository => new SimpleFunctionRepository();
        public IEventProviderRepository EventProviderRepository => new EventProviderRepository();
        public IFunctionEngine FunctionEngine =>new FunctionEngine(
            ActiveEventsRepository,
            SimpleFunctionRepository,
            EventProviderRepository);
    }
}
