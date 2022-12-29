using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Samples;

namespace WorkflowInCode.Abstraction.Engine
{
    public interface IWorkFlowProcess
    {

        //public abstract void DefineTask(
        //    Expression<Func<object, object>> initiateTaskFunc,
        //    Expression<Func<object, object>> replyToTaskFunc,
        //    Expression<bool[]> outPutNodes);
    }

    public record LongRunningTask<Input, Output>(
        Func<Input, Output> Initiate,
        Func<Input, Output> Reply,
        Output Data,
        Expression<Func<Input,Output,bool>> MatchingFunction)
    {
    }

}