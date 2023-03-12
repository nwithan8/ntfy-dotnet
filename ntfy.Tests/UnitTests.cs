using ntfy.Actions;
using ntfy.Filters;
using ntfy.Requests;
using Xunit.Abstractions;

namespace ntfy.Tests;

public class UnitTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    private static string[] GetTopics() => new string[] { "csharp-test" };
    
    private static Client GetPublicClient() => new Client("https://ntfy.sh");

    private static Client GetPrivateClient() => new Client("https://ntfy.nateharr.is");

    private static User GetPrivateUser() => new User("demo", "demo_password");

    [Fact]
    public async Task TestPublish()
    {
        var client = GetPublicClient();

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
                },
            },
        };

        foreach (var action in message.Actions)
        {
            var type = action.Type;
        }

        await client.Publish(GetTopics()[0], message);
    }

    [Fact]
    public async Task TestSubscribe()
    {
        var client = GetPublicClient();

        var filter = new ReceptionFilters
        {
            Priorities = new PriorityLevel[]
            {
                PriorityLevel.Low,
                PriorityLevel.Default,
            },
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
        var client = GetPublicClient();
        
        await client.SubscribeAndProcess(GetTopics(), (message) =>
        {
            _testOutputHelper.WriteLine(message.Title ?? message.Id);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task TestPoll()
    {
        var client = GetPublicClient();
        
        var filter = new ReceptionFilters
        {
            Priorities = new PriorityLevel[]
            {
                PriorityLevel.Low,
                PriorityLevel.Default,
            },
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
        var client = GetPrivateClient();
        
        var allowed = await client.CheckAuthentication(GetTopics()[0]);
        
        // Anonymous users are not allowed to access this topic.
        Assert.False(allowed);
        
        var user = GetPrivateUser();

        allowed = await client.CheckAuthentication(GetTopics()[0], user);
        
        // The demo user is allowed to access this topic.
        Assert.True(allowed);
    }

    [Fact]
    public async Task TestServerInfo()
    {
        var client = GetPublicClient();

        var info = await client.GetServerInfo();

        Assert.NotNull(info);
        Assert.NotNull(info!.DisallowedTopics);
    }
    
    [Fact]
    public async Task TestServerHealthInfo()
    {
        var client = GetPublicClient();

        var info = await client.GetServerHealthInfo();

        Assert.NotNull(info);
    }
    
    [Fact]
    public async Task TestUserInfo()
    {
        var client = GetPrivateClient();
        
        var user = GetPrivateUser();

        var info = await client.GetUserInfo(user);

        Assert.NotNull(info);
    }

    [Fact]
    public async Task TestUserStats()
    {
        var client = GetPrivateClient();

        var user = GetPrivateUser();

        var stats = await client.GetUserStats(user);

        Assert.NotNull(stats);
    }

    [Fact]
    public async Task TestUserLimits()
    {
        var client = GetPrivateClient();

        var user = GetPrivateUser();

        var limits = await client.GetUserLimits(user);

        Assert.NotNull(limits);
    }
    
    [Fact]
    public async Task TestUserInfoNoUser()
    {
        var client = GetPrivateClient();

        var info = await client.GetUserInfo();

        Assert.NotNull(info);
    }

    [Fact]
    public async Task TestUserStatsNoUser()
    {
        var client = GetPrivateClient();
        
        var stats = await client.GetUserStats();

        Assert.NotNull(stats);
    }

    [Fact]
    public async Task TestUserLimitsNoUser()
    {
        var client = GetPrivateClient();

        var limits = await client.GetUserLimits();

        Assert.NotNull(limits);
    }

    [Fact]
    public async Task TestUserSignUp()
    {
        var client = GetPrivateClient();
        
        var newUser = await client.SignUp("test_username", "test_password");
        
        Assert.Null(newUser);
    }

    [Fact]
    public async Task TestUserChangePassword()
    {
        var client = GetPrivateClient();
        
        var user = GetPrivateUser();
        
        // These passwords are invalid on purpose.
        var success = await client.ChangeUserPassword(user, "new_password", "demo_password");
        
        Assert.False(success);
    }

    [Fact]
    public async Task TestGenerateUserToken()
    {
        var client = GetPrivateClient();
        
        var user = GetPrivateUser();
        
        var details = await client.GenerateUserToken(user);
        
        Assert.NotNull(details);
    }

    [Fact]
    public async Task TestExtendUserToken()
    {
        var client = GetPrivateClient();
        
        var user = GetPrivateUser();
        
        var success = await client.ExtendUserToken(user);
    }

    [Fact]
    public async Task TestDeleteUserToken()
    {
        var client = GetPrivateClient();
        
        var user = GetPrivateUser();
        
        var details = await client.GenerateUserToken(user);
        
        var success = await client.DeleteUserToken(details.Token!);
        
        Assert.True(success);
    }

    [Fact]
    public async Task TestTopicReservation()
    {
        var client = GetPrivateClient();
        
        var user = GetPrivateUser();
        
        var topic = GetTopics()[0];
        
        var reserved = await client.ReserveTopic(user, topic, Permission.ReadWrite);
        
        Assert.False(reserved);
    }

    [Fact]
    public void TestRandomTopic()
    {
        var topic = Utilities.GenerateRandomTopic();

        Assert.NotNull(topic);
        
        // Correct length
        Assert.True(topic.Length >= 16);
        Assert.True(topic.Length <= 64);
        
        // No special characters (could have capital letters)
        Assert.True(topic.ToLowerInvariant().All(c => "abcdefghijklmnopqrstuvwxyz0123456789".Contains(c)));
    }
    
    [Fact]
    public void TestRandomTopicWithWords()
    {
        var topic = Utilities.GenerateRandomTopic(true);

        Assert.NotNull(topic);
        
        // Correct length
        Assert.True(topic.Length >= 16);
        Assert.True(topic.Length <= 64);
    }
}
