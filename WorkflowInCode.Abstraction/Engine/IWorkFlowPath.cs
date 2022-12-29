namespace WorkflowInCode.Abstraction.Engine
{
    public interface IWorkFlowCombination
    {
        bool RunAll { get; set; }
    }
    public interface IWorkFlowPath
    {
        IWorkFlowPath AllMustStart();
        IWorkFlowPath AllMustFinish();
        IWorkFlowPath FirstMatchAndCancelOthers();
        IWorkFlowPath FirstMatch();
        IWorkFlowPath Parallel();
        IWorkFlowPath Sequential();
    }
}