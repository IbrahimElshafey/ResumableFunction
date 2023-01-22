using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.InOuts
{
    public record WaitResult(EventWaitingResult Result,int State,bool IsEnd)
    {
    }
}
