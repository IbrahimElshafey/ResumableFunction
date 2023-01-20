using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction.Samples
{
    public class ManagerApprovalEvent: IEventData
    {
       public int ProjectId{get;set;}
       public bool Accepted{get;set;}
       public bool Rejected{get;set;}
        public string EventProviderName => Const.CurrentEventProvider;

        public string EventIdentifier => nameof(ManagerApprovalEvent);
    }
}
