namespace Provausio.TukTuk
{
    public interface IUnsubscribeSource
    {
        /// <summary>
        /// Unsubscribes the specified subscription.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        void Unsubscribe(EventSubscription subscription);
    }
}