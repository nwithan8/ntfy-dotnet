using System.Net;
using NetTools;
using NetTools.HTTP;
using ntfy.Filters;
using ntfy.Requests;
using ntfy.Responses;
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

        var request = new RestRequest(endpoint, Method.Get);
        
        var response = await client.ExecuteAsync(request);

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
    /// <param name="user">Optional specific user to check the allowance for.</param>
    /// <returns>The max amount of bytes of attachments the user can upload to the server.</returns>
    public async Task<long> GetAttachmentAllowance(User? user = null)
    {
        var limits = await GetUserLimits(user);

        return limits.AttachmentTotalSize ?? 0;
    }
    
    /// <summary>
    ///     Get the maximum allowed file size for an attachment, in bytes.
    /// </summary>
    /// <param name="user">Optional specific user to check the allowance for.</param>
    /// <returns>The maximum allowed file size for an attachment, in bytes.</returns>
    public async Task<long> GetAttachmentSizeByteLimit(User? user = null)
    {
        var limits = await GetUserLimits(user);
        
        return limits.AttachmentFileSize ?? 0;
    }

    /// <summary>
    ///     Get the remaining storage allowance for attachments, in bytes.
    /// </summary>
    /// <param name="user">Optional specific user to check the allowance for.</param>
    /// <returns>The remaining amount of bytes of attachments the specific user can upload to the server.</returns>
    public async Task<long> GetAttachmentAllowanceRemaining(User? user = null)
    {
        var stats = await GetUserStats(user);

        return stats.AttachmentTotalSizeRemaining ?? 0;
    }

    /// <summary>
    ///     Get the user storage allowance for attachments, in bytes.
    /// </summary>
    /// <param name="user">Optional specific user to check the allowance for.</param>
    /// <returns>The amount of bytes of attachments the specific user has previously uploaded to the server.</returns>
    public async Task<long> GetAttachmentAllowanceUsed(User? user = null)
    {
        var stats = await GetUserStats(user);

        return stats.AttachmentTotalSize ?? 0;
    }

    /// <summary>
    ///     Check if the provided attachment size can be uploaded by the provided user.
    /// </summary>
    /// <param name="user">User attempting to upload an attachment.</param>
    /// <param name="size">Size of the attempted attachment.</param>
    /// <returns><c>true</c> if the provided user can upload the attachment, <c>false</c> otherwise.</returns>
    public async Task<bool> IsAttachmentOfSizeAllowed(User user, long size)
    {
        var userInfo = await GetUserInfo(user);
        
        var maxFileSize = userInfo.Limits.AttachmentFileSize ?? 0;
        
        var allowanceRemaining = userInfo.Stats.AttachmentTotalSizeRemaining ?? 0;

        // File size is less than the max file size and the user has enough allowance remaining
        return size <= maxFileSize && size <= allowanceRemaining;
    }

    /// <summary>
    ///     Get information about the server.
    /// </summary>
    /// <returns>Information about the server.</returns>
    public async Task<ServerInfo> GetServerInfo()
    {
        const string endpoint = Constants.Endpoints.ServerInfo;

        var client = GetClient(15); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18

        var request = new RestRequest(endpoint, Method.Get);

        var response = await client.ExecuteAsync(request);

        return ServerInfo.FromResponse(response.Content);
    }

    /// <summary>
    ///     Get health information about the server.
    /// </summary>
    /// <returns>Health information about the server.</returns>
    public async Task<ServerHealth> GetServerHealthInfo()
    {
        const string endpoint = Constants.Endpoints.ServerHealth;

        var client = GetClient(15); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18

        var request = new RestRequest(endpoint, Method.Get);

        var response = await client.ExecuteAsync<ServerHealth>(request);

        return response.Data;
    }
    

    /// <summary>
    ///     Get information about the provided user.
    ///     Get server-wide information if no user is provided.
    /// </summary>
    /// <param name="user">Optional user to get information for.</param>
    /// <returns>Information about the specific user, or server-wide information if no user is provided.</returns>
    public async Task<UserInfo> GetUserInfo(User? user = null)
    {
        const string endpoint = Constants.Endpoints.UserAccount;

        var client = GetClient(15, user); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18

        var request = new RestRequest(endpoint, Method.Get);

        var response = await client.ExecuteAsync<UserInfo>(request);

        return response.Data;
    }

    /// <summary>
    ///     Get stats about the provided user.
    ///     Get server-wide stats if no user is provided.
    /// </summary>
    /// <param name="user">Optional user to get stats for.</param>
    /// <returns>Stats about the specific user, or server-wide stats if no user is provided.</returns>
    public async Task<UserStats> GetUserStats(User? user = null)
    {
        var userInfo = await GetUserInfo(user);
        
        return userInfo.Stats;
    }
    
    /// <summary>
    ///     Get limits about the provided user.
    ///     Get server-wide limits if no user is provided.
    /// </summary>
    /// <param name="user">Optional user to get limits for.</param>
    /// <returns>Limits about the specific user, or server-wide limits if no user is provided.</returns>
    public async Task<UserLimits> GetUserLimits(User? user = null)
    {
        var userInfo = await GetUserInfo(user);
        
        return userInfo.Limits;
    }

    public async Task<User> SignUp(string username, string password)
    {
        const string endpoint = Constants.Endpoints.UserAccount;
        
        var client = GetClient(15);
        
        var request = new RestRequest(endpoint, Method.Post);
        
        var signUpRequest = new UserSignup(username, password);
        var data = signUpRequest.ToData();
        request.AddBody(data);

        var response = await client.ExecuteAsync(request);
        
        var @switch = new SwitchCase
        {
            { HttpStatusCode.OK, () => { } }, // Do nothing if the request was successful
            { HttpStatusCode.BadRequest, () => throw new FeatureNotEnabledException("Signups") },
            // Can't get 401 or 403 because we're never including a user
            { HttpStatusCode.Conflict, () => throw new UserAlreadyExistsException(username) },
            { HttpStatusCode.TooManyRequests, () => throw new TooManyRequestsException() },
            { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") },
        };
        @switch.MatchFirst(response.StatusCode);

        // If we got here, the request was successful
        return new User(username, password);
    }

    public async Task<bool> ChangeUserPassword(User user, string oldPassword, string newPassword)
    {
        const string endpoint = Constants.Endpoints.UserAccountPassword;
        
        var client = GetClient(15, user);
        
        var request = new RestRequest(endpoint, Method.Post);
        
        var passwordChangeRequest = new UserChangePassword(oldPassword, newPassword);
        var data = passwordChangeRequest.ToData();
        request.AddBody(data);
        
        var response = await client.ExecuteAsync(request);
        
        var @switch = new SwitchCase
        {
            { HttpStatusCode.OK, () => { } }, // Do nothing if the request was successful
            // Can't get 401 or 403 because we're never not including a user
            { HttpStatusCode.BadRequest, () => throw new InvalidCredentialsException() },
            { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") },
        };
        @switch.MatchFirst(response.StatusCode);

        // If we got here, the request was successful
        return true;
    }

    public async Task<UserTokenDetails> GenerateUserToken(User user)
    {
        const string endpoint = Constants.Endpoints.UserAccountToken;
        
        var client = GetClient(15, user);
        
        var request = new RestRequest(endpoint, Method.Post);
        
        var response = await client.ExecuteAsync<UserTokenDetails>(request);

        return response.Data;
    }

    public async Task<bool> ExtendUserToken(User user)
    {
        const string endpoint = Constants.Endpoints.UserAccountToken;
        
        var client = GetClient(15, user);
        
        var request = new RestRequest(endpoint, Method.Patch);
        
        var response = await client.ExecuteAsync(request);
        
        var @switch = new SwitchCase
        {
            { HttpStatusCode.OK, () => { } }, // Do nothing if the request was successful
            // Can't get 401 or 403 because we're never not including a user
            { HttpStatusCode.BadRequest, () => throw new InvalidCredentialsException() },
            { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") },
        };
        
        @switch.MatchFirst(response.StatusCode);
        
        // If we got here, the request was successful
        return true;
    }

    public async Task<bool> DeleteUserToken(string token)
    {
        const string endpoint = Constants.Endpoints.UserAccountToken;

        // Request requires the bearer token to be deleted to be used as the auth method
        var user = new User(token);
        
        var client = GetClient(15, user);
        
        var request = new RestRequest(endpoint, Method.Delete);
        
        var response = await client.ExecuteAsync(request);
        
        var @switch = new SwitchCase
        {
            { HttpStatusCode.OK, () => { } }, // Do nothing if the request was successful
            { HttpStatusCode.Unauthorized, () => throw new InvalidCredentialsException() },
            { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") },
        };
        
        @switch.MatchFirst(response.StatusCode);
        
        // If we got here, the request was successful
        return true;
    }

    public async Task<bool> ReserveTopic(User user, string topic, Permission permissionForOthers)
    {
        const string endpoint = Constants.Endpoints.UserAccountReservations;
        
        var client = GetClient(15, user);
        
        var request = new RestRequest(endpoint, Method.Post);
        
        var reservationRequest = new TopicReservation(topic, permissionForOthers);
        var data = reservationRequest.ToData();
        request.AddBody(data);

        var response = await client.ExecuteAsync<ApiSuccess>(request);

        return response.Data.Success;
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
        var data = message.ToData(topic);
        request.AddBody(data);

        var response = await client.ExecuteAsync(request);

        var @switch = new SwitchCase
        {
            { HttpStatusCode.OK, () => { } }, // Do nothing if the request was successful
            { HttpStatusCode.Unauthorized, () => throw new UnauthorizedException(user) },
            { HttpStatusCode.Forbidden, () => throw new UnauthorizedException(user) },
            { HttpStatusCode.RequestEntityTooLarge, () => throw new EntityTooLargeException() },
            { HttpStatusCode.TooManyRequests, () => throw new TooManyRequestsException() },
            { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") },
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
    /// <param name="ignoreKeepAlive">Ignore keepalive messages. Defaults to <c>true</c>.</param>
    /// <returns>An IAsyncEnumerable of <see cref="ReceivedMessage"/> objects.</returns>
    public async IAsyncEnumerable<ReceivedMessage> Subscribe(IEnumerable<string> topics, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null, CancellationToken? cancellationToken = default, bool ignoreKeepAlive = true)
    {
        var endpoint = Constants.TopicReceive(StreamType.Json, topics, since ?? Constants.DefaultSince, getScheduledMessages, filters);

        var client = GetClient(77, user); // https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L30

        var response = client.StreamJsonAsync<ReceivedMessage>(endpoint, cancellationToken ?? default);

        if (cancellationToken != null)
            await foreach (var notification in response.WithCancellation(cancellationToken.Value))
                if (ignoreKeepAlive && notification.Event == EventType.KeepAlive)
                    continue;
                else
                    yield return notification;
        else
            await foreach (var notification in response)
                if (ignoreKeepAlive && notification.Event == EventType.KeepAlive)
                    continue;
                else
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
    /// <param name="ignoreKeepAlive">Ignore (do not process) keepalive messages. Defaults to <c>true</c>.</param>
    public async Task SubscribeAndProcess(IEnumerable<string> topics, Func<ReceivedMessage, Task> onNotification, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null, CancellationToken? cancellationToken = default, bool ignoreKeepAlive = true)
    {
        await foreach (var notification in Subscribe(topics, since, getScheduledMessages, filters, user, cancellationToken, ignoreKeepAlive))
            if (onNotification != null)
                await onNotification(notification);
    }

    /// <summary>
    ///     Get a prepared <see cref="RestSharp.RestClient" /> for the API.
    /// </summary>
    /// <param name="timeout">Maximum timeout for requests made by the client.</param>
    /// <param name="user">Optional user to include authentication for.</param>
    /// <returns>A <see cref="RestSharp.RestClient" /> object.</returns>
    private RestClient GetClient(int timeout = 15, User? user = null)
    {
        // 15 second timeout: https://github.com/binwiederhier/ntfy-android/blob/6333a063a13d7a01797ea40ccd1031bcd3025045/app/src/main/java/io/heckel/ntfy/msg/ApiService.kt#L18
        
        var clientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(_serverUrl),
            UserAgent = UserAgent,
            MaxTimeout = timeout * 1000, // turn seconds into milliseconds
        };

        var client = new RestClient(clientOptions);

        if (user != null) client.AddDefaultHeader("Authorization", user.AuthHeaderValue);

        client.UseSerializer(() => new RestSharpSerializer());

        return client;
    }
}
