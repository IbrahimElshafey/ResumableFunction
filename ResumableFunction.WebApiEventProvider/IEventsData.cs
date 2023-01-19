using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.WebApiEventProvider
{
    public interface IEventsData
    {
        Dictionary<string, ApiInOutResult> ActiveCalls { get; }
        Task<bool> AddActionPath(string actionPath);
        Task<bool> DeleteActionPath(string actionPath);
        Task Start();
        Task Stop();

        Task<bool> IsStarted();
    }
}
