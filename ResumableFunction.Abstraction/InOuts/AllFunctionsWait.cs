namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllFunctionsWait : ManyFunctionsWait
    {

        public List<FunctionWait> CompletedFunctions { get; set; }

        internal void MoveToMatched(Guid? functionWaitId)
        {
            var functionWait = WaitingFunctions.First(x => x.Id == functionWaitId);
            functionWait.Status = WaitStatus.Completed;
            CompletedFunctions.Add(functionWait);
            WaitingFunctions.Remove(functionWait);
            Status = WaitingFunctions.Count == 0 ? WaitStatus.Completed : Status;
        }
    }
}