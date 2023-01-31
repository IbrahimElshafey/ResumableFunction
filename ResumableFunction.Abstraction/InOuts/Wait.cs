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

        internal FunctionRuntimeInfo FunctionRuntimeInfo { get; set; }
        public string EventIdentifier { get; internal set; }
        public bool IsFirst { get; internal set; } = false;
        public string InitiatedByFunction { get; internal set; }
        internal int StateAfterWait { get; set; }
    }
}
