using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;

namespace Test
{
    public class SimpleEventProvider : IEventProvider
    {
        public string UniqueName => "SimpleEventProvider";

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task Start()
        {
            //read json file and push events to the engine
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task Stop()
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task<bool> SubscribeToEvent(IEventData eventToSubscribe)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UnSubscribeEvent(IEventData eventToSubscribe)
        {
            return Task.FromResult(true);
        }
    }
}
