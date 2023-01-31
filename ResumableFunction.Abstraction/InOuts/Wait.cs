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
        internal int StateAfterWait { get; set; }
        internal FunctionRuntimeInfo FunctionRuntimeInfo { get; set; }
        internal ResumableFunctionInstance CurrntFunction { get; set; }
        internal Guid? FunctionWaitId { get; set; }
        internal bool IsNode { get; set; }
    }

}
