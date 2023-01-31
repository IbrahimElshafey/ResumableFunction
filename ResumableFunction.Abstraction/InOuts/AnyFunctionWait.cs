namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AnyFunctionWait : ManyFunctionsWait
    {
        public OneFunctionWait MatchedFunction { get; set; }
        internal void SetMatchedFunction(EventWait currentWait)
        {
            WaitingFunctions.ForEach(wait => wait.Status = WaitStatus.Skipped);
            MatchedEvent = currentWait;
            Status = WaitStatus.Completed;
        }
    }
}