using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Engine.InOuts;

namespace WorkflowInCode.Abstraction.Engine
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

        protected void PushEvent(dynamic @event)
        {
            //will use engine gRPC client to push an event
        }

    }
}
