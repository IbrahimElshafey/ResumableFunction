using System.Linq.Expressions;

namespace WorkflowInCode.Abstraction.Engine
{
    //marker interface
    public interface LongRunningTask
    {

    }

    public abstract class LongRunningTask<TaskOutput> : LongRunningTask
    {
        public ISubscribedEvent<TaskOutput> CompletionEvent { get; }
        public TaskOutput Result { get; protected set; }
        public LongRunningTaskStatus Status { get; protected set; }

        public Expression<Func<object, TaskOutput, bool>> MatchingFunction { get; protected set; }
    }

    public abstract class LongRunningTask<InitiationOutput, TaskOutput> : LongRunningTask<TaskOutput>
    {
        //ask manager x to approve request y
        //input may contains (UserId who approve,User who request Id,Request itself,Previuous user comment,...)
        public virtual InitiationOutput Initiate<Input>(Input input) 
        {
            Status = LongRunningTaskStatus.Initiated;
            return default; 
        }

        public new Expression<Func<InitiationOutput, TaskOutput, bool>> MatchingFunction { get; protected set; }
    }

}