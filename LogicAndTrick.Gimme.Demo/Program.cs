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
            var ob = Gimme.Fetch<Thing>("Async.RESOURCE");

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

        public override IEnumerable<Thing> Fetch(string location)
        {
            var r = new Random();
            foreach (var c in location)
            {
                System.Threading.Thread.Sleep(r.Next(100, 1000));
                yield return new Thing { Text = c.ToString() };
            }
        }
    }

    public class AsyncResourceProvider : AsyncResourceProvider<Thing>
    {
        public override bool CanProvide(string location)
        {
            return location.StartsWith("Async");
        }

        public override Task Fetch(string location, Action<Thing> callback)
        {
            return Task.Factory.StartNew(() =>
            {
                var r = new Random();
                foreach (var c in location)
                {
                    System.Threading.Thread.Sleep(r.Next(100, 1000));
                    callback(new Thing { Text = c.ToString() });
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

        public override IObservable<Thing> Fetch(string location)
        {
            return Observable.Create<Thing>(o =>
            {
                var r = new Random();
                Task.Factory.StartNew(() =>
                {
                    foreach (var c in location)
                    {
                        System.Threading.Thread.Sleep(r.Next(100, 1000));
                        o.OnNext(new Thing { Text = c.ToString() });
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