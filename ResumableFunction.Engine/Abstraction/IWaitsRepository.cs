using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IWaitsRepository
    {
        Task AddWait(EventWait eventWait);

        /// <summary>
        /// Load waits with its ParentFunctionInfo that include function data
        /// </summary>
        Task<List<EventWait>> GetMatchedWaits(PushedEvent pushedEvent);
    }
}
