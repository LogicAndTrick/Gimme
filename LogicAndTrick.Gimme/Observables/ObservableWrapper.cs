using System;
using System.Collections.Concurrent;

namespace LogicAndTrick.Gimme.Observables
{
    /// <summary>
    /// The observable collection is a simple thread-safe wrapper around the observable interface and a list.
    /// If you want to implement an observable resource provider, this an easy way to do it.
    /// </summary>
    /// <typeparam name="T">The collection type</typeparam>
    public class ObservableCollection<T> : IObservable<T>
    {
        private ConcurrentDictionary<Subscription, bool> _observers;
        private BlockingCollection<T> _items;
        private Exception _error;

        /// <summary>
        /// Create a new observable collection
        /// </summary>
        public ObservableCollection()
        {
            _observers = new ConcurrentDictionary<Subscription, bool>();
            _items = new BlockingCollection<T>();
            _error = null;
        }

        /// <summary>
        /// Mark the collection as errored and stop further items from being added. Publishers will be immediately notified.
        /// </summary>
        /// <param name="ex">The exception details</param>
        public void Error(Exception ex)
        {
            _error = ex;
            foreach (var o in _observers.Keys) o.Observer.OnError(ex);
            Done();
        }

        /// <summary>
        /// Add a new item to the collection. Publishers will be immediately notified on the same thread.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _items.Add(item);
            foreach (var o in _observers.Keys) o.Observer.OnNext(item);
        }

        /// <summary>
        /// Mark the collection as completed and stop further items from being added. Publishers will be immediately notified.
        /// </summary>
        public void Done()
        {
            foreach (var o in _observers.Keys) o.Observer.OnCompleted();
            _observers.Clear();
            _items.CompleteAdding();
            _items.Dispose();
        }

        /// <summary>
        /// Subscribe to this observable collection.
        /// The subscriber will be immediately notified of any data that has been previously added to the collection.
        /// </summary>
        /// <param name="observer">The observer to add</param>
        /// <returns>An object that will unsubscribe the observer when disposed.</returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new Subscription(this, observer);
            if (_error != null)
            {
                observer.OnError(_error);
            }
            else
            {
                _observers.TryAdd(sub, true);
                foreach (var item in _items) observer.OnNext(item);
            }
            return sub;
        }

        private class Subscription : IDisposable
        {
            public ObservableCollection<T> ObservableWrapper { get; }
            public IObserver<T> Observer { get; }

            public Subscription(ObservableCollection<T> observableWrapper, IObserver<T> observer)
            {
                ObservableWrapper = observableWrapper;
                Observer = observer;
            }

            public void Dispose()
            {
                ObservableWrapper._observers.TryRemove(this, out bool b);
            }
        }
    }
}
