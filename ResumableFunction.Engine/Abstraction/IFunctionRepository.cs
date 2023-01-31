using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IFunctionRepository
    {
        Task<bool> IsFunctionRegistred(Type functionType);
        Task<bool> RegisterFunction(Type functionType);
        Task<bool> SaveFunctionState(FunctionRuntimeInfo functionRuntimeInfo);
        Task<object> GetFunctionData(Guid instanceId);
        Task<int> GetFunctionState(Guid functionId, string initiatedByFunction);
        Task<bool> MoveFunctionToRecycleBin(FunctionRuntimeInfo functionRuntimeInfo);
    }
}
