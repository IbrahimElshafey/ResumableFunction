using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IEventsRepository
    {
        Task<IQueryable<SingleEventWaiting>> GetActiveWaits(string providerName,string eventType,object eventData);
        Task<List<SingleEventWaiting>> GetEventWaits(PushedEvent pushedEvent);
    }
}
