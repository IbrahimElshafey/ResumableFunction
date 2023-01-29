using System.ComponentModel.DataAnnotations.Schema;

namespace ResumableFunction.Abstraction.InOuts
{
    public class ResumableFunctionState
    {
        public Guid FunctionId { get; set; }
        public object Data { get; set; }
        public Type DataType { get; set; }

        /// <summary>
        /// The class that contians the resumable functions
        /// </summary>
        public Type InitiatedByClass { get; set; }

        public Dictionary<string, int> FunctionsStates { get; set; } = new Dictionary<string, int>();
        internal Dictionary<string, int> FunctionsStatesHistory { get; set; } = new Dictionary<string, int>();
    }
}