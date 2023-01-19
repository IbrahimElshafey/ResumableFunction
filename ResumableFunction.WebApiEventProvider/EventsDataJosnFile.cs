using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.WebApiEventProvider
{
    public class EventsDataJosnFile : IEventsData
    {
        private Dictionary<string, ApiInOutResult>? _activeCalls;
        public Dictionary<string, ApiInOutResult> ActiveCalls
        {
            get
            {
                if (_activeCalls == null)
                    _activeCalls = new Dictionary<string, ApiInOutResult>();
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

        public async Task Start()
        {
        }

        public async Task Stop()
        {
        }
    }
}
