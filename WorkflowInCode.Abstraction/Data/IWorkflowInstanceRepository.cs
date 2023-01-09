using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Data.InOuts;

namespace WorkflowInCode.Abstraction.Data
{
    public interface IWorkflowInstanceRepository
    {
         Task<CheckWorkflowRegistartionResult> IsWorkflowRegistred(CheckWorkflowRegistartionArgs args);
    }
}
