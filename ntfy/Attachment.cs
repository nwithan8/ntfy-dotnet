using Newtonsoft.Json;

namespace ntfy;

/// <summary>
///     An Attachment that can be included on a <see cref="ReceivedMessage" />.
/// </summary>
public class Attachment
{
    #region JSON Properties

    /// <summary>
    ///     The name of the attachment.
    /// </summary>
    [JsonProperty("name", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The size of the attachment, in bytes.
    ///     Only defined if the attachment was uploaded to the ntfy server.
    /// </summary>
    [JsonProperty("size", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public int? Size { get; set; }

    [JsonProperty("type", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = null!;

    /// <summary>
    ///     The URL of the attachment.
    /// </summary>
    [JsonProperty("url", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; } = null!;

    /// <summary>
    ///     The expiry date of the attachment, as a Unix time stamp.
    ///     Only defined if the attachment was uploaded to the ntfy server.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("expires", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    private long? ExpiresTimestamp { get; set; }

    #endregion

    /// <summary>
    ///     The expiry date of the attachment, as a <see cref="DateTime" /> object.
    ///     Only defined if the attachment was uploaded to the ntfy server.
    /// </summary>
    [JsonIgnore]
    public DateTime? Expires
    {
        get => ExpiresTimestamp != null ? DateTimeOffset.FromUnixTimeSeconds((long)ExpiresTimestamp).DateTime : null;
        set => ExpiresTimestamp = value != null ? (int)((DateTimeOffset)value).ToUnixTimeSeconds() : null;
    }
}
