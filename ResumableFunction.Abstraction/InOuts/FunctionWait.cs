using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ResumableFunction.Abstraction.InOuts
{
    public sealed class FunctionWait : Wait
    {
        public FunctionWait(string eventIdentifier, Expression<Func<IAsyncEnumerable<Wait>>> function)
        {
            EventIdentifier = eventIdentifier;
            Function = function;
            //InitiatedByFunction = callerName;
        }
        public Guid? ParentFunctionGroupId { get; internal set; }
        public string FunctionName { get; set; }
        public Wait CurrentEvent { get; internal set; }
        public Expression<Func<IAsyncEnumerable<Wait>>> Function { get; internal set; }
    }
}
