using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumableFunction.Abstraction.InOuts
{
    public class FunctionRuntimeInfo
    {
        [Key]
        public Guid FunctionId { get; internal set; }

        /// <summary>
        /// The class that contians the resumable functions
        /// </summary>
        public Type InitiatedByClassType { get; internal set; }

        //has the state serialzed
        public object FunctionState { get; internal set; }

        public List<Wait> FunctionWaits { get; internal set; } = new List<Wait>();
    }
}