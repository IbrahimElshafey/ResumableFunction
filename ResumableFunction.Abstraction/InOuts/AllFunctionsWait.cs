namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AllFunctionsWait : ManyFunctionsWait
    {
       
        public OneFunctionWait[] CompletedFunctions { get; set; }
    }
}