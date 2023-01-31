namespace ResumableFunction.Abstraction.InOuts
{
    public class ManyFunctionsWait : Wait
    {
        public List<OneFunctionWait> WaitingFunctions { get; set; }
    }
}