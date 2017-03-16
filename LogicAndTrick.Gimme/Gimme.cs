using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Gimme.Providers;
using System.Reactive.Linq;

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

            Fetch<T>(location, new List<string> { resource }, t => {
                tcs.TrySetResult(t);
            }).ContinueWith(x => {
                tcs.TrySetResult(default(T));
            });

            return tcs.Task;
        }

        /// <summary>
        /// Fetch a list of resources from a single location and call a callback on each one
        /// </summary>
        public static Task Fetch<T>(string location, List<string> resources, Action<T> callback) where T : class
        {
            var tasks = GetAsyncProviders<T>(location).Select(x => x.Fetch(location, resources, callback)).ToArray();
            var result = tasks.Length == 0 ? Task.FromResult(0) : Task.WhenAll(tasks);
            result.ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Fetch a list of resources from a single location as an observable collection
        /// </summary>
        public static IObservable<T> Fetch<T>(string location, List<string> resources) where T : class
        {
            return GetObservableProviders<T>(location).Select(x => x.Fetch(location, resources)).Merge();
        }

        /// <summary>
        /// Get an async provider for a specific type and location
        /// </summary>
        /// <typeparam name="T">The resource type to load</typeparam>
        /// <param name="location">The location to load</param>
        /// <returns>The first async provider that can load the given resource</returns>
        /// <exception cref="InvalidOperationException">If a provider couldn't be located</exception>
        public static IEnumerable<IAsyncResourceProvider<T>> GetAsyncProviders<T>(string location) where T : class
        {
            return _providers.OfType<IResourceProvider<T>>().Where(x => x.CanProvide(location)).Select(x => x.ToAsyncResourceProvider());
        }

        /// <summary>
        /// Get an observable provider for a specific type and location
        /// </summary>
        /// <typeparam name="T">The resource type to load</typeparam>
        /// <param name="location">The location to load</param>
        /// <returns>The first observable provider that can load the given resource</returns>
        /// <exception cref="InvalidOperationException">If a provider couldn't be located</exception>
        public static IEnumerable<IObservableResourceProvider<T>> GetObservableProviders<T>(string location) where T : class
        {
            return _providers.OfType<IResourceProvider<T>>().Where(x => x.CanProvide(location)).Select(x => x.ToObservableResourceProvider());
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

        public static void Unregister<T>(IResourceProvider<T> provider) where T: class
        {
            _providers.Remove(provider);
        }

        public static void UnregisterAll()
        {
            _providers.Clear();
        }
    }
}
