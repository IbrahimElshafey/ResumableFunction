namespace WorkflowInCode.Engine
{
    public class TaskDefinition
    {
        public string Name { get; }
        public Func<Task,bool> CanStart { get; }
        public Action<Task> AfterDone { get; }

        public List<TaskAction> TaskActions { get; set; }
    }

    public class TaskAction
    {
        public int ActionCode { get;}
        public int ActionName { get;}
        public List<TaskDefinition> NextTasks { get;}
        public Action<Task> AfterDone { get; }
    }

    public class TaskInstance:TaskDefinition
    {
        public TaskAction ActionDone { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public bool IsCompletedBySystem { get; set; }

    }
}