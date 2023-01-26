using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    public interface IResumableFunction<FunctionData>
    {
        FunctionData Data { get; }
        ResumableFunctionState FunctionState { get; }
        Task OnFunctionEnd();

        IAsyncEnumerable<Wait> Start();
    }
}