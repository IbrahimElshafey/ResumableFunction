using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction
{
    public interface IEventDataConverter
    {
        string UniqeName { get; }
        IEventData Convert(object objectToConvert); 
    }
}
