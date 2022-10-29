using ntfy.Actions;
using Xunit.Abstractions;

namespace ntfy.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private Client GetClient() => new Client("https://ntfy.nateharr.is");

    private string[] GetTopics() => new string[] { "csharp-test" };

    [Fact]
    public async Task TestPublish()
    {
        var client = GetClient();

        var message = new SendingMessage
        {
            Title = "This is a demo.",
            Actions = new Actions.Action[]
            {
                new Broadcast("label")
                {
                },
                new View("label2", new Uri("https://google.com"))
                {
                }
            }
        };

        foreach (var action in message.Actions)
        {
            var type = action.Type;
        }

        await client.Publish("topic", message);
    }

    [Fact]
    public async Task TestSubscribe()
    {
        var client = GetClient();

        var filter = new ReceptionFilters
        {
            Priorities = new PriorityLevel[]
            {
                PriorityLevel.Low,
                PriorityLevel.Default,
            }
        };

        var since = new Since(new DelayDuration(1, DelayUnit.Hours));

        var subscription = client.Subscribe(GetTopics(), since: since, filters: filter);

        await foreach (var notification in subscription.WithCancellation(default))
        {
            _testOutputHelper.WriteLine(notification.ToString());
            _testOutputHelper.WriteLine(notification.Actions?.ToString());
        }
    }

    [Fact]
    public async Task TestSubscribeAndProcess()
    {
        var client = GetClient();

        await client.SubscribeAndProcess(GetTopics(), (message) =>
        {
            _testOutputHelper.WriteLine(message.Title ?? message.Id);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task TestPoll()
    {
        var client = GetClient();

        var filter = new ReceptionFilters
        {
            Priorities = new PriorityLevel[]
            {
                PriorityLevel.Low,
                PriorityLevel.Default,
            }
        };

        var messages = await client.Poll(GetTopics(), since: Since.All, filters: filter);

        foreach (var message in messages)
        {
            _testOutputHelper.WriteLine(message.Title ?? message.Id);
        }
    }

    [Fact]
    public async Task TestAuthentication()
    {
        var client = GetClient();

        var allowed = await client.CheckAuthentication(GetTopics()[0]);

        Assert.True(allowed);
    }

    [Fact]
    public async Task TestServerInfo()
    {
        var client = GetClient();

        var info = await client.GetServerInfo();

        Assert.NotNull(info);
    }

    [Fact]
    public async Task TestUserStats()
    {
        var client = GetClient();

        var user = new User("demo", "demo_password");

        var stats = await client.GetUserStats(user);

        Assert.NotNull(stats);
    }
}
