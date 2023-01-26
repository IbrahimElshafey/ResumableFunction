using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.InOuts
{
    public class NextWaitResult
    {
        public NextWaitResult(Wait result, bool isFinalEnd, bool isSubFunctionEnd)
        {
            Result = result;
            IsFinalExit = isFinalEnd;
            IsSubFunctionExit = isSubFunctionEnd;
        }
        public Wait Result { get; private set; }
        public bool IsFinalExit { get; private set; }
        public bool IsSubFunctionExit { get; private set; }
    }
}
