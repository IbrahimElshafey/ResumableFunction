using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction
{
    public interface IWorkflowRepository
    {
        Task<CheckWorkflowResult> IsWorkflowRegistred(CheckWorkflowArgs args);
        Task<bool> SaveWorkflowData<InstanceData>(InstanceData args,string workflowName);
        Task<InstanceData> GetWorkflowData<InstanceData>(Guid instanceId,string dataType,string workflowName);
    }
}
