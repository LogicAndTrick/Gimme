# Gimme
Gimme is an asyncronous resource loader for .NET

Nuget Package: [LogicAndTrick.Gimme](https://www.nuget.org/packages/LogicAndTrick.Gimme)
```
Install-Package LogicAndTrick.Gimme
```

## What's it do?
It's a very simple abstraction over resource providers. Conforming to an interface forces you to design your resource loaders in a particular way. Gimme prioritises asyncronous loading over everything else. Basically it's going to make it hard for you to do it the wrong way.

## How do I use it?
First, write your resource provider. This is as simple as implementing any one of these interfaces:

- `ISyncResourceProvider<T>` - A very simple provider that returns an `IEnumerable<T>`.
- `IAsyncResourceProvider<T>` - A provider that returns a task that completes when all items are loaded. A callback is called for each resource that is loaded.
- `IObservableResourceProvider<T>` - A provider that returns an `IObservable<T>` for those using Reactive Extensions or simply want a subscriber model.

Here's a basic resource provider that will return the numbers 0-9 as strings:

```csharp
public class ExampleResourceProvider : SyncResourceProvider<string>
{
    // True if this class can provide the requested resource
    public override bool CanProvide(string location)
    {
        return location == "Example";
    }
    
    // Return the resource(s)
    public override IEnumerable<Thing> Fetch(string location)
    {
        for (var i = 0; i < 10; i++) yield return i.ToString();
    }
}
```

Next, tell Gimme about your resource provider. At application startup, register your providers:

```csharp
Gimme.Register(new ExampleResourceProvider());
// ... and any others
```

Finally, request the resource. There are a few ways to do this:

- `IObservable<T> Gimme.Fetch<T>(string)` - Request the resource as an observable
- `Task Gimme.Fetch<T>(string, Action<T>)` - Request the resource as an async task
- `Task<T> Gimme.FetchOne<T>(string)` - Request ONE item from a resource as a task and discard any others

This simple example will print the numbers 0-9 from our provider to the console (as long as you wait for the task to complete):
```csharp
var task = Gimme.Fetch<string>("Example", r => {
    Console.WriteLine(r);
});
```

## Why did you call it "Gimme"?
Because boring names are boring.

## License
MIT
