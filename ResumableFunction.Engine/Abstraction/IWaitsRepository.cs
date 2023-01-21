using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IWaitsRepository
    {
        Task<IQueryable<SingleEventWaiting>> GetWaits(string providerName,string eventType,object eventData);
        Task<List<SingleEventWaiting>> GetEventWaits(PushedEvent pushedEvent);
    }
}
