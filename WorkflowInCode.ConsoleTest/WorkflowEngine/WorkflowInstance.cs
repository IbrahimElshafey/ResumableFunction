namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public abstract class WorkflowInstance<WorkflowContextData>
    {
        public string WorkflowId { get; private set; }
        public IWorkflow<WorkflowContextData> Workflow { get; }
        public WorkflowContextData ContextData { get; private set; }

        public WorkflowInstance(IWorkflow<WorkflowContextData> workflow)
        {
            Workflow = workflow;
            WorkflowId = Guid.NewGuid().ToString();
        }
        public virtual async Task SaveState() { }
        public virtual async Task<WorkflowContextData> LoadState() { return default; }
    }
}
