using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction.Samples
{
    public class ProjectRequestedEvent:IEventData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }

        public string EventProviderName => Const.CurrentEventProvider;
    }
}
