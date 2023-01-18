namespace ResumableFunction.Abstraction.InOuts
{
    public class AllFunctionWaiting : EventWaitingResult
    {
        public FunctionWaitingResult[] WaitingFunctions { get; set; }
        public FunctionWaitingResult[] CompletedFunctions { get; set; }
    }
}