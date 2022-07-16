using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public class BasicEvent<T> : IEvent<T>
    {
        private readonly string eventName;

        public BasicEvent(string eventName)
        {
            this.eventName = eventName;
        }
        public async Task<T> ReceivingEvent(Func<T, bool> filter, string stepName)
        {
            //throw new NotImplementedException();
            return default;
        }

        public async Task Subscribe()
        {
            //throw new NotImplementedException();
        }

        public async Task Unsubscribe()
        {
            //throw new NotImplementedException();
        }
    }
}
