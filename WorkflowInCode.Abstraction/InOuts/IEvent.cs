using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.Abstraction.InOuts
{
    public interface IEventData
    {
        string EventProviderName { get; internal set; }
    }
}
