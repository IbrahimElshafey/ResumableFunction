namespace ResumableFunction.Abstraction.InOuts
{
    public class FunctionWaitingResult : EventWaitingResult
    {
        public string FunctionName { get; set; }
        public EventWaitingResult CurrentEvent { get; internal set; }
    }
}
