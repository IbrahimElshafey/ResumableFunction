using ResumableFunction.Engine.Abstraction;

namespace ResumableFunction.Engine.EfDataImplementation
{
    public class FunctionRepository : IFunctionRepository
    {
        public Task<FunctionData> GetFunctionData<FunctionData>(Guid instanceId, string functionName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsFunctionRegistred(Type functionType)
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
    }
}
