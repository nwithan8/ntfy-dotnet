using Newtonsoft.Json;

namespace ntfy.Responses;

/// <summary>
///     Information about the ntfy server.
/// </summary>
public class ServerInfo
{
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
        responseJavaScript = responseJavaScript!.Replace("// Generated server configuration", "");
        responseJavaScript = responseJavaScript!.Replace("var config = ", "");
        responseJavaScript = responseJavaScript!.Replace(";", "");

        // clean up JSON string
        var json = responseJavaScript!.Replace("\n", "").Replace("\r", "");
        json = json.Replace("  appRoot", "\"appRoot\"");
        json = json.Replace("  disallowedTopics", "\"disallowedTopics\"");
        json = json.Replace(" ", "");

        // deserialize JSON
        var serverInfo = JsonConvert.DeserializeObject<ServerInfo>(json);
        return serverInfo;
    }

    #region JSON Properties

    /// <summary>
    ///     The server's base URL.
    /// </summary>
    [JsonProperty("base_url")]
    public string? BaseUrl { get; internal set; }

    /// <summary>
    ///     The server's root path.
    /// </summary>
    [JsonProperty("app_root")]
    public string? AppRoot { get; internal set; }

    /// <summary>
    ///     Whether the server allows login.
    /// </summary>
    [JsonProperty("enable_login")]
    public bool LoginEnabled { get; internal set; }

    /// <summary>
    ///     Whether the server allows signup.
    /// </summary>
    [JsonProperty("enable_signup")]
    public bool SignupEnabled { get; internal set; }

    /// <summary>
    ///     Whether the server allows payments.
    /// </summary>
    [JsonProperty("enable_payments")]
    public bool PaymentsEnabled { get; internal set; }

    /// <summary>
    ///     Whether the server allows topic reservations.
    /// </summary>
    [JsonProperty("enable_reservations")]
    public bool TopicReservationsEnabled { get; internal set; }

    /// <summary>
    ///     A list of disallowed topics on the server.
    /// </summary>
    [JsonProperty("disallowed_topics")]
    public string[]? DisallowedTopics { get; internal set; }

    #endregion
}