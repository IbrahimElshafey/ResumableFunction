using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace ResumableFunction.Abstraction.InOuts
{

    public abstract class Wait
    {
        public Wait()
        {
            //Id = Guid.NewGuid();
        }
        [Key]
        public int Id { get; private set; }
        public WaitStatus Status { get; internal set; }
        public string EventIdentifier { get; internal set; }
        public bool IsFirst { get; internal set; } = false;
        /// <summary>
        /// Resumable function or subfunction that request the event waiting.
        /// </summary>
        public string InitiatedByFunctionName { get; internal set; }
        public int StateAfterWait { get; internal set; }
        public FunctionRuntimeInfo FunctionRuntimeInfo { get; internal set; }
        
        [ForeignKey(nameof(FunctionRuntimeInfo))]
        public int FunctionId { get; internal set; }

        [NotMapped]
        public ResumableFunctionInstance CurrntFunction
        {
            get
            {
                if (FunctionRuntimeInfo is not null)
                    if (FunctionRuntimeInfo.FunctionState is JObject stateAsJson)
                    {
                        var result = (ResumableFunctionInstance)stateAsJson.ToObject(FunctionRuntimeInfo.InitiatedByClassType);
                        FunctionRuntimeInfo.FunctionState = result;
                        return result;
                    }
                    else if (FunctionRuntimeInfo.FunctionState is ResumableFunctionInstance funcInstance)
                        return funcInstance;
                return null;
            }
        }

        public Wait ParentFunctionWait { get; internal set; }

        [ForeignKey(nameof(ParentFunctionWait))]
        public int? ParentFunctionWaitId { get; internal set; }

        public bool IsNode { get; internal set; }

        public ReplayType? ReplayType { get; internal set; }
        public WaitType WaitType { get; internal set; }


    }

}
