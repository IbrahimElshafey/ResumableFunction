namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class ManyFunctionsWait : Wait
    {
        public FunctionWait[] WaitingFunctions { get; set; }
    }
    public sealed class AllFunctionsWait : ManyFunctionsWait
    {
       
        public FunctionWait[] CompletedFunctions { get; set; }
    }
}