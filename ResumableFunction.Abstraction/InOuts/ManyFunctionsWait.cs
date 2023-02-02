namespace ResumableFunction.Abstraction.InOuts
{
    public class ManyFunctionsWait : Wait
    {
        public List<FunctionWait> WaitingFunctions { get; internal set; }

        internal AllFunctionsWait ToAllFunctionsWait()
        {
            return new AllFunctionsWait
            {
                WaitingFunctions = WaitingFunctions,
            };
        }

        internal AnyFunctionWait ToAnyFunctionWait()
        {
            return new AnyFunctionWait
            {
                WaitingFunctions = WaitingFunctions,
            };
        }
    }
}