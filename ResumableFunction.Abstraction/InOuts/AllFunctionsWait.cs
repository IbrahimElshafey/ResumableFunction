namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllFunctionsWait : Wait
    {
        public FunctionWait[] WaitingFunctions { get; set; }
        public FunctionWait[] CompletedFunctions { get; set; }
    }
}