namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllFunctionsWait : ManyFunctionsWait
    {
       
        public FunctionWait[] CompletedFunctions { get; set; }
    }
}