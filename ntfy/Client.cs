using NetTools;
using NetTools.HTTP;
using RestSharp;

namespace ntfy;

/// <summary>
///     A Client to interact with the ntfy.sh API.
/// </summary>
public class Client
{
    /// <summary>
    ///     The base URL of the ntfy.sh server.
    /// </summary>
    private readonly string _serverUrl;

    /// <summary>
    ///     The user agent to use when making requests to the ntfy.sh API.
    /// </summary>
    private static string UserAgent => $"ntfy-dotnet/{RuntimeInfo.ApplicationInfo.ApplicationVersion}";

    /// <summary>
    ///     Constructs a new <see cref="Client" />.
    /// </summary>
    /// <param name="serverUrl">The base URL of the ntfy.sh server.</param>
    public Client(string? serverUrl = null)
    {
        _serverUrl = serverUrl ?? Constants.DefaultServer;
    }

    /// <summary>
    ///     Check if the provided user is authorized to access the provided topic.
    ///     If not user is provided, check if the provided topic is anonymously accessible.
    /// </summary>
    /// <param name="topic">Topic the user is attempting to access.</param>
    /// <param name="user">Optional specific user attempting to access the topic.</param>
    /// <returns><c>true</c> if the topic is accessible, <c>false</c> otherwise.</returns>
    /// <exception cref="UnexpectedException">An unexpected HTTP status code was encountered during the request.</exception>
    public async Task<bool> CheckAuthentication(string topic, User? user = null)
    {
        var endpoint = Constants.TopicAuth(topic);

        var client = GetClient(15, user); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18

        var response = await client.ExecuteAsync(new RestRequest(endpoint));

        // If no user is provided, checking the ability to anonymously interact with a topic.
        var allowed = false;
        var @switch = new SwitchCase
        {
            { Http.StatusCodeIs2xx(response.StatusCode), () => { allowed = true; } }, // Do nothing if the request was successful
            { user == null && (int)response.StatusCode == 404, () => { allowed = true; } }, // Special case: Anonymous login to old servers return 404 since /<topic>/auth doesn't exist
            { (int)response.StatusCode == 401, () => { allowed = false; } },
            { (int)response.StatusCode == 403, () => { allowed = false; } },
            { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") }
        };
        @switch.MatchFirst(true);

        return allowed;
    }

    /// <summary>
    ///     Get the total storage allowance for attachments, in bytes.
    /// </summary>
    /// <returns>The max amount of bytes of attachments any user can upload to the server.</returns>
    public async Task<long> GetAttachmentAllowance()
    {
        // it's the same for everyone, so user

        var stats = await GetUserStats();

        return stats?.VisitorAttachmentBytesTotal ?? 0;
    }

    /// <summary>
    ///     Get the remaining storage allowance for attachments for a specific user, in bytes.
    /// </summary>
    /// <param name="user">User to check the allowance for.</param>
    /// <returns>The remaining amount of bytes of attachments the specific user can upload to the server.</returns>
    public async Task<long> GetAttachmentAllowanceRemaining(User user)
    {
        var stats = await GetUserStats(user);

        return stats?.VisitorAttachmentBytesRemaining ?? 0;
    }

    /// <summary>
    ///     Get the user storage allowance for attachments for a specific user, in bytes.
    /// </summary>
    /// <param name="user">User to check the allowance for.</param>
    /// <returns>The amount of bytes of attachments the specific user has previously uploaded to the server.</returns>
    public async Task<long> GetAttachmentAllowanceUsed(User user)
    {
        var stats = await GetUserStats(user);

        return stats?.VisitorAttachmentBytesUsed ?? 0;
    }

    /// <summary>
    ///     Get the maximum allowed file size for an attachment, in bytes.
    /// </summary>
    /// <returns>The maximum allowed file size for an attachment, in bytes.</returns>
    public async Task<long> GetAttachmentSizeByteLimit()
    {
        // it's the same for everyone, so user doesn't matter

        var stats = await GetUserStats();

        return stats?.AttachmentFileSizeLimit ?? 0;
    }

    /// <summary>
    ///     Get information about the server.
    /// </summary>
    /// <returns>Information about the server.</returns>
    public async Task<ServerInfo?> GetServerInfo()
    {
        const string endpoint = Constants.ServerInfoEndpoint;

        var client = GetClient(15); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18

        var request = new RestRequest(endpoint);

        var response = await client.ExecuteAsync(request);

        return ServerInfo.FromResponse(response.Content);
    }

    /// <summary>
    ///     Get stats about the provided user.
    ///     Get server-wide stats if no user is provided.
    /// </summary>
    /// <param name="user">Optional user to get stats for.</param>
    /// <returns>Stats about the specific user, or server-wide stats if no user is provided.</returns>
    public async Task<UserStats?> GetUserStats(User? user = null)
    {
        const string endpoint = Constants.UserStatsEndpoint;

        var client = GetClient(15, user); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18

        var request = new RestRequest(endpoint);

        var response = await client.ExecuteAsync<UserStats>(request);

        return response.Data;
    }

    /// <summary>
    ///     Check if the provided attachment size is within the provided user's attachment allowance.
    /// </summary>
    /// <param name="user">User attempting to upload an attachment.</param>
    /// <param name="size">Size of the attempted attachment.</param>
    /// <returns><c>true</c> if the provided user can upload the attachment, <c>false</c> otherwise.</returns>
    public async Task<bool> IsAttachmentOfSizeAllowed(User user, long size)
    {
        var stats = await GetUserStats(user);

        return stats?.VisitorAttachmentBytesRemaining >= size;
    }

    /// <summary>
    ///     Poll the server for new messages.
    /// </summary>
    /// <param name="topics">A list of topics to poll messages for.</param>
    /// <param name="since">Optional since filter to use when polling.</param>
    /// <param name="getScheduledMessages">Whether to get messages scheduled for the future. Defaults to <c>false</c>.</param>
    /// <param name="filters">Optional additional filters to use when polling.</param>
    /// <param name="user">Optional user to use when polling.</param>
    /// <returns>A list of all filtered <see cref="ReceivedMessage" /> objects.</returns>
    public async Task<List<ReceivedMessage>> Poll(IEnumerable<string> topics, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null)
    {
        var endpoint = Constants.TopicReceive(StreamType.Json, topics, since ?? Constants.DefaultSince, getScheduledMessages, filters, true);

        var client = GetClient(15, user); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18

        var response = client.StreamJsonAsync<ReceivedMessage>(endpoint, default);

        var messages = new List<ReceivedMessage>();

        await foreach (var notification in response) messages.Add(notification);

        return messages;
    }

    /// <summary>
    ///     Publish a message to the server.
    /// </summary>
    /// <param name="topic">Topic to publish the message to.</param>
    /// <param name="message"><see cref="SendingMessage" /> message to publish.</param>
    /// <param name="user">Optional user to use when publishing.</param>
    /// <exception cref="UnauthorizedException">Provided user is unauthorized to publish to the topic.</exception>
    /// <exception cref="EntityTooLargeException">Provided <see cref="SendingMessage" /> payload is too large to publish.</exception>
    /// <exception cref="TooManyRequestsException">Server is rate-limiting due to too many requests.</exception>
    /// <exception cref="UnexpectedException">An unexpected HTTP status code was encountered during the request.</exception>
    public async Task Publish(string topic, SendingMessage message, User? user = null)
    {
        var client = GetClient(5 * 60, user); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L24

        // Since we're sending the data as JSON, we don't need to post to a specific topic endpoint.
        var request = new RestRequest("/", Method.Post);

        // Topic will instead be included in the JSON data.
        request.AddBody(message.ToData(topic));

        var response = await client.ExecuteAsync(request);

        var @switch = new SwitchCase
        {
            { 200, () => { } }, // Do nothing if the request was successful
            { 401, () => throw new UnauthorizedException(user) },
            { 403, () => throw new UnauthorizedException(user) },
            { 413, () => throw new EntityTooLargeException() },
            { 429, () => throw new TooManyRequestsException() },
            { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") }
        };
        @switch.MatchFirst(response.StatusCode);
    }

    /// <summary>
    ///     Subscribe to a topic.
    ///     Opens an asynchronous stream to the server and returns an IAsyncEnumerable of <see cref="ReceivedMessage"/> objects.
    ///     New messages will be pushed to the stream as they are received.
    /// </summary>
    /// <param name="topics">List of topics to subscribe to.</param>
    /// <param name="since">Optional since filter to use when polling.</param>
    /// <param name="getScheduledMessages">Whether to get messages scheduled for the future. Defaults to <c>false</c>.</param>
    /// <param name="filters">Optional additional filters to use when polling.</param>
    /// <param name="user">Optional user to use when polling.</param>
    /// <param name="cancellationToken">Optional cancellation token to use to cancel the stream.</param>
    /// <returns>An IAsyncEnumerable of <see cref="ReceivedMessage"/> objects.</returns>
    public async IAsyncEnumerable<ReceivedMessage> Subscribe(IEnumerable<string> topics, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null, CancellationToken? cancellationToken = default)
    {
        var endpoint = Constants.TopicReceive(StreamType.Json, topics, since ?? Constants.DefaultSince, getScheduledMessages, filters);

        var client = GetClient(77, user); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L30

        var response = client.StreamJsonAsync<ReceivedMessage>(endpoint, cancellationToken ?? default);

        if (cancellationToken != null)
            await foreach (var notification in response.WithCancellation(cancellationToken.Value))
                yield return notification;
        else
            await foreach (var notification in response)
                yield return notification;
    }

    /// <summary>
    ///     Subscribe to a topic.
    ///     Opens an asynchronous stream to the server and processes each <see cref="ReceivedMessage" /> object as it is
    ///     received.
    /// </summary>
    /// <param name="topics">List of topics to subscribe to.</param>
    /// <param name="onNotification">Function to execute when a new message is received.</param>
    /// <param name="since">Optional since filter to use when polling.</param>
    /// <param name="getScheduledMessages">Whether to get messages scheduled for the future. Defaults to <c>false</c>.</param>
    /// <param name="filters">Optional additional filters to use when polling.</param>
    /// <param name="user">Optional user to use when polling.</param>
    /// <param name="cancellationToken">Optional cancellation token to use to cancel the stream.</param>
    public async Task SubscribeAndProcess(IEnumerable<string> topics, Func<ReceivedMessage, Task> onNotification, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null, CancellationToken? cancellationToken = default)
    {
        await foreach (var notification in Subscribe(topics, since, getScheduledMessages, filters, user, cancellationToken))
            if (onNotification != null)
                await onNotification(notification);
    }

    /// <summary>
    ///     Get a prepared <see cref="RestSharp.RestClient" /> for the API.
    /// </summary>
    /// <param name="timeout">Maximum timeout for requests made by the client.</param>
    /// <param name="user">Optional user to include authentication for.</param>
    /// <returns>A <see cref="RestSharp.RestClient" /> object.</returns>
    private RestClient GetClient(int timeout, User? user = null)
    {
        var clientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(_serverUrl),
            UserAgent = UserAgent,
            MaxTimeout = timeout * 1000 // turn seconds into milliseconds
        };

        var client = new RestClient(clientOptions);

        if (user != null) client.Authenticator = user.AuthHeader;

        client.UseSerializer(() => new RestSharpSerializer());

        return client;
    }
}
