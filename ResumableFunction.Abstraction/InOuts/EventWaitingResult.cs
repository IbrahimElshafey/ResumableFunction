namespace ResumableFunction.Abstraction.InOuts
{
    public abstract class EventWaitingResult
    {
        public EventWaitingResult()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
    }
}
