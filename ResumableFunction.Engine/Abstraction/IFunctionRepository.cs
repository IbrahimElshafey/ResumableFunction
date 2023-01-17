using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IFunctionRepository
    {
        Task<bool> IsFunctionRegistred(Type functionType);
        Task<bool> SaveFunctionData<FunctionData>(FunctionData args, Guid instanceId, string FunctionName);
        Task<FunctionData> GetFunctionData<FunctionData>(Guid instanceId,string functionName);
    }
}
