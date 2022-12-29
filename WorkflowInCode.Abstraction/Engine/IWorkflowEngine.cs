using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace WorkflowInCode.Abstraction.Engine
{
    public static class Workflow
    {
        public static Input InputOf<Input, Output>(Func<Input,Output> func) { return default; }
        public static IWorkFlowPath Path(string path, params object[] nodes) { return null; }
    }

    public class WorkflowDefinition 
    {
        public void DefineProcesses(Expression<Func<object[]>> processes) { }
        public void DefinePaths(params Expression<Func<IWorkFlowPath>>[] paths) { }
    }
}
    