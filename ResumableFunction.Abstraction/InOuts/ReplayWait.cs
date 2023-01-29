namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class ReplayWait : Wait
    {
        public string GotoWaitName { get; private set; }

        public ReplayWait(string name)
        {
            GotoWaitName = name;
        }
    }



}
