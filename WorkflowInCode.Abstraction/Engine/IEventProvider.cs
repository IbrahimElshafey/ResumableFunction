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
    public interface IEventProvider
    {
        string UniqueName { get; }
        /// <summary>
        /// Start listening to the events
        /// </summary>
        /// <returns></returns>
        Task Start();
        /// <summary>
        /// Stop listening to the events
        /// </summary>
        /// <returns></returns>
        Task Stop();

    }
}
