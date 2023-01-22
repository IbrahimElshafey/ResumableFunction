using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IFunctionRepository
    {
        Task<bool> IsFunctionRegistred(Type functionType);
        Task<bool> RegisterFunction(Type functionType);
        Task<bool> SaveFunctionData(object data, Guid instanceId, string functionClassName);
        Task<object> GetFunctionData(Guid instanceId);
        Task<int> GetFunctionState(Guid functionId, string initiatedByFunction);
    }
}
