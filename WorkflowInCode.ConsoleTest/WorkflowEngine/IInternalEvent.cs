using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IEvent<EventData>
    {
        EventData Data { get; set; }
    }

    /// <summary>
    /// Event that created internaly in the engine
    /// </summary>
    /// <typeparam name="EventData"></typeparam>
    public interface IInternalEvent<EventData>:IEvent<EventData>
    {
        
    }
}
