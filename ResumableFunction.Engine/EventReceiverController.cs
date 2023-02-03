using Microsoft.AspNetCore.Mvc;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResumableFunction.Engine
{
    [ApiController]
    [Route("[controller]")]
    public class EventReceiverController : ControllerBase
    {
        private readonly FunctionEngine _engine;

        public EventReceiverController(FunctionEngine engine)
        {
            _engine = engine;
        }

        [HttpPost]
        public async Task ReceiveEvent(PushedEvent pushEvent)
        {
            await _engine.WhenProviderPushEvent(pushEvent);
        }

        [HttpGet(nameof(ScanFunctionsFolders))]
        public async Task ScanFunctionsFolders()
        {
            await _engine.ScanFunctionsFolders();
        }
    }
}