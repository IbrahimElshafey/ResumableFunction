namespace ResumableFunction.Abstraction
{
    public interface IResumableFunction<FunctionData> where FunctionData : class, new()
    {
        FunctionData Data { get; set; }
        Guid InstanceId { get; }

        Task OnFunctionEnd();
    }
}