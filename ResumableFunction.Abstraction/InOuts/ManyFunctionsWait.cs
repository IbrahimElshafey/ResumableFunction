namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class ManyFunctionsWait : Wait
    {
        public FunctionWait[] WaitingFunctions { get; set; }
    }
}