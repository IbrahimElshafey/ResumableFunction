
using ResumableFunction.Abstraction.InOuts;

namespace Example.InOuts
{
    public class ManagerApprovalEvent : IEventData
    {
        public string EventProviderName => Constant.EventProviderName;

        public string EventIdentifier { get; set; }

        public int ProjectId { get; set; }
        public string Decision { get; set; }
    }
}