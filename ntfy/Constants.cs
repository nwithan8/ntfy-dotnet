namespace ntfy;

internal static class Constants
{
    internal const string DefaultServer = "https://ntfy.sh";

    internal static readonly Since DefaultSince = Since.All;

    internal static string TopicSend(string topic) => $"{topic}";

    private static string AddFutureMessages(string url) => $"{url}&sched=1";

    private static string AddPolling(string url) => $"{url}&poll=1";

    private static string AddFilters(string url, ReceptionFilters filters) => $"{url}&{filters.ToQueryString()}";

    internal static string TopicReceive(StreamType streamType, IEnumerable<string> topics, Since since, bool getFutureMessages = false, ReceptionFilters? filters = null, bool poll = false)
    {
        var topicString = string.Join(",", topics);
        var url = $"{topicString}/{streamType.Endpoint}?since={since.Value}";
        if (getFutureMessages)
            url = AddFutureMessages(url);
        if (filters != null)
            url = AddFilters(url, filters);
        if (poll)
            url = AddPolling(url);
        return url;
    }

    internal static string TopicAuth(string topic) => $"{topic}/auth";
}
