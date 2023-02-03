using Refit;
using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Abstraction.WebApiProvider
{
    public abstract class WebApiEventProviderHandler : IEventProviderHandler
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
            try
            {
                var remoteProviderName = await _client.GetProviderName();
                if (remoteProviderName != EventProviderName)
                    throw new Exception($"The RemoteProviderName and it's client must use the same event provider name.");
                await _client.Start();
            }
            catch (Exception)
            {

            }
        }
           

        public async Task Stop()
        {
            try
            {
                await _client.Stop();
            }
            catch (Exception)
            {

            }
            
        }

        public async Task<bool> SubscribeToEvent(IEventData eventData)
        {
            try
            {
                return await _client.SubscribeToApiAction(eventData.EventIdentifier);
            }
            catch (Exception)
            {
                return false;
            }
            //revisit
            
        }

        public async Task<bool> UnSubscribeEvent(IEventData eventData)
        {
            try
            {
                return await _client.UnsubscribeApiAction(eventData.EventIdentifier);
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
