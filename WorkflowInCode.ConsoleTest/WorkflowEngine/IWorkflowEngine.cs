using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public interface IWorkflowEngine
    {
        Task RegisterEvent<EventData>(IExternalEvent<EventData> externalEvent,);
    }
}
