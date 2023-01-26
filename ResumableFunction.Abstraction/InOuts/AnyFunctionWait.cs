namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class AnyFunctionWait : Wait
    {
        public FunctionWait[] Functions { get; set; }
        public FunctionWait MatchedFunction { get; set; }
    }
}