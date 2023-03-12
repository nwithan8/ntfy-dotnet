using Newtonsoft.Json;

namespace ntfy.Responses;

public class UserTokenDetails
{
    [JsonProperty("token")]
    public string? Token { get; set; }
    
    [JsonProperty("last_access")]
    private string? LastAccessTimestamp { get;}

    [JsonIgnore]
    public DateTime LastAccess => LastAccessTimestamp == null ? DateTime.MinValue : DateTime.Parse(LastAccessTimestamp);

    [JsonProperty("last_origin")]
    public string? LastOrigin { get; set; }
    
    [JsonProperty("expires")]
    private string? ExpiresTimestamp { get; }
    
    [JsonIgnore]
    public DateTime Expires => ExpiresTimestamp == null ? DateTime.MinValue : DateTime.Parse(ExpiresTimestamp);
}
