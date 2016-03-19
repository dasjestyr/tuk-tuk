using System;

namespace Provausio.TukTuk
{
    public class EventSubscription : IDisposable
    {
        private readonly Type _eventType;
        private readonly IUnsubscribeSource _unsubscribeSource;
        private readonly Guid _subscriptionId;

        public readonly Action<IEvent> Handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSubscription"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="unsubscribeSource">The unsubscribe source.</param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public EventSubscription(Type eventType, Action<IEvent> handler, IUnsubscribeSource unsubscribeSource)
        {
            if(handler == null)
                throw new ArgumentNullException(nameof(handler));

            if(eventType == null)
                throw new ArgumentNullException(nameof(eventType));

            if(unsubscribeSource == null)
                throw new ArgumentNullException(nameof(unsubscribeSource));

            _subscriptionId = Guid.NewGuid();
            _eventType = eventType;
            Handler = handler;
            _unsubscribeSource = unsubscribeSource;
        }

        /// <summary>
        /// Determines whether [is subscribed to] [the specified ev].
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <returns></returns>
        public bool IsSubscribedTo(IEvent ev)
        {
            return _eventType.IsInstanceOfType(ev);
        }

        /// <summary>
        /// Handles the specified ev.
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Handle(IEvent ev)
        {
            if(ev == null)
                throw new ArgumentNullException(nameof(ev));

            Handler(ev);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EventSubscription);
        }

        protected bool Equals(EventSubscription other)
        {
            return other != null &&
                _subscriptionId.Equals(other._subscriptionId);
        }

        public override int GetHashCode()
        {
            return _subscriptionId.GetHashCode();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _unsubscribeSource.Unsubscribe(this);
        }
    }
}
