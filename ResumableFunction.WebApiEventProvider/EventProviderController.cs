using Microsoft.AspNetCore.Mvc;
using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using ResumableFunction.WebApiEventProvider;
using System.Reflection;

namespace ResumableFunction.Abstraction.WebApiEventProvider
{
    [ApiController]
    [Route("api/EventProvider")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EventProviderController : ControllerBase
    {
        private readonly IEventsData eventsData;

        public EventProviderController(IEventsData eventsData)
        {
            this.eventsData = eventsData;
        }

        [DisableEventProvider]
        [HttpGet(nameof(GetProviderName))]
        public string GetProviderName()
        {
            return $"WebApiEventProvider-{Assembly.GetEntryAssembly().GetName().Name}";
        }

        [DisableEventProvider]
        [HttpGet(nameof(Start))]
        public async Task Start()
        {
            await eventsData.Start();
        }

        [DisableEventProvider]
        [HttpGet(nameof(Stop))]
        public async Task Stop()
        {
            await eventsData.Stop();
        }

        [DisableEventProvider]
        [HttpGet(nameof(SubscribeToApiAction))]
        public async Task<bool> SubscribeToApiAction(string actionPath)
        {
            return await eventsData.AddActionPath(actionPath);
        }

        [DisableEventProvider]
        [HttpGet(nameof(UnsubscribeApiAction))]
        public async Task<bool> UnsubscribeApiAction(string actionPath)
        {
            return await eventsData.DeleteActionPath(actionPath);
        }
    }
}
