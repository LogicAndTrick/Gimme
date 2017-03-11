using LogicAndTrick.Gimme.Providers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LogicAndTrick.Gimme.Demo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Gimme.Register(new AsyncResourceProvider());
            Gimme.Register(new SyncResourceProvider());
            Gimme.Register(new ObservableResourceProvider());

            int done = 0;
            var ob = Gimme.Fetch<Thing>("Async", new List<string> { "Resource 1", "Resource 2", "Resource 3", "Resource 4", "Resource 5" });

            ob.Subscribe(x => Console.WriteLine(x.Text), () => done++);

            while (done < 1)
            {
                System.Threading.Thread.Sleep(500);
                Console.WriteLine("Doing some processing...");
            }
        }
    }

    public class SyncResourceProvider : SyncResourceProvider<Thing>
    {
        public override bool CanProvide(string location)
        {
            return location.StartsWith("Sync");
        }

        public override IEnumerable<Thing> Fetch(string location, List<string> resources)
        {
            var r = new Random();
            foreach (var res in resources)
            {
                System.Threading.Thread.Sleep(r.Next(100, 1000));
                yield return new Thing { Text = res };
            }
        }
    }

    public class AsyncResourceProvider : AsyncResourceProvider<Thing>
    {
        public override bool CanProvide(string location)
        {
            return location.StartsWith("Async");
        }

        public override Task Fetch(string location, List<string> resources, Action<Thing> callback)
        {
            return Task.Factory.StartNew(() =>
            {
                var r = new Random();
                foreach (var res in resources)
                {
                    System.Threading.Thread.Sleep(r.Next(100, 1000));
                    callback(new Thing { Text = res });
                }
            });
        }
    }

    public class ObservableResourceProvider : ObservableResourceProvider<Thing>
    {
        public override bool CanProvide(string location)
        {
            return location.StartsWith("Observable");
        }

        public override IObservable<Thing> Fetch(string location, List<string> resources)
        {
            return Observable.Create<Thing>(o =>
            {
                var r = new Random();
                Task.Factory.StartNew(() =>
                {
                    foreach (var res in resources)
                    {
                        System.Threading.Thread.Sleep(r.Next(100, 1000));
                        o.OnNext(new Thing { Text = res });
                    }
                    o.OnCompleted();
                });
                return Disposable.Empty;
            });
        }
    }

    public class Thing
    {
        public string Text { get; set; }
    }
}