using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Abstraction
{
    public interface IEventPusher
    {
        /// <summary>
        /// Remote Call <br/>
        /// Push and event to the engine
        /// </summary>
        Task PushEvent(PushedEvent pushEvent);
        //{
        //    var type = pushEvent.EventData?.GetType();
        //    if (type == null) return;
        //    //pushEvent.Type = $"{type.FullName}#{type.AssemblyQualifiedName}";
        //    pushEvent.EventDataType = type.FullName;
        //    pushEvent.EventDataConverterName = UniqueName;
        //    //will use engine gRPC client to push an event
        //}
    }
}
