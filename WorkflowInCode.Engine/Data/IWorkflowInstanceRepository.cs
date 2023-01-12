using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Engine.Data.InOuts;

namespace WorkflowInCode.Engine.Data
{
    public interface IWorkflowInstanceRepository
    {
         Task<CheckWorkflowResult> IsWorkflowRegistred(CheckWorkflowArgs args);
    }
}
