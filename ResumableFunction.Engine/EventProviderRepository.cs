using ResumableFunction.Abstraction;
using ResumableFunction.Engine.Abstraction;

namespace ResumableFunction.Engine
{
    public class EventProviderRepository : IEventProviderRepository
    {
        public Task<IEventProviderHandler> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegsiterEventProvider(IEventProviderHandler eventProvider)
        {
            throw new NotImplementedException();
        }
    }
}
