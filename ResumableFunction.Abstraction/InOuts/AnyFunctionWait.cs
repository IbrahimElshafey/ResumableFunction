namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AnyFunctionWait : ManyFunctionsWait
    {
        public FunctionWait MatchedFunction { get; set; }
        internal void SetMatchedFunction(Guid? functionId)
        {
            WaitingFunctions.ForEach(wait => wait.Status = WaitStatus.Skipped);
            MatchedFunction = WaitingFunctions.First(x => x.Id == functionId);
            MatchedFunction.Status = WaitStatus.Completed;
            Status = WaitStatus.Completed;
        }
    }
}