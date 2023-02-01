using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace ResumableFunction.Abstraction.InOuts
{

    public abstract class Wait
    {
        public Wait()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
        public WaitStatus Status { get; internal set; }
        public string EventIdentifier { get; internal set; }
        public bool IsFirst { get; internal set; } = false;
        /// <summary>
        /// Resumable function or subfunction that request the event waiting.
        /// </summary>
        public string InitiatedByFunctionName { get; internal set; }
        public int StateAfterWait { get; internal set; }
        public FunctionRuntimeInfo FunctionRuntimeInfo { get; internal set; }
        public ResumableFunctionInstance CurrntFunction { get; internal set; }
        public Guid? ParentFunctionWaitId { get; internal set; }
        public bool IsNode { get; internal set; }

        public ReplayType? ReplayType { get; internal set; }

        //internal Wait UpdateForReplay()
        //{
        //    Wait result = null;
        //    switch (this)
        //    {
        //        case EventWait eventWait:
        //            result = eventWait;
        //            break;
        //        case AllEventsWait allEventsWait:
        //            if(allEventsWait.MatchedEvents?.Any() is true)
        //                allEventsWait.WaitingEvents.AddRange(allEventsWait.MatchedEvents);
        //            allEventsWait.WaitingEvents.ForEach(x=>x.Status)
        //            result = allEventsWait;
        //            break;
        //        case AnyEventWait anyEventWait:
        //            result = anyEventWait;
        //            break;
        //        case FunctionWait functionWait:
        //            result = functionWait;
        //            break;
        //        case AllFunctionsWait allFunctionsWait:
        //            result = allFunctionsWait;
        //            break;
        //        case AnyFunctionWait anyFunctionWait:
        //            result = anyFunctionWait;
        //            break;
        //    }
        //    result.Status = WaitStatus.Waiting;
        //    return result;
        //}


    }

    public enum ReplayType
    {
        ExecuteDontWait,
        WaitSameEventAgain,
    }

}
