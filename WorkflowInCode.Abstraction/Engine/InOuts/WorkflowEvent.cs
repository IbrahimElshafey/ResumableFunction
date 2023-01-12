using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.Abstraction.Engine.InOuts
{

    public class WorkflowEvent
    {
        public dynamic EventData { get; set; }
        public LambdaExpression MatchExpression { get; private set; }
        public LambdaExpression SetPropExpression { get; private set; }


        public WorkflowEvent Match<T>(Expression<Func<T, bool>> func)
        {
            MatchExpression = func;
            return this;
        }

        public WorkflowEvent SetProp<T>(Expression<Func<T>> instancePropFunc)
        {
            SetPropExpression = instancePropFunc;
            return this;
        }
    }
}