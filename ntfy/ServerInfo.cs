using NetTools.HTTP;
using Newtonsoft.Json;

namespace ntfy;

public class ServerInfo
{
    #region JSON Properties

    [JsonProperty("appRoot")]
    public string? AppRoot { get; internal set; }

    [JsonProperty("disallowedTopics")]
    public string[]? DisallowedTopics { get; internal set; }

    #endregion

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

public class UserStats
{
    #region JSON Properties

    [JsonProperty("attachmentFileSizeLimit")]
    public long AttachmentFileSizeLimit { get; internal set; }

    [JsonProperty("visitorAttachmentBytesRemaining")]
    public long VisitorAttachmentBytesRemaining { get; internal set; }

    [JsonProperty("visitorAttachmentBytesTotal")]
    public long VisitorAttachmentBytesTotal { get; internal set; }

    [JsonProperty("visitorAttachmentBytesUsed")]
    public long VisitorAttachmentBytesUsed { get; internal set; }

    #endregion
}
