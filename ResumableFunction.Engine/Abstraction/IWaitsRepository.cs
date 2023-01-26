using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IWaitsRepository
    {
        Task<IQueryable<EventWait>> GetWaits(string providerName,string eventType,object eventData);
        Task<List<EventWait>> GetEventWaits(string eventIdentifier, string eventProvider);
    }
}
