namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class ManyWaits : Wait
    {
        public string Name { get; set; }
        public EventWait[] WaitingEvents { get; set; }
    }
    public sealed class AnyEventWait : ManyWaits
    {
        public EventWait MatchedEvent { get; set; }
    }
}