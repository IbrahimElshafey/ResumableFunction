namespace WorkflowInCode.Abstraction.Engine
{
    public interface IWorkFlowPath
    {
        IWorkFlowPath ThenProcess<InputType, OutputType>(IProcess process, Func<OutputType, bool> nextStepFilter);
        void EndPath<InputType, OutputType>(IProcess process);

        void ThenParallelPaths(params IWorkFlowPath[] paths);
    }
}