using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IWaitsRepository
    {
        Task AddWait(Wait eventWait);
        Task<object> GetFunctionWait(Guid? functionWaitId);

        //Task AddWait(EventWait eventWait);
        //Task AddWait(AllEventsWait allEventsWait);
        //Task AddWait(AnyEventWait anyEventWait);
        //Task AddWait(FunctionWait functionWait);
        //Task AddWait(AllFunctionsWait allFunctionsWait);
        //Task AddWait(AnyFunctionWait anyFunctionWait);


        /// <summary>
        /// Load waits with its ParentFunctionInfo that include function data
        /// </summary>
        Task<List<EventWait>> GetMatchedWaits(PushedEvent pushedEvent);
        Task<ManyWaits> GetWaitGroup(Guid? parentGroupId);
    }
}
