namespace ResumableFunction.Abstraction.InOuts
{
    public class ManyFunctionsWait : Wait
    {
        public List<FunctionWait> WaitingFunctions { get; internal set; }
        public List<FunctionWait> CompletedFunctions
            => WaitingFunctions?.Where(x => x.Status == WaitStatus.Completed).ToList();
        public FunctionWait MatchedFunction => WaitingFunctions?.Single(x => x.Status == WaitStatus.Completed);
        internal void SetMatchedFunction(Guid? functionId)
        {
            WaitingFunctions.ForEach(wait => wait.Status = WaitStatus.Skipped);
            var functionWait = WaitingFunctions.First(x => x.Id == functionId);
            functionWait.Status = WaitStatus.Completed;
            Status = WaitStatus.Completed;
        }
        internal void MoveToMatched(Guid? functionWaitId)
        {
            var functionWait = WaitingFunctions.First(x => x.Id == functionWaitId);
            functionWait.Status = WaitStatus.Completed;
            Status = 
                WaitingFunctions.Count == CompletedFunctions.Count ? 
                WaitStatus.Completed :
                Status;
        }
    }
}