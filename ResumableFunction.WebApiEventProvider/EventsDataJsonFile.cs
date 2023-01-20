using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.WebApiEventProvider
{
    public class EventsDataJsonFile : IEventsData
    {
        private Dictionary<string, ApiCallEvent>? _activeCalls;
        public Dictionary<string, ApiCallEvent> ActiveCalls
        {
            get
            {
                if (_activeCalls == null)
                    _activeCalls = new Dictionary<string, ApiCallEvent>();
                return _activeCalls;
            }
        }

        public async Task<bool> AddActionPath(string actionPath)
        {
            return true;
        }

        public async Task<bool> DeleteActionPath(string actionPath)
        {
            return true;
        }

        public async Task<bool> IsStarted()
        {
            return true;
        }

        public async Task SetStarted()
        {
        }

        public async Task SetStopped()
        {
        }
    }
}
