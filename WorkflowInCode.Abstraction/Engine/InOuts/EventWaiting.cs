using System.Linq.Expressions;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Engine
{
    public record EventWaiting<EventData>(
            IEvent EventToWait,
            Expression<Func<EventData, bool>> MatchFunction,
            Expression<Func<EventData>> ContextProp)
    {
        public bool IsOptional { get; set; }
    }

   
}
