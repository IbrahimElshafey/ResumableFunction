using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction.Samples
{
    public class ManagerApprovalEvent : IEventData
    {
        public ManagerApprovalEvent PreviousApproval { get; set; }
        public int ProjectId { get; set; }
        public bool Accepted { get; set; }
        public bool Rejected { get; set; }
        public string EventProviderName => Const.CurrentEventProvider;

        public string EventIdentifier => nameof(ManagerApprovalEvent);

        public override bool Equals(object? obj)
        {
            if (obj is ManagerApprovalEvent input)
            {
                return input.ProjectId == ProjectId && input.Accepted == Accepted && input.Rejected == Rejected;
            }
            return base.Equals(obj);
        }
    }
}
