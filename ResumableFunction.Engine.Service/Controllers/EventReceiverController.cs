using Microsoft.AspNetCore.Mvc;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventReceiverController : ControllerBase
    {
        private readonly ILogger<PushedEvent> _logger;
        private readonly IFunctionEngine _engine;

        public EventReceiverController(ILogger<PushedEvent> logger, IFunctionEngine engine)
        {
            _logger = logger;
            _engine = engine;
        }

        [HttpPost]
        public async Task ReceiveEvent(PushedEvent pushEvent)
        {
          await _engine.WhenEventProviderPushEvent(pushEvent);
        }
    }
}