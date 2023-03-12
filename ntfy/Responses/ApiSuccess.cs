using Newtonsoft.Json;

namespace ntfy.Responses;

public class ApiSuccess
{
    [JsonProperty("success")]
    public bool Success { get; set; }
}
