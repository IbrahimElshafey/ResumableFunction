using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

namespace ResumableFunction.Abstraction.InOuts
{
    /*
        {
          "eventProviderName": "EventproviderNameqqqqqqqqqqqqqqq",
          "eventIdentifier": "eventIdentifierrrrrrrrrrrrrrrrr",
          "dataType": "ResumableFunction.Abstraction.Samples.ManagerApprovalEvent",
          "data": {"ProjectId":"125", "Accepted":"true", "Rejected":"false"},
        }
     */
    public class PushedEvent : Dictionary<string, object>, IEventData
    {
        public string EventProviderName
        {
            get
            {
                if (ContainsKey(nameof(EventProviderName)))
                    return (string)this[nameof(EventProviderName)];
                else if (ContainsKey("eventProviderName"))
                    return (string)this["eventProviderName"];
                return null;
            }
        }
        public string EventIdentifier
        {
            get
            {
                if (ContainsKey(nameof(EventIdentifier)))
                    return (string)this[nameof(EventIdentifier)];
                else if (ContainsKey("eventIdentifier"))
                    return (string)this["eventIdentifier"];
                return null;
            }
        }

    }
}
