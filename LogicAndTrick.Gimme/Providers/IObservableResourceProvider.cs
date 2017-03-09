using System;

namespace LogicAndTrick.Gimme.Providers
{
    /// <summary>
    /// An observable resource provider that provides a resource as an observable collection
    /// </summary>
    /// <typeparam name="T">The resource type</typeparam>
    public interface IObservableResourceProvider<T> : IResourceProvider<T> where T : class
    {
        /// <summary>
        /// Fetch the given resource as an observable collection
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <returns>An observable collection that will publish the loaded resource</returns>
        IObservable<T> Fetch(string location);
    }
}
