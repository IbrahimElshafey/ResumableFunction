using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    public interface IActiveEventsRepository
    {
        Task<IQueryable<SingleEventWaiting>> GetActiveEvents(string providerName,string eventType,object eventData);
    }
}
