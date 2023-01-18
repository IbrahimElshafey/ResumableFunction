namespace ResumableFunction.Abstraction.InOuts
{
    public class AnyFunctionWaiting : EventWaitingResult
    {
        public FunctionWaitingResult[] Functions { get; set; }
        public FunctionWaitingResult MatchedFunction { get; set; }
    }
}