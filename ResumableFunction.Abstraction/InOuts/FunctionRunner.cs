namespace ResumableFunction.Abstraction.InOuts
{
    public class FunctionRunner
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public EventWaitingResult WaitingEvent { get; set; }
    }
}