using System.Collections.ObjectModel;

namespace WorkflowInCode.Abstraction.Engine
{
    public static class WorkflowEngine
    {
        public static IWorkFlowPath Path(string path, params IWorkflowProcessingUnit[] nodes) { return null; }
        public static IWorkFlowPath GoToPath(string path,bool fromStart=true) { return null; }
        public static IWorkflowProcessingUnit Combine(string timeName,Selection waitOption, params IWorkflowProcessingUnit[] processes) { return null; }
    }

    public class Selection
    {
        /// <summary>
        /// Wait until any path complete and cancel others
        /// </summary>
        public static Selection FirstOneAndCancelOthers = nameof(FirstOneAndCancelOthers);
        /// <summary>
        /// Wait until any path complete
        /// </summary>
        public static Selection FirstOne = nameof(FirstOne);
        public static Selection ZeroOrFirstOne = nameof(ZeroOrFirstOne);
        /// <summary>
        /// Wait for all paths to finish
        /// </summary>
        public static Selection AllCompleted = nameof(AllCompleted);
     
        /// <summary>
        /// Wait all paths where first step successed
        /// </summary>
        public static Selection StartedPaths = nameof(StartedPaths);
        public static Selection InRange(int min, int max) => null;
        private string _Name;

        private Selection(string name)
        {
            _Name = name;
        }
        public override bool Equals(object? obj)
        {
            if (obj is Selection x) 
                obj = x._Name;
            return _Name.Equals(obj);
        }

        public static implicit operator Selection(string name)
        {
            return new Selection(name);
        }
    }
}
