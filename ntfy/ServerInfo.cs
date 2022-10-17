using NetTools.HTTP;
using Newtonsoft.Json;

namespace ntfy;

/// <summary>
///     Information about the ntfy server.
/// </summary>
public class ServerInfo
{
    #region JSON Properties

    /// <summary>
    ///     The server's root path.
    /// </summary>
    [JsonProperty("appRoot")]
    public string? AppRoot { get; internal set; }

    /// <summary>
    ///     A list of disallowed topics on the server.
    /// </summary>
    [JsonProperty("disallowedTopics")]
    public string[]? DisallowedTopics { get; internal set; }

    #endregion

    /// <summary>
    ///     Construct a new <see cref="ServerInfo" /> instance from the JavaScript response from the info endpoint.
    /// </summary>
    /// <param name="responseJavaScript">JavaScript code returned by the server's info endpoint.</param>
    /// <returns>A <see cref="ServerInfo" /> instance.</returns>
    internal static ServerInfo? FromResponse(string? responseJavaScript)
    {
        if (string.IsNullOrWhiteSpace(responseJavaScript)) return null;

        /* example response:
            // Generated server configuration
            var config = {
              appRoot: "/",
              disallowedTopics: ["docs", "static", "file", "app", "settings"]
            };
         */
        // remove non-JSON elements from string
        responseJavaScript = responseJavaScript.Replace("// Generated server configuration", "");
        responseJavaScript = responseJavaScript.Replace("var config = ", "");
        responseJavaScript = responseJavaScript.Replace(";", "");

        // clean up JSON string
        var json = responseJavaScript.Replace("\n", "").Replace("\r", "");
        json = json.Replace("  appRoot", "\"appRoot\"");
        json = json.Replace("  disallowedTopics", "\"disallowedTopics\"");
        json = json.Replace(" ", "");

        // deserialize JSON
        var serverInfo = JsonSerialization.ConvertJsonToObject<ServerInfo>(json);
        return serverInfo;
    }
}

/// <summary>
///     Information about a ntfy user.
/// </summary>
public class UserStats
{
    #region JSON Properties

    /// <summary>
    ///     The maximum file size, in bytes, allowed for message attachments.
    ///     This is user-agnostic, and is the same for all users.
    /// </summary>
    [JsonProperty("attachmentFileSizeLimit")]
    public long AttachmentFileSizeLimit { get; internal set; }

    /// <summary>
    ///     The remaining bytes of storage space available for attachments for this user.
    /// </summary>
    [JsonProperty("visitorAttachmentBytesRemaining")]
    public long VisitorAttachmentBytesRemaining { get; internal set; }

    /// <summary>
    ///     The total bytes of storage space available for attachments for this user.
    ///     This is user-agnostic, and is the same for all users.
    /// </summary>
    [JsonProperty("visitorAttachmentBytesTotal")]
    public long VisitorAttachmentBytesTotal { get; internal set; }

    /// <summary>
    ///     The remaining bytes of storage space available for attachments for this user.
    /// </summary>
    [JsonProperty("visitorAttachmentBytesUsed")]
    public long VisitorAttachmentBytesUsed { get; internal set; }

    #endregion
}
