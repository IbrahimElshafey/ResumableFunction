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

        public ResumableFunctionState ParentFunctionState { get; set; }
    }
}
