using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumableFunction.Abstraction.InOuts;

namespace ResumableFunction.Abstraction
{
    /// <summary>
    /// Event provider will listen to events and push events to the engine.<br\>
    /// This may be a client for remote event provider or local implementation.
    /// </summary>
    public interface IEventProvider : IDisposable
    {
        /// <summary>
        /// Unique name for the provider.
        /// </summary>
        string EventProviderName { get; }

        /// <summary>
        /// Start listening to the events,the engine will call this when an event that expected to be handled by this provider
        /// </summary>
        /// <returns></returns>
        Task Start();

        /// <summary>
        /// Stop listening to the all event types,the engine will call this when no active event exist that handled by this provider
        /// </summary>
        /// <returns></returns>
        Task Stop();

        /// <summary>
        /// The engine call this when Function run and ask to wait an event
        /// </summary>
        /// <param name="eventToSubscribe"></param>
        /// <returns></returns>
        Task<bool> SubscribeToEvent(IEventData eventData);

        /// <summary>
        /// The engine will call this if no instance waiting the event type after the event pushed to the engine
        /// </summary>
        /// <param name="eventToSubscribe"></param>
        /// <returns></returns>
        Task<bool> UnSubscribeEvent(IEventData eventData);

    }
}
