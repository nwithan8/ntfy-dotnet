using NetTools;

namespace ntfy;

/// <summary>
///     Constants for the ntfy library.
/// </summary>
internal static class Constants
{
    /// <summary>
    ///     Default ntfy server.
    /// </summary>
    internal const string DefaultServer = "https://ntfy.sh";

    /// <summary>
    ///     Endpoint for the ntfy server information.
    /// </summary>
    internal const string ServerInfoEndpoint = "config.js";

    /// <summary>
    ///     Endpoint for user stats information.
    /// </summary>
    internal const string UserStatsEndpoint = "user/stats";

    /// <summary>
    ///     Regex pattern for a valid topic.
    /// </summary>
    private const string ValidTopicRegex = @"^[-_A-Za-z0-9]{1,64}$";

    /// <summary>
    ///     Default since filter.
    /// </summary>
    internal static readonly Since DefaultSince = Since.All;

    /// <summary>
    ///     Get a prepared topic authentication endpoint.
    /// </summary>
    /// <param name="topic">Topic to check authentication for.</param>
    /// <returns>A topic authentication endpoint string.</returns>
    /// <exception cref="InvalidTopicException">The provided topic is invalid.</exception>
    internal static string TopicAuth(string topic)
    {
        if (!IsValidTopic(topic)) throw new InvalidTopicException(topic);

        return $"{topic}/auth";
    }

    /// <summary>
    ///     Get a prepared topic message reception endpoint.
    /// </summary>
    /// <param name="streamType">Type of streaming protocol to use for the subscription.</param>
    /// <param name="topics">List of topics to subscribe to.</param>
    /// <param name="since">Optional since filter to use when polling.</param>
    /// <param name="getFutureMessages">Whether to get messages scheduled for the future. Defaults to <c>false</c>.</param>
    /// <param name="filters">Optional additional filters to use when polling.</param>
    /// <param name="poll">Whether this is a poll request.</param>
    /// <returns>A topic message reception endpoint string.</returns>
    /// <exception cref="InvalidTopicException">One or more of the provided topics are invalid.</exception>
    internal static string TopicReceive(StreamType streamType, IEnumerable<string> topics, Since since, bool getFutureMessages = false, ReceptionFilters? filters = null, bool poll = false)
    {
        var topicString = "";
        foreach (var topic in topics)
        {
            if (!IsValidTopic(topic)) throw new InvalidTopicException(topic);

            topicString += $"{topic},";
        }

        topicString = topicString.TrimEnd(',');

        var url = $"{topicString}/{streamType.Value}?since={since.Value}";
        if (getFutureMessages)
            url = AddFutureMessages(url);
        if (filters != null)
            url = AddFilters(url, filters);
        if (poll)
            url = AddPolling(url);
        return url;
    }

    /// <summary>
    ///     Get a prepared topic message sending endpoint.
    /// </summary>
    /// <param name="topic">Topic to send a message to.</param>
    /// <returns>A prepared topic send endpoint string.</returns>
    /// <exception cref="InvalidTopicException">The provided topic is invalid.</exception>
    internal static string TopicSend(string topic)
    {
        if (!IsValidTopic(topic)) throw new InvalidTopicException(topic);

        return $"{topic}";
    }

    /// <summary>
    ///     Add filters to the endpoint.
    /// </summary>
    /// <param name="url">URL to append to.</param>
    /// <param name="filters">Filters to append.</param>
    /// <returns>The URL with appended filter query parameters.</returns>
    private static string AddFilters(string url, ReceptionFilters filters)
    {
        return $"{url}&{filters.ToQueryString()}";
    }

    /// <summary>
    ///     Add query parameters to the endpoint to retrieve future messages.
    /// </summary>
    /// <param name="url">URL to append to.</param>
    /// <returns>The URL with appended future messages query parameters.</returns>
    private static string AddFutureMessages(string url)
    {
        return $"{url}&sched=1";
    }

    /// <summary>
    ///     Add query parameters to the endpoint to poll for messages.
    /// </summary>
    /// <param name="url">URL to append to.</param>
    /// <returns>The URL with appended polling query parameters.</returns>
    private static string AddPolling(string url)
    {
        return $"{url}&poll=1";
    }

    /// <summary>
    ///     Check if the provided topic is valid.
    /// </summary>
    /// <param name="topic">Topic to validate.</param>
    /// <returns><c>true</c> if the provided topic is valid, <c>false</c> otherwise.</returns>
    private static bool IsValidTopic(string topic)
    {
        return RegularExpressions.Matches(topic, ValidTopicRegex);
    }
}
