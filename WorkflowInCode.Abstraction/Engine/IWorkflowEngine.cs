using System.Collections.ObjectModel;

namespace WorkflowInCode.Abstraction.Engine
{
    public static class WorkflowEngine
    {
        public static IWorkFlowPath Path(string path, params IWorkflowProcessingUnit[] nodes) { return null; }
        public static IWorkFlowPath Path(string path, IWorkflowProcessingUnit nodes) { return null; }
        public static IWorkflowProcessingUnit SameTime(string timeName, params IWorkflowProcessingUnit[] processes) { return null; }
        public static IWorkflowProcessingUnit SameTime(string timeName,WaitOption waitOption, params IWorkflowProcessingUnit[] processes) { return null; }
        public static IWorkflowProcessingUnit AnyOf(string name,params IWorkflowProcessingUnit[] nodes) { return null; }
        public static IWorkflowProcessingUnit Selct(string name,WaitOption waitOption,params IWorkflowProcessingUnit[] nodes) { return null; }
        public static IWorkflowProcessingUnit StartEvent<T>(ISubscribeEvent<T> subscribeEvent) { return null; }
    }

    public class WaitOption
    {
        public static WaitOption One = null;
        public static WaitOption ZeroOrOne = null;
        public static WaitOption OneOrMore = null;
        public static WaitOption All = null;
        public static WaitOption InRange(int min, int max) => null;
        private string _Name;

        private WaitOption(string name)
        {
            _Name = name;
        }
        public override bool Equals(object? obj)
        {
            if (obj is WaitOption x) 
                obj = x._Name;
            return _Name.Equals(obj);
        }
    }
}
