using Refit;
using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Abstraction.WebApiProvider
{
    public abstract class WebApiEventProviderClient : IEventProvider
    {
        protected abstract string ApiUrl { get; }
        protected abstract string ApiProjectName { get; }

        protected IWebApiEventProviderClient _client => RestService.For<IWebApiEventProviderClient>(ApiUrl);
        public string EventProviderName => $"WebApiEventProvider-{ApiProjectName}";

        public void Dispose()
        {
            //dispose http context
        }

        public async Task Start()
        {
            var remoteProviderName = await _client.GetProviderName();
            if (remoteProviderName != EventProviderName)
                throw new Exception($"The RemoteProviderName and it's client must use the same event provider name.");
            await _client.Start();
        }

        public async Task Stop()
        {
            await _client.Stop();
        }

        public async Task<bool> SubscribeToEvent(Type eventType)
        {
            //revisit
            if (eventType == typeof(ApiInOutResult))
            {
                var instance = (ApiInOutResult)Activator.CreateInstance(eventType);
                if (instance != null)
                    return await _client.SubscribeToApiAction(instance.Url);
            }
            return true;
        }

        public async Task<bool> UnSubscribeEvent(Type eventType)
        {
            //revisit
            if (eventType == typeof(ApiInOutResult))
            {
                var instance = (ApiInOutResult)Activator.CreateInstance(eventType);
                if (instance != null)
                    return await _client.UnsubscribeApiAction(instance.Url);
            }
            return true;
        }
    }
}
