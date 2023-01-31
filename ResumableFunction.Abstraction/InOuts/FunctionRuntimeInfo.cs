using System.ComponentModel.DataAnnotations.Schema;

namespace ResumableFunction.Abstraction.InOuts
{
    public class FunctionRuntimeInfo
    {
        public Guid FunctionId { get; internal set; }
        public object Data { get; internal set; }
        public Type DataType { get; internal set; }

        /// <summary>
        /// The class that contians the resumable functions
        /// </summary>
        public Type InitiatedByClass { get; internal set; }

        public Dictionary<string, int> FunctionsStates { get; internal set; } = new Dictionary<string, int>();

        public List<Wait> FunctionWaits { get; internal set; } = new List<Wait>();
    }
}