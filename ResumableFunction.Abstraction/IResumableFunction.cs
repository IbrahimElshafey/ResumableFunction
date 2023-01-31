using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    internal interface IResumableFunction<FunctionData>
    {
        //FunctionData Data { get; }
        FunctionRuntimeInfo FunctionRuntimeInfo { get; }
        Task OnFunctionEnd();

        IAsyncEnumerable<Wait> Start();
    }
}