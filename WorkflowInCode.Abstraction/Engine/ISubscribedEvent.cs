using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Samples;

namespace WorkflowInCode.Abstraction.Engine
{
   
    public interface ISubscribedEvent<EventData>
    {
        EventData Result { get;}
    }

}