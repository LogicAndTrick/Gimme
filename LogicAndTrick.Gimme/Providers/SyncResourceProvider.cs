using LogicAndTrick.Gimme.Observables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicAndTrick.Gimme.Providers
{
    /// <summary>
    /// An abstract implementation of a sync resource provider that automatically provides observable and async implementations.
    /// </summary>
    /// <typeparam name="T">The resource type</typeparam>
    public abstract class SyncResourceProvider<T> : ISyncResourceProvider<T>, IAsyncResourceProvider<T>, IObservableResourceProvider<T> where T : class
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
        /// Synchronously loads the given resource as an enumerable
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="resources">The list of resources to fetch</param>
        /// <returns>An enumerable list</returns>
        public abstract IEnumerable<T> Fetch(string location, List<string> resources);

        /// <summary>
        /// Fetch the given resource with an async callback
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="resources">The list of resources to fetch</param>
        /// <param name="callback">The callback to use when each item is loaded</param>
        /// <returns>A task that will complete when all items in the resource are loaded</returns>
        public Task Fetch(string location, List<string> resources, Action<T> callback)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var t in Fetch(location, resources))
                {
                    try
                    {
                        callback(t);
                    }
                    catch
                    {
                        // Swallow exceptions from the consumer
                    }
                }
            });
        }

        /// <summary>
        /// Fetch the given resource as an observable collection
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="resources">The list of resources to fetch</param>
        /// <returns>An observable collection that will publish the loaded resource</returns>
        IObservable<T> IObservableResourceProvider<T>.Fetch(string location, List<string> resources)
        {
            var wrapper = new ObservableCollection<T>();
            Fetch(location, resources, wrapper.Add).ContinueWith(t => wrapper.Done());
            return wrapper;
        }
    }
}
