using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public class LongTask<EventData, ActionData>
    {


        public LongTask(string taskName,
            ICommand initiationCommand,
            IEvent<EventData> wakeUpEvent,
            Func<EventData, Task> afterWakeUpAction)
        {
            TaskName = taskName;
            InitiationCommand = initiationCommand;
            WakeUpEvent = wakeUpEvent;
            AfterWakeUpAction = afterWakeUpAction;
        }

        public string TaskName { get; }
        public ICommand InitiationCommand { get; }
        public IEvent<EventData> WakeUpEvent { get; }
        public Expression<Func<EventData, string>> EventcCorrelation { get; }
        public Func<EventData, Task> AfterWakeUpAction { get; }
        public ICommand CancelCommand { get; }
        public ProcessStatus Status { get; } = ProcessStatus.Created;
        public ActionData TakenAction { get; }
        public Func<Task<bool>> IsCompleted { get;}
        public string Message { get; }
    }


    public enum ProcessStatus
    {
        Defined = 0,
        Created = 10,
        Initiated = 20,
        InitiationFailed = -20,
        WaitingAction = 30,
        ActionStarted = 40,
        ActionCompleted = 50,
        ActionFailed = -50,
        CanceledBySystem = 60,
        CanceledByUser = 70,
    }
}
