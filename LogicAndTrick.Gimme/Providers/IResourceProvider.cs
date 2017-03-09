namespace LogicAndTrick.Gimme.Providers
{
    /// <summary>
    /// The base interface for a resource provider.
    /// </summary>
    /// <typeparam name="T">The provider type</typeparam>
    public interface IResourceProvider<T> where T : class
    {
        /// <summary>
        /// Returns true if this resource provider can provide for the given location
        /// </summary>
        /// <param name="location">The location details</param>
        /// <returns>True if this handler can provide the given resource</returns>
        bool CanProvide(string location);

        /// <summary>
        /// Convert this resource provider into an async resource provider.
        /// </summary>
        /// <returns>An async resource provider</returns>
        IAsyncResourceProvider<T> ToAsyncResourceProvider();

        /// <summary>
        /// Convert this resource provider into an observable resource provider.
        /// </summary>
        /// <returns>An observable resource provider</returns>
        IObservableResourceProvider<T> ToObservableResourceProvider();
    }
}
