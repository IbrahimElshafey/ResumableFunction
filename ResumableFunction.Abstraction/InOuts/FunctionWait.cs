namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class FunctionWait : Wait
    {
        public string FunctionName { get; set; }
        public Wait CurrentEvent { get; internal set; }
    }
}
