using System;
using System.Threading.Tasks;

namespace LogicAndTrick.Gimme.Providers
{
    /// <summary>
    /// An async resource provider that calls an action for each item that is loaded
    /// </summary>
    /// <typeparam name="T">The resource type</typeparam>
    public interface IAsyncResourceProvider<T> : IResourceProvider<T> where T : class
    {
        /// <summary>
        /// Fetch the given resource with an async callback
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="callback">The callback to use when each item is loaded</param>
        /// <returns>A task that will complete when all items in the resource are loaded</returns>
        Task Fetch(string location, Action<T> callback);
    }
}
