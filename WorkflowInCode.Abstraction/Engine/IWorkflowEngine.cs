namespace WorkflowInCode.Abstraction.Engine
{
    public interface IWorkflowEngine
    {
        IWorkFlowPath DefinePath(string path, IProcess process, Func<IProcessOutput, bool> nextStepFilter);
        void DefinePath(string v, object value, object );
        IWorkFlowPath Start(string path,IProcess process,Func<IProcessOutput,bool> nextStepFilter);
        void Start(params (string path, IProcess process, Func<IProcessOutput, bool> nextStepFilter)[] processes);
    }
}
