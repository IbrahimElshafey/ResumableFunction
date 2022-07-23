using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public class BasicEvent<EventData> : IExternalEvent<EventData>
    {
        public BasicEvent(string eventName)
        {
            this.Name = eventName;
        }

        public string Name { get; private set; }

        public EventData Data => default;

        public async Task Subscribe()
        {
            //throw new NotImplementedException();
        }

        public string UniqueIdentifier => Name;

        /// <summary>
        /// Will be set when event registred by the engine
        /// </summary>
        public int DataBaseIdentifier { get; private set; }

        public async Task Unsubscribe()
        {
            //throw new NotImplementedException();
        }
    }
}
