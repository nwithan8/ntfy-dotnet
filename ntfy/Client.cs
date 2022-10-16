using NetTools;
using RestSharp;
using RestSharp.Authenticators;

namespace ntfy
{
    public class Client
    {
        private readonly string _serverUrl;

        private static string UserAgent => $"ntfy-dotnet/{NetTools.RuntimeInfo.ApplicationInfo.ApplicationVersion}";

        public Client(string? serverUrl = null)
        {
            _serverUrl = serverUrl ?? Constants.DefaultServer;
        }

        private RestClient GetClient(User? user = null)
        {
            var clientOptions = new RestClientOptions
            {
                BaseUrl = new Uri(_serverUrl),
                UserAgent = UserAgent
            };

            var client = new RestClient(clientOptions);
            if (user != null)
            {
                client.Authenticator = new HttpBasicAuthenticator(user.Username, user.Password);
            }

            client.UseSerializer(() => new NetTools.HTTP.RestSharpSerializer());

            return client;
        }

        public async Task Publish(string topic, SendingMessage message, User? user = null)
        {
            var client = GetClient(user);

            // Since we're sending the data as JSON, we don't need to post to a specific topic endpoint.
            var request = new RestRequest("/", Method.Post);

            // Topic will instead be included in the JSON data.
            request.AddBody(message.ToData(topic));

            var response = await client.ExecuteAsync(request);

            var @switch = new NetTools.SwitchCase
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

        public async IAsyncEnumerable<ReceivedMessage> Subscribe(IEnumerable<string> topics, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null, CancellationToken? cancellationToken = default)
        {
            var endpoint = Constants.TopicReceive(StreamType.Json, topics, since ?? Constants.DefaultSince, getScheduledMessages, filters);

            var client = GetClient(user);

            var response = client.StreamJsonAsync<ReceivedMessage>(endpoint, cancellationToken ?? default);

            if (cancellationToken != null)
            {
                await foreach (var notification in response.WithCancellation(cancellationToken.Value))
                {
                    yield return notification;
                }
            }
            else
            {
                await foreach (var notification in response)
                {
                    yield return notification;
                }
            }
        }

        public async Task SubscribeAndProcess(IEnumerable<string> topics, Func<ReceivedMessage, Task> onNotification, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null, CancellationToken? cancellationToken = default)
        {
            await foreach (var notification in Subscribe(topics, since, getScheduledMessages, filters, user, cancellationToken))
            {
                if (onNotification != null)
                {
                    await onNotification(notification);
                }
            }
        }

        public async Task<List<ReceivedMessage>> Poll(IEnumerable<string> topics, Since? since = null, bool getScheduledMessages = false, ReceptionFilters? filters = null, User? user = null)
        {
            var endpoint = Constants.TopicReceive(StreamType.Json, topics, since ?? Constants.DefaultSince, getScheduledMessages, filters, true);

            var client = GetClient(user);

            var response = client.StreamJsonAsync<ReceivedMessage>(endpoint, default);

            var messages = new List<ReceivedMessage>();

            await foreach (var notification in response)
            {
                messages.Add(notification);
            }

            return messages;
        }

        public async Task<bool> CheckAuthentication(string topic, User? user = null)
        {
            var endpoint = Constants.TopicAuth(topic);

            var client = GetClient(user);

            var response = await client.ExecuteAsync(new RestRequest(endpoint, Method.Get));

            // If no user is provided, checking the ability to anonymously interact with a topic.
            var allowed = false;
            var @switch = new NetTools.SwitchCase
            {
                { NetTools.HTTP.Http.StatusCodeIs2xx(response.StatusCode), () => { allowed = true; } }, // Do nothing if the request was successful
                { user == null && (int)response.StatusCode == 404, () => { allowed = true; } }, // Special case: Anonymous login to old servers return 404 since /<topic>/auth doesn't exist
                { (int)response.StatusCode == 401, () => { allowed = false; } },
                { (int)response.StatusCode == 403, () => { allowed = false; } },
                { Scenario.Default, () => throw new UnexpectedException($"Unexpected status code {response.StatusCode}") }
            };
            @switch.MatchFirst(true);

            return allowed;
        }
    }
}
