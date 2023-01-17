using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    public interface IFunctionRepository
    {
        Task<CheckFunctionResult> IsFunctionRegistred(CheckFunctionArgs args);
        Task<bool> SaveFunctionData<FunctionData>(FunctionData args,string FunctionName);
        Task<FunctionData> GetFunctionData<FunctionData>(Guid instanceId,string dataType,string FunctionName);
    }
}
