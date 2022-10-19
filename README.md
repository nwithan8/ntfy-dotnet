## ntfy-dotnet

---

A .NET library for [ntfy](https://ntfy.sh), allowing sending and receiving messages.

Useful for:
 - Xamarin (mobile) apps
 - .NET apps
 - C# scripts

### Installation

Download the latest release from the [releases page](https://github.com/nwithan8/ntfy-dotnet/releases) or from [NuGet](https://www.nuget.org/packages/ntfy-dotnet/).

### Usage

#### Sending a message

```csharp
using ntfy;

// Create a new client
var client = new Client("https://ntfy.sh");

// Publish a message to the "test" topic
var message = new SendingMessage
        {
            Title = "This is a demo.",
            Actions = new Action.Action[]
            {
                new Broadcast("label")
                {
                },
                new View("label2", new Uri("https://google.com"))
                {
                }
            }
        };

await client.Publish("test", message);
```

#### Receiving a message

```csharp
using ntfy;

// Create a new client
var client = new Client("https://ntfy.sh");

// Fiter messages by priority
var filter = new Filter
{
    Priorities = new PriorityLevel[] { Priority.High },
};

// Filter messages since 1 hour ago
var since = new Since(new DelayDuration(1, DelayUnit.Hours));

// Subscribe to the "test" topic
vvar subscription = client.Subscribe(new string[] { "test" }, since: since, filters: filter);

// Process a new message when it arrives
await foreach (var message in subscription.WithCancellation(default))
{
    // Do something with the message
}
```

#### Users

You can specify a specific user for sending and receiving messages on private topics.

```csharp
using ntfy;

// Create a new user
var user = new User("username", "password");

// Pass this user into the Subscribe, Publish, Poll methods

// Verify that the user has access to the "test" topic
var client = new Client("https://ntfy.sh");
bool allowed = await client.CheckAuthentication("test", user);

// You can also call this method without a user to see if the topic is anonymously accessible
```


### Documentation

See the [documentation](https://ntfy-dotnet.nateharr.is/api/ntfy.html) for more information.

### Dependencies

- `Microsoft.Bcl.AsyncInterfaces`
- `N8.NetTools.HTTP`

### Supported Frameworks

- .NET Standard 2.0
- .NET Standard 2.1
- .NET Core 3.1
- .NET 5.0
- .NET 6.0
