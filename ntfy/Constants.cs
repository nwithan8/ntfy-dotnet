using System.Net;
using NetTools;
using ntfy.Filters;

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

    internal static class Endpoints
    {
        /// <summary>
        ///     Endpoint for the ntfy server information.
        /// </summary>
        internal const string ServerInfo = "config.js";

        /// <summary>
        ///     Endpoint for server health information.
        /// </summary>
        internal const string ServerHealth = "v1/health";
        
        /// <summary>
        ///     Endpoint for server tiers information.
        /// </summary>
        internal const string Tiers = "v1/tiers";
        
        /// <summary>
        ///     Endpoint for user account information.
        /// </summary>
        internal const string UserAccount = "v1/account";
        
        /// <summary>
        ///     Endpoint for user account token information.
        /// </summary>
        internal const string UserAccountToken = "v1/account/token";
        
        /// <summary>
        ///     Endpoint for user account password information.
        /// </summary>
        internal const string UserAccountPassword = "v1/account/password";
        
        /// <summary>
        ///     Endpoint for user account settings information.
        /// </summary>
        internal const string UserAccountSettings = "v1/account/settings";
        
        /// <summary>
        ///     Endpoint for user account subscription information.
        /// </summary>
        internal const string UserAccountSubscription = "v1/account/subscription";
        
        /// <summary>
        ///     Endpoint for user account reservations information.
        /// </summary>
        internal const string UserAccountReservations = "v1/account/reservation";
        
        /// <summary>
        ///     Endpoint for user account billing portal.
        /// </summary>
        internal const string UserAccountBillingPortal = "v1/account/billing/portal";
        
        /// <summary>
        ///     Endpoint for user account billing webhook information.
        /// </summary>
        internal const string UserAccountBillingWebhook = "v1/account/billing/webhook";
        
        /// <summary>
        ///     Endpoint for user account billing subscription information.
        /// </summary>
        internal const string UserAccountBillingSubscription = "v1/account/billing/subscription";

        /// <summary>
        ///     Endpoint for user account billing subscription success.
        /// </summary>
        internal static string UserAccountBillingSubscriptionSuccess(string checkoutSessionId) => $"v1/account/billing/subscription/success/{checkoutSessionId}";
    } 
}
