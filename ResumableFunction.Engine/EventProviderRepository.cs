using ResumableFunction.Abstraction;
using ResumableFunction.Engine.Abstraction;

namespace ResumableFunction.Engine
{
    public class EventProviderRepository : IEventProviderRepository
    {
        public Task<IEventProvider> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegsiterEventProvider(IEventProvider eventProvider)
        {
            throw new NotImplementedException();
        }
    }
}
