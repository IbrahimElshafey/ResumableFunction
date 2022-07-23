namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public abstract class WorkflowDefinition<WorkflowContextData>
    {
        public string Name { get; private set; }
        public WorkflowInstance<WorkflowContextData> CurrentInstance { get; }
        public IWorkflowEngine Workflow { get; }
        public WorkflowDefinition(IWorkflowEngine workflow,string name=null)
        {
            Workflow = workflow;
            Name = name ?? GetType().Name;
        }
    }
}
