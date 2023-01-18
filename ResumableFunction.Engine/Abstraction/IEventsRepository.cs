using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IEventsRepository
    {
        Task<IQueryable<SingleEventWaiting>> GetActiveEvents(string providerName,string eventType,object eventData);
        Task<List<SingleEventWaiting>> GetEvents(PushedEvent pushedEvent);
    }
}
