using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Engine.Abstraction;

namespace ResumableFunction.Engine.EfDataImplementation
{
    public class FunctionRepository : IFunctionRepository
    {
        public Task<bool> MoveFunctionToRecycleBin(FunctionRuntimeInfo functionRuntimeInfo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterFunction(Type functionType)
        {
            //check if alerady registred
            //if not register it
            throw new NotImplementedException();
        }

        public Task<bool> SaveFunctionState(FunctionRuntimeInfo functionRuntimeInfo)
        {
            throw new NotImplementedException();
        }
    }
}
