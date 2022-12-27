namespace WorkflowInCode.Abstraction.Engine
{
    public interface IWorkFlowPath: IWorkflowProcessingUnit
    {
        IWorkFlowPath End();
        //IWorkFlowPath Then(IWorkflowProcessingUnit node);
    }
}