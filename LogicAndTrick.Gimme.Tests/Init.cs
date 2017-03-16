using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicAndTrick.Gimme.Tests
{
    [TestClass]
    public static class Init
    {
        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            Gimme.Register(new BasicAsyncProvider(new[] { "One", "Two" }));
            Gimme.Register(new BasicAsyncProvider(new[] { "Three", "Four" }));
            Gimme.Register(new BasicObservableProvider(new[] { "One", "Two" }));
            Gimme.Register(new BasicObservableProvider(new[] { "Three", "Four" }));
            Gimme.Register(new BasicSyncProvider(new[] { "One", "Two" }));
            Gimme.Register(new BasicSyncProvider(new[] { "Three", "Four" }));
        }
    }
}
