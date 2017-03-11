using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Gimme.Providers;

namespace LogicAndTrick.Gimme
{
    /// <summary>
    /// Gimme is an asyncronous resource loader.
    /// </summary>
    public static class Gimme
    {
        private static List<object> _providers = new List<object>();

        /// <summary>
        /// Fetch a single resource asyncronously and return it as a task.
        /// Be warned that ALL resources will be requested anyway, only the first result will be used.
        /// </summary>
        public static Task<T> FetchOne<T>(string location, string resource) where T : class
        {
            var tcs = new TaskCompletionSource<T>();

            bool done = false;
            Fetch<T>(location, new List<string> { resource }, t => {
                if (done) return;
                tcs.SetResult(t);
                done = true;
            });

            return tcs.Task;
        }

        /// <summary>
        /// Fetch a list of resources from a single location and call a callback on each one
        /// </summary>
        public static Task Fetch<T>(string location, List<string> resources, Action<T> callback) where T : class
        {
            return GetAsyncProvider<T>(location).Fetch(location, resources, callback);
        }

        /// <summary>
        /// Fetch a list of resources from a single location as an observable collection
        /// </summary>
        public static IObservable<T> Fetch<T>(string location, List<string> resources) where T : class
        {
            return GetObservableProvider<T>(location).Fetch(location, resources);
        }

        /// <summary>
        /// Get an async provider for a specific type and location
        /// </summary>
        /// <typeparam name="T">The resource type to load</typeparam>
        /// <param name="location">The location to load</param>
        /// <returns>The first async provider that can load the given resource</returns>
        /// <exception cref="InvalidOperationException">If a provider couldn't be located</exception>
        public static IAsyncResourceProvider<T> GetAsyncProvider<T>(string location) where T : class
        {
            return _providers.OfType<IResourceProvider<T>>().First(x => x.CanProvide(location)).ToAsyncResourceProvider();
        }

        /// <summary>
        /// Get an observable provider for a specific type and location
        /// </summary>
        /// <typeparam name="T">The resource type to load</typeparam>
        /// <param name="location">The location to load</param>
        /// <returns>The first observable provider that can load the given resource</returns>
        /// <exception cref="InvalidOperationException">If a provider couldn't be located</exception>
        public static IObservableResourceProvider<T> GetObservableProvider<T>(string location) where T : class
        {
            return _providers.OfType<IResourceProvider<T>>().First(x => x.CanProvide(location)).ToObservableResourceProvider();
        }

        /// <summary>
        /// Register a resource provider
        /// </summary>
        /// <typeparam name="T">The resource type provided by this provider</typeparam>
        /// <param name="provider">The resource provider</param>
        public static void Register<T>(IResourceProvider<T> provider) where T : class
        {
            _providers.Add(provider);
        }
    }
}
