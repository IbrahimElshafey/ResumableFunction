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

        public FunctionRuntimeInfo FunctionRuntimeInfo { get; set; }
    }
}
