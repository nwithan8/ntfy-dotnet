namespace ntfy;

internal static class Constants
{
    internal const string DefaultServer = "https://ntfy.sh";

    internal static readonly Since DefaultSince = Since.All;

    internal const string ServerInfoEndpoint = "config.js";

    internal const string UserStatsEndpoint = "user/stats";

    private const string ValidTopicRegex = @"^[-_A-Za-z0-9]{1,64}$";

    internal static string TopicAuth(string topic)
    {
        if (!IsValidTopic(topic))
        {
            throw new InvalidTopicException(topic);
        }
        
        return $"{topic}/auth";
    }

    internal static string TopicReceive(StreamType streamType, IEnumerable<string> topics, Since since, bool getFutureMessages = false, ReceptionFilters? filters = null, bool poll = false)
    {
        var topicString = "";
        foreach (var topic in topics)
        {
            if (!IsValidTopic(topic))
            {
                throw new InvalidTopicException(topic);
            }
            
            topicString += $"{topic},";
        }
        topicString = topicString.TrimEnd(',');
        
        var url = $"{topicString}/{streamType.Endpoint}?since={since.Value}";
        if (getFutureMessages)
            url = AddFutureMessages(url);
        if (filters != null)
            url = AddFilters(url, filters);
        if (poll)
            url = AddPolling(url);
        return url;
    }

    internal static string TopicSend(string topic)
    {
        if (!IsValidTopic(topic))
        {
            throw new InvalidTopicException(topic);
        }
        
        return $"{topic}";
    }

    private static string AddFilters(string url, ReceptionFilters filters)
    {
        return $"{url}&{filters.ToQueryString()}";
    }

    private static string AddFutureMessages(string url)
    {
        return $"{url}&sched=1";
    }

    private static string AddPolling(string url)
    {
        return $"{url}&poll=1";
    }

    private static bool IsValidTopic(string topic)
    {
        return NetTools.RegularExpressions.Matches(topic, ValidTopicRegex);
    }
}
