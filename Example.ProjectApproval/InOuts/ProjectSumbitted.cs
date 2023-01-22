using ResumableFunction.Abstraction.InOuts;

namespace Example.InOuts
{
    public class ProjectSumbitted : IEventData
    {
        public Project Project { get; set; }

        public string EventProviderName => Constant.EventProviderName;

        public string EventIdentifier => Constant.ProjectSumbittedEvent;
    }
}