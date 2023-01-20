using System.Text.Json;

namespace ResumableFunction.Abstraction.InOuts
{
    /*
        {
          "provider": "EventproviderName",
          "dataType": "ResumableFunction.Abstraction.Samples.ManagerApprovalEvent",
          "data": {"ProjectId":"125", "Accepted":"true", "Rejected":"false"},
          "dataConverterName": ""
        }
     */
    public class PushedEvent : Dictionary<string, object>, IEventData
    {
        //revisit: to vlaidate

        public string EventProviderName => ((JsonElement)this[nameof(EventProviderName)]).GetString();

        public string EventIdentifier => ((JsonElement)this[nameof(EventIdentifier)]).GetString();
    }



}
