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
        Task<bool> RegisterFunction(ResumableFunctionInstance function, FunctionFolder folder);
        Task<bool> MoveFunctionToRecycleBin(FunctionRuntimeInfo functionRuntimeInfo);
    }
}
