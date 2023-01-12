using System.Linq.Expressions;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Engine
{
    public class EventWaiting
    {
        public EventWaiting(dynamic eventToWait)
        {

        }

        public bool IsOptional { get; private set; } = false;
        public LambdaExpression MatchExpression { get; private set; }
        public LambdaExpression SetPropExpression { get; private set; }


        public EventWaiting Match<T>(Expression<Func<T, bool>> func)
        {
            MatchExpression = func;
            return this;
        }

        public EventWaiting SetProp<T>(Expression<Func<T>> instancePropFunc)
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
