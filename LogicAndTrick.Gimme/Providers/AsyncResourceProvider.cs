using LogicAndTrick.Gimme.Observables;
using System;
using System.Threading.Tasks;

namespace LogicAndTrick.Gimme.Providers
{
    /// <summary>
    /// An abstract implementation of an async resource provider that automatically provides an observable implementation.
    /// </summary>
    /// <typeparam name="T">The resource type</typeparam>
    public abstract class AsyncResourceProvider<T> : IAsyncResourceProvider<T>, IObservableResourceProvider<T> where T : class
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
        /// Fetch the given resource with an async callback
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="callback">The callback to use when each item is loaded</param>
        /// <returns>A task that will complete when all items in the resource are loaded</returns>
        public abstract Task Fetch(string location, Action<T> callback);

        /// <summary>
        /// Fetch the given resource as an observable collection
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <returns>An observable collection that will publish the loaded resource</returns>
        public IObservable<T> Fetch(string location)
        {
            var wrapper = new ObservableCollection<T>();
            Fetch(location, wrapper.Add).ContinueWith(t => wrapper.Done());
            return wrapper;
        }
    }
}
