namespace ResumableFunction.Abstraction.InOuts
{
    public class ManyWaits : Wait
    {
        public List<EventWait> WaitingEvents { get; internal set; } = new List<EventWait>();
    }
}