using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Provausio.TukTuk
{
    public class TukBus : IEventBus, IUnsubscribeSource
    {
        private readonly object _syncLock = new object();
        private static ConcurrentBag<EventSubscription> _registeredHandlers 
            = new ConcurrentBag<EventSubscription>();

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Subscribes the specified handler to the specified event.
        /// </summary>
        /// <typeparam name="T">The type of event. Must implement <see cref="IEvent" /></typeparam>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IDisposable Subscribe<T>(Action<IEvent> handler)
            where T : IEvent
        {
            ValidateState();

            if(handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = new EventSubscription(typeof(T), handler, this);
            var existing = _registeredHandlers.SingleOrDefault(h => h.Handler.Equals(handler));

            if (existing != null)
                return existing;

            _registeredHandlers.Add(sub);

            return sub;
        }

        /// <summary>
        /// Unsubscribes the specified subscription.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        public void Unsubscribe(EventSubscription subscription)
        {
            ValidateState();

            EventSubscription removedSub;
            _registeredHandlers.TryTake(out removedSub);
        }

        /// <summary>
        /// Notifies all registered handlers of an event, if they are subscribed to receive notification
        /// </summary>
        /// <typeparam name="TEvent">The type of event. Must implement <see cref="IEvent" /></typeparam>
        /// <param name="ev">The ev.</param>
        public void Publish<TEvent>(TEvent ev)
            where TEvent : IEvent
        {
            ValidateState();

            var handlers = GetHandlers(ev);
            Task.Run(() =>
            {
                foreach (var handler in handlers)
                {
                    handler.Handle(ev);
                }
            });
        }

        /// <summary>
        /// Clears all events and handlers
        /// </summary>
        public void Clear()
        {
            ValidateState();

            lock (_syncLock)
            {
                foreach (var registeredHandler in _registeredHandlers)
                {
                    registeredHandler.Dispose();
                }
                _registeredHandlers = new ConcurrentBag<EventSubscription>();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ValidateState();
            Clear();
            IsDisposed = true;
        }

        private IEnumerable<EventSubscription> GetHandlers<TEvent>(TEvent ev)
            where TEvent : IEvent
        {
            ValidateState();

            var handlers = _registeredHandlers
                .Where(e => e.IsSubscribedTo(ev));

            return handlers;
        }

        private void ValidateState()
        {
            if(IsDisposed)
                throw new InvalidOperationException("The bus has already been disposed.");
        }
    }
}
