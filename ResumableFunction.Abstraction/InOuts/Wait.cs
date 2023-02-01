using System.ComponentModel.DataAnnotations.Schema;
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

        public ReplayType? ReplayType { get; internal set;}
    }

    public enum ReplayType
    {
        ExecuteDontWait,
        WaitNewEvent,
    }

}
