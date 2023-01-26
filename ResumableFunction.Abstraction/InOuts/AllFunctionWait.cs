namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllFunctionWait : Wait
    {
        public FunctionWait[] WaitingFunctions { get; set; }
        public FunctionWait[] CompletedFunctions { get; set; }
    }
}