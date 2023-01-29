namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AnyFunctionWait : ManyFunctionsWait
    {
        public FunctionWait MatchedFunction { get; set; }
    }
}