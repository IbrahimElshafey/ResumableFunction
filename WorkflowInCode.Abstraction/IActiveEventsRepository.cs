using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction
{
    public interface IActiveEventsRepository
    {
        Task<IQueryable<SingleEventWaiting>> GetActiveEvents(string providerName,string eventType,object eventData);
    }
}
