using ResumableFunction.Engine.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IFunctionFolderRepository
    {
        Task<List<FunctionFolder>> GetFunctionFolders();
    }
}