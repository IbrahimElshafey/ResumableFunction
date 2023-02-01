using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    internal interface IResumableFunction
    {
        FunctionRuntimeInfo FunctionRuntimeInfo { get; }
        Task OnFunctionEnd();
        IAsyncEnumerable<Wait> Start();
    }
}