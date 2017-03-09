using System.Collections.Generic;

namespace LogicAndTrick.Gimme.Providers
{
    /// <summary>
    /// A synchronous provider for when you don't want to do anything special
    /// </summary>
    /// <typeparam name="T">The resource type</typeparam>
    public interface ISyncResourceProvider<T> : IResourceProvider<T> where T : class
    {
        /// <summary>
        /// Synchronously loads the given resource as an enumerable
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <returns>An enumerable list</returns>
        IEnumerable<T> Fetch(string location);
    }
}
