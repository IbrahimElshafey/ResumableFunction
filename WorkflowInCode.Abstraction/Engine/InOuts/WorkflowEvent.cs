using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.Abstraction.Engine.InOuts
{

    public class WorkflowEvent : IEvent
    {
        public LambdaExpression MatchFunction { get; set; }
        public LambdaExpression ContextProp { get; set; }
        public object EventData { get; set; }
    }
}