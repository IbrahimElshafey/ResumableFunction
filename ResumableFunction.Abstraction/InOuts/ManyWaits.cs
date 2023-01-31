namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class ManyWaits : Wait
    {
        public List<EventWait> WaitingEvents { get; internal set; } = new List<EventWait>();
    }
}