using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction
{

    /// <summary>
    /// Event provider will push events to the engine
    /// </summary>
    public abstract class EventProvider
    {
        public abstract string UniqueName { get; }
        /// <summary>
        /// Start listening to the events
        /// </summary>
        /// <returns></returns>
        public abstract Task Start();
        /// <summary>
        /// Stop listening to the events
        /// </summary>
        /// <returns></returns>
        public abstract Task Stop();

        public abstract bool SubscribeToEvent(IEvent eventToSubscribe);

        protected void PushEvent(PushEvent pushEvent)
        {
            if (pushEvent == null) return;
            pushEvent.ProviderName = UniqueName;
            var type = pushEvent.Payload?.GetType();
            if (type == null) return;
            //pushEvent.Type = $"{type.FullName}#{type.AssemblyQualifiedName}";
            pushEvent.EventType = type.FullName;
            //will use engine gRPC client to push an event
        }

    }
}
