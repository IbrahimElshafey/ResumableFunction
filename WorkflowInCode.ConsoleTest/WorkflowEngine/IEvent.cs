using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IEvent<EventData>
    {
        public string Name { get; }
        EventData Data { get;}
    }
}
