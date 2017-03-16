using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;
using System.Threading;

namespace LogicAndTrick.Gimme.Tests
{
    [TestClass]
    public class AsyncResourceProviderTest
    {
        [TestMethod]
        public async Task Async_AsyncFetchSimple()
        {
            var called = 0;
            await Gimme.Fetch<string>("Async", new List<string> { "One" }, x =>
            {
                Assert.AreEqual("One", x);
                called++;
            });
            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public async Task Async_AsyncFetchNone()
        {
            var called = 0;
            await Gimme.Fetch<string>("Async", new List<string>(), x =>
            {
                Assert.Fail();
                called++;
            });
            Assert.AreEqual(0, called);
        }

        [TestMethod]
        public async Task Async_AsyncFetchNotFound()
        {
            var called = 0;
            await Gimme.Fetch<string>("Async", new List<string> { "Missing" }, x =>
            {
                Assert.Fail();
                called++;
            });
            await Gimme.Fetch<string>("Invalid", new List<string> { "One" }, x =>
            {
                Assert.Fail();
                called++;
            });
            Assert.AreEqual(0, called);
        }

        [TestMethod]
        public async Task Async_FetchOne()
        {
            var result = await Gimme.FetchOne<string>("Async", "One");
            Assert.AreEqual("One", result);
        }

        [TestMethod]
        public async Task Async_FetchOneMissing()
        {
            var result = await Gimme.FetchOne<string>("Async", "Missing");
            Assert.IsNull(result);

            result = await Gimme.FetchOne<string>("Invalid", "One");
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Async_ObservableFetchSimple()
        {
            var called = 0;
            var completed = false;
            var ob = Gimme.Fetch<string>("Async", new List<string> { "One" });
            ob.Subscribe(x => {
                called++;
                Assert.AreEqual("One", x);
            }, () => completed = true);
            await ob.ToTask();
            Assert.AreEqual(1, called);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        public async Task Async_ObservableFetchNone()
        {
            var called = 0;
            var completed = false;
            var ob = Gimme.Fetch<string>("Async", new List<string>());
            ob.Subscribe(x => {
                called++;
                Assert.Fail();
            }, () => completed = true);
            try
            {
                await ob.ToTask();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
            Assert.AreEqual(0, called);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        public async Task Async_ObservableFetchNotFound()
        {
            var called = 0;
            var completed = false;
            var ob = Gimme.Fetch<string>("Async", new List<string> { "Missing" });
            ob.Subscribe(x => {
                Assert.Fail();
                called++;
            }, () => completed = true);
            try
            {
                await ob.ToTask();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
            Assert.AreEqual(0, called);
            Assert.IsTrue(completed);

            completed = false;
            ob = Gimme.Fetch<string>("Invalid", new List<string> { "One" });
            ob.Subscribe(x => {
                Assert.Fail();
                called++;
            }, () => completed = true);
            try
            {
                await ob.ToTask();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
            Assert.AreEqual(0, called);
            Assert.IsTrue(completed);
        }
    }

    internal class BasicAsyncProvider : Providers.AsyncResourceProvider<string>
    {
        private string[] _result;
        public BasicAsyncProvider(string[] result)
        {
            _result = result;
        }

        public override bool CanProvide(string location)
        {
            return location == "Async";
        }

        public override Task Fetch(string location, List<string> resources, Action<string> callback)
        {
            Parallel.ForEach(_result.Where(resources.Contains), x => {
                callback(x);
            });
            return Task.FromResult(0);
        }
    }
}
