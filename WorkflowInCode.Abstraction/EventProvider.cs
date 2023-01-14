using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Abstraction
{

    /// <summary>
    /// Event provider will listen to events and push events to the engine
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

        public abstract bool SubscribeToEvent(IEventData eventToSubscribe);
        public abstract bool UnSubscribeEvent(IEventData eventToSubscribe);

        protected void PushEvent(PushedEvent pushEvent)
        {
            if (pushEvent == null) throw new NullReferenceException("pushEvent in EventProvider.PushEvent");
            var type = pushEvent.EventData?.GetType();
            if (type == null) return;
            //pushEvent.Type = $"{type.FullName}#{type.AssemblyQualifiedName}";
            pushEvent.EventDataType = type.FullName;
            pushEvent.EventProviderName = UniqueName;
            //will use engine gRPC client to push an event
        }

    }
}
