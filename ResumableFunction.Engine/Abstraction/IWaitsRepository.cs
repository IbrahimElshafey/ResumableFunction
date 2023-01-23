using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IWaitsRepository
    {
        Task<IQueryable<SingleEventWait>> GetWaits(string providerName,string eventType,object eventData);
        Task<List<SingleEventWait>> GetEventWaits(string eventIdentifier, string eventProvider);
    }
}
