using Microsoft.AspNetCore.Mvc;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResumableFunction.Engine.Service.Controllers
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
            //var y = pushEvent.ToObject<TestResult>();
            await _engine.WhenProviderPushEvent(pushEvent);
        }
        //public class TestResult : IEventData
        //{
        //    [JsonPropertyName("CallResult")]
        //    public Result CallResult { get; set; }
        //    public Input Input { get; set; }
        //    public string InUrl { get; set; }

        //    public string EventProviderName => "WebApiEventProvider-Example.Api";

        //    public string EventIdentifier => "POST#/WeatherForecast";
        //}
        //public record Input(string ProjectId, string Accepted, string Rejected);
        //public record Result(string Message, int Number);
    }
}