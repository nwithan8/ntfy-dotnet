namespace ntfy;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
///     A selection of filters that can be used to filter received messages.
/// </summary>
public class ReceptionFilters
{
    /// <summary>
    ///     Only return messages that match this exact message ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    ///     Only return messages that match this exact message string.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///     Only return messages that match any priority listed.
    /// </summary>
    public List<PriorityLevel>? Priorities { get; set; }

    /// <summary>
    ///     Only return messages that match all listed tags.
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    ///     Only return messages that match this exact title string.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     Convert this filter collection to a query string.
    /// </summary>
    /// <returns></returns>
    internal string ToQueryString()
    {
        var queryElements = new Dictionary<string, string>();
        if (Id != null)
            queryElements.Add("id", Id);
        if (Message != null)
            queryElements.Add("message", Message);
        if (Title != null)
            queryElements.Add("title", Title);
        if (Priorities != null)
            queryElements.Add("priority", Priorities.Aggregate("", (current, priority) => current + priority.Word + ","));
        if (Tags != null)
            queryElements.Add("tags", string.Join(",", Tags));
        return queryElements.Aggregate("", (current, queryElement) => current + queryElement.Key + "=" + queryElement.Value + "&");
    }
}
