using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace WorkflowInCode.Abstraction.Engine
{
    public static class Workflow
    {
        public static IWorkFlowPath DefineProcesses(Expression<Func<IProcess[]>> processes) { return null; }
        public static IWorkFlowPath DefinePaths(params Expression<Func<IWorkFlowPath>>[] paths) { return null; }
        public static IWorkFlowPath Path(string path, params IWorkflowProcessingUnit[] nodes) { return null; }
        public static IWorkFlowPath GoToPath(string path,bool fromStart=true) { return null; }
        public static IWorkflowProcessingUnit Combine(string timeName,Selection waitOption, params IWorkflowProcessingUnit[] processes) { return null; }
    }
}
