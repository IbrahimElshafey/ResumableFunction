using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.Abstraction.Data
{
    public interface IWorkflowData:IDisposable
    {
        IWorkflowInstanceRepository WorkflowInstanceRepository { get; }
        Task<bool> SaveChanges();

    }
}
