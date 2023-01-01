using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Samples;

namespace WorkflowInCode.Abstraction.Engine
{


    public abstract class LongRunningTask
    {

    }
    public abstract class LongRunningTask<Input, Output>:LongRunningTask
    {
        //ask manager x to approve request y
        //input may contains (UserId who approve,User who request Id,Request itself,Previuous user comment,...)
        public LongRunningTask<Input, Output> Initiate(Input input) { return default; }
        public LongRunningTask<Input, Output> Complete<T>(T input) { return default; }
        
        public  Output Result { get; protected set; }
        public  Expression<Func<Input, Output, bool>> MatchingFunction { get;protected set; }
        public LongRunningTaskStatus Status { get; protected set; }
    }

}