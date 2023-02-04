
using ResumableFunction.Abstraction.InOuts;

namespace Example.InOuts
{
    public class ManagerApprovalEvent : IEventData
    {
        public string EventProviderName => Constant.EventProviderName;

        public string EventIdentifier => Constant.ManagerApprovalEvent;

        public int ProjectId { get; set; }
        public bool Decision { get; set; }
    }
}