using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicAndTrick.Gimme.Providers
{
    /// <summary>
    /// An abstract implementation of an observable resource provider that automatically provides an async implementation.
    /// </summary>
    /// <typeparam name="T">The resource type</typeparam>
    public abstract class ObservableResourceProvider<T> : IObservableResourceProvider<T>, IAsyncResourceProvider<T> where T : class
    {
        /// <summary>
        /// Convert this resource provider into an async resource provider.
        /// </summary>
        /// <returns>An async resource provider</returns>
        public virtual IAsyncResourceProvider<T> ToAsyncResourceProvider()
        {
            return this;
        }

        /// <summary>
        /// Convert this resource provider into an observable resource provider.
        /// </summary>
        /// <returns>An observable resource provider</returns>
        public virtual IObservableResourceProvider<T> ToObservableResourceProvider()
        {
            return this;
        }

        /// <summary>
        /// Returns true if this resource provider can provide for the given location
        /// </summary>
        /// <param name="location">The location details</param>
        /// <returns>True if this handler can provide the given resource</returns>
        public abstract bool CanProvide(string location);

        /// <summary>
        /// Fetch the given resource as an observable collection
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="resources">The list of resources to fetch</param>
        /// <returns>An observable collection that will publish the loaded resource</returns>
        public abstract IObservable<T> Fetch(string location, List<string> resources);

        /// <summary>
        /// Fetch the given resource with an async callback
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="resources">The list of resources to fetch</param>
        /// <param name="callback">The callback to use when each item is loaded</param>
        /// <returns>A task that will complete when all items in the resource are loaded</returns>
        public Task Fetch(string location, List<string> resources, Action<T> callback)
        {
            var tcs = new TaskCompletionSource<object>();
            IObservable<T> ob = this.Fetch(location, resources);
            ob.Subscribe(new Subscriber(tcs, callback));
            return tcs.Task;
        }

        private class Subscriber : IObserver<T>
        {
            private TaskCompletionSource<object> _source;
            private Action<T> _callback;

            public Subscriber(TaskCompletionSource<object> source, Action<T> callback)
            {
                _source = source;
                _callback = callback;
            }

            public void OnCompleted()
            {
                _source.SetResult(new object());
            }

            public void OnError(Exception ex)
            {
                _source.SetException(ex);
            }

            public void OnNext(T item)
            {
                _callback.Invoke(item);
            }
        }
    }
}
