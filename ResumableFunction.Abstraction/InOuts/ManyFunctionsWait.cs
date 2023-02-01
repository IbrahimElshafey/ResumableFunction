namespace ResumableFunction.Abstraction.InOuts
{
    public class ManyFunctionsWait : Wait
    {
        public List<FunctionWait> WaitingFunctions { get; set; }
    }
}