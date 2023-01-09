using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Samples;

namespace WorkflowInCode.Abstraction.Engine
{
    public abstract class ISubscribedEvent
    {
        public object EventData { get; set; }
        public LambdaExpression MatchFunction { get; set; }
        public LambdaExpression ContextProp { get; set; }
    }

    public abstract class IWaitAnyEvent : ISubscribedEvent
    {

    }
    public abstract class IWaitAllEvent : ISubscribedEvent
    {

    }
}