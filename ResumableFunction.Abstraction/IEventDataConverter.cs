using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    //may be deleted
    public interface IEventDataConverter
    {
        string UniqeName { get; }
        T Convert<T>(PushedEvent pushedEvent) where T : IEventData;
    }
}
