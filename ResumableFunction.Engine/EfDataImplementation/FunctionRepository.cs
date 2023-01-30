using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;

namespace ResumableFunction.Engine.EfDataImplementation
{
    public class FunctionRepository : IFunctionRepository
    {
        public Task<FunctionData> GetFunctionData<FunctionData>(Guid instanceId, string functionName)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetFunctionData(Guid instanceId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetFunctionState(Guid functionId, string initiatedByFunction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsFunctionRegistred(Type functionType)
        {
            throw new NotImplementedException();
        }

        public Task MoveFunctionToRecycleBin(FunctionRuntimeInfo functionRuntimeInfo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterFunction(Type functionType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveFunctionData<FunctionData>(FunctionData args, Guid instanceId, string FunctionName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveFunctionData(object data, Guid instanceId, string functionClassName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveFunctionState(FunctionRuntimeInfo functionRuntimeInfo)
        {
            throw new NotImplementedException();
        }
    }
}
