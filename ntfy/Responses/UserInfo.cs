using Newtonsoft.Json;

namespace ntfy.Responses;

/// <summary>
///     Information about a ntfy user.
/// </summary>
public class UserInfo
{
    #region JSON Properties

    [JsonProperty("username")]
    public string? Username { get; internal set; }

    [JsonProperty("role")]
    public string? Role { get; internal set; }

    [JsonProperty("sync_topic")]
    public string? SyncTopic { get; internal set; }

    [JsonProperty("limits")]
    public UserLimits? Limits { get; internal set; }

    [JsonProperty("stats")]
    public UserStats? Stats { get; internal set; }

    #endregion
}

/// <summary>
///     Limits for a ntfy user.
/// </summary>
public class UserLimits
{
    #region JSON Properties

    [JsonProperty("basic")]
    public string? Basic { get; internal set; }

    [JsonProperty("messages")]
    public long? Messages { get; internal set; }

    [JsonProperty("messages_expiry_duration")]
    public long? MessagesExpiryDuration { get; internal set; }

    [JsonProperty("emails")]
    public long? Emails { get; internal set; }

    [JsonProperty("reservations")]
    public long? Reservations { get; internal set; }

    [JsonProperty("attachment_total_size")]
    public long? AttachmentTotalSize { get; internal set; }

    [JsonProperty("attachment_file_size")]
    public long? AttachmentFileSize { get; internal set; }

    [JsonProperty("attachment_expiry_duration")]
    public long? AttachmentExpiryDuration { get; internal set; }

    [JsonProperty("attachment_bandwidth")]
    public long? AttachmentBandwidth { get; internal set; }

    #endregion
}

/// <summary>
///     Statistics about a ntfy user.
/// </summary>
public class UserStats
{
    #region JSON Properties

    [JsonProperty("messages")]
    public long? Messages { get; internal set; }

    [JsonProperty("messages_remaining")]
    public long? MessagesRemaining { get; internal set; }

    [JsonProperty("emails")]
    public long? Emails { get; internal set; }

    [JsonProperty("emails_remaining")]
    public long? EmailsRemaining { get; internal set; }

    [JsonProperty("reservations")]
    public long? Reservations { get; internal set; }

    [JsonProperty("reservations_remaining")]
    public long? ReservationsRemaining { get; internal set; }

    [JsonProperty("attachment_total_size")]
    public long? AttachmentTotalSize { get; internal set; }

    [JsonProperty("attachment_total_size_remaining")]
    public long? AttachmentTotalSizeRemaining { get; internal set; }

    #endregion
}