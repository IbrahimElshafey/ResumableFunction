using System.Linq.Expressions;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Engine
{
    public class EventWaiting
    {
        public EventWaiting(IEvent eventToWait)
        {

        }

        public bool IsOptional { get; private set; } = false;
        public LambdaExpression MatchExpression { get; private set; }
        public LambdaExpression SetPropExpression { get; private set; }


        public EventWaiting Match<T>(Expression<Func<T, bool>> func) where T : IEvent
        {
            MatchExpression = func;
            return this;
        }

        public EventWaiting SetProp<T>(Expression<Func<T>> instancePropFunc) where T : IEvent
        {
            SetPropExpression = instancePropFunc;
            return this;
        }

        public EventWaiting SetOptional()
        {
            IsOptional = true;
            return this;
        }
    }



}
