namespace ntfy;

// ReSharper disable once ClassNeverInstantiated.Global
public class ReceptionFilters
{
    public string? Id { get; set; }

    public string? Message { get; set; }

    public List<PriorityLevel>? Priorities { get; set; }

    public string[]? Tags { get; set; }

    public string? Title { get; set; }

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
