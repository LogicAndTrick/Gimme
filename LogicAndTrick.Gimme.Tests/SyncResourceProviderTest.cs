using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;

namespace LogicAndTrick.Gimme.Tests
{
    [TestClass]
    public class SyncResourceProviderTest
    {
        [TestMethod]
        public async Task Sync_AsyncFetchSimple()
        {
            var called = 0;
            await Gimme.Fetch<string>("Sync", new List<string> { "One" }, x =>
            {
                Assert.AreEqual("One", x);
                called++;
            });
            Assert.AreEqual(1, called);
        }

        [TestMethod]
        public async Task Sync_AsyncFetchNone()
        {
            var called = 0;
            await Gimme.Fetch<string>("Sync", new List<string>(), x =>
            {
                Assert.Fail();
                called++;
            });
            Assert.AreEqual(0, called);
        }

        [TestMethod]
        public async Task Sync_AsyncFetchNotFound()
        {
            var called = 0;
            await Gimme.Fetch<string>("Sync", new List<string> { "Missing" }, x =>
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
        public async Task Sync_FetchOne()
        {
            var result = await Gimme.FetchOne<string>("Sync", "One");
            Assert.AreEqual("One", result);
        }

        [TestMethod]
        public async Task Sync_FetchOneMissing()
        {
            var result = await Gimme.FetchOne<string>("Sync", "Missing");
            Assert.IsNull(result);

            result = await Gimme.FetchOne<string>("Invalid", "One");
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Sync_ObservableFetchSimple()
        {
            var called = 0;
            var completed = false;
            var ob = Gimme.Fetch<string>("Sync", new List<string> { "One" });
            ob.Subscribe(x => {
                called++;
                Assert.AreEqual("One", x);
            }, () => completed = true);
            await ob.ToTask();
            Assert.AreEqual(1, called);
            Assert.IsTrue(completed);
        }

        [TestMethod]
        public async Task Sync_ObservableFetchNone()
        {
            var called = 0;
            var completed = false;
            var ob = Gimme.Fetch<string>("Sync", new List<string>());
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
        public async Task Sync_ObservableFetchNotFound()
        {
            var called = 0;
            var completed = false;
            var ob = Gimme.Fetch<string>("Sync", new List<string> { "Missing" });
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

    class BasicSyncProvider : Providers.SyncResourceProvider<string>
    {
        private string[] _result;
        public BasicSyncProvider(string[] result)
        {
            _result = result;
        }

        public override bool CanProvide(string location)
        {
            return location == "Sync";
        }

        public override IEnumerable<string> Fetch(string location, List<string> resources)
        {
            foreach (var r in resources)
            {
                if (_result.Contains(r)) yield return r;
            }
        }
    }
}
