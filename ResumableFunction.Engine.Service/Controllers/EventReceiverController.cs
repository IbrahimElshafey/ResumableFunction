using Microsoft.AspNetCore.Mvc;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using System.Text.Json;

namespace ResumableFunction.Engine.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventReceiverController : ControllerBase
    {
        private readonly IFunctionEngine _engine;

        public EventReceiverController(IFunctionEngine engine)
        {
            _engine = engine;
        }

        [HttpPost]
        public async Task ReceiveEvent(PushedEvent pushEvent)
        {
            if(pushEvent.Data is JsonElement jsonElement)
            {
               var x = jsonElement.ToObject<ApiInOutResult>();
            }
          await _engine.WhenEventProviderPushEvent(pushEvent);
        }
    }
}