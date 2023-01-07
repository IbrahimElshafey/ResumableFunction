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
        public string EventName { get; protected set; }
        public object EventData { get; set; }
        public LambdaExpression MatchFunction { get; set; }
        public string ContextProp { get; set; }
    }


  
    //public interface ISubscribedEvent<out EventData>: ISubscribedEvent
    //{
    //    new EventData Result { get;}
    //}




}