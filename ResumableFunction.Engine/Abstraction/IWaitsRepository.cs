using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IWaitsRepository
    {
        Task AddWait(Wait eventWait);
        Task<Wait> GetParentFunctionWait(Guid? functionWaitId);

        /// <summary>
        /// Load waits with its ParentFunctionInfo that include function data
        /// </summary>
        Task<List<EventWait>> GetMatchedWaits(PushedEvent pushedEvent);
        Task<ManyEventsWait> GetWaitGroup(Guid? parentGroupId);
        Task DuplicateWaitIfFirst(EventWait currentWait);
    }
}
