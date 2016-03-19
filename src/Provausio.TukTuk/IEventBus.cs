using System;

namespace Provausio.TukTuk
{
    public interface IEventBus : IDisposable
    {
        /// <summary>
        /// Subscribes the specified handler to the specified event.
        /// </summary>
        /// <typeparam name="T">The type of event. Must implement <see cref="IEvent"/></typeparam>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        IDisposable Subscribe<T>(Action<IEvent> handler)
            where T : IEvent;

        /// <summary>
        /// Notifies all registered handlers of an event, if they are subscribed to receive notification
        /// </summary>
        /// <typeparam name="TEvent">The type of event. Must implement <see cref="IEvent"/></typeparam>
        /// <param name="ev">The ev.</param>
        void Publish<TEvent>(TEvent ev)
            where TEvent : IEvent;

        /// <summary>
        /// Clears all events and handlers
        /// </summary>
        void Clear();
    }
}