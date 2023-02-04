using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    /// <summary>
    /// The function that paused when WaitEvent requested and resumed when event come.
    /// `FunctionData` Must be a class with parameter less constructor.
    /// </summary>
    //Can i make this class not generic? we use generic for match and set property expressions only?

    public abstract partial class ResumableFunctionInstance : IResumableFunction
    {
        internal string Name => GetType().FullName;
        public ResumableFunctionInstance()
        {
            FunctionRuntimeInfo = new FunctionRuntimeInfo
            {
                //FunctionId = Guid.NewGuid(),
                InitiatedByClassType = GetType(),
                FunctionState = this,
            };
        }

        //public FunctionData Data { get; set; }
        [JsonIgnore]
        public FunctionRuntimeInfo FunctionRuntimeInfo { get; internal set; }

        public abstract IAsyncEnumerable<Wait> Start();
        public virtual Task OnFunctionEnd()
        {
            return Task.CompletedTask;
        }



    }


}
