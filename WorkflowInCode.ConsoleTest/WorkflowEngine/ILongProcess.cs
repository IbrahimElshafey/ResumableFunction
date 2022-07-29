using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface ILongProcess
    {
        IEvent<EventData> WaitEvent<EventData>();
        ICommand ProcessCommand { get; }
        IEvent<EventData> RaiseEvent<EventData>();
        IEvent<EventData> CancelEvent<EventData>();
        ICommand CancelCommand { get; }
    }
}
