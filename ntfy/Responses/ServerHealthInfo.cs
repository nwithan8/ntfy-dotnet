using Newtonsoft.Json;

namespace ntfy.Responses;

/// <summary>
///     Health information about the ntfy server.
/// </summary>
public class ServerHealth
{
    #region JSON Properties

    /// <summary>
    ///     Whether the server is healthy.
    /// </summary>
    [JsonProperty("healthy")]
    public bool Healthy { get; internal set; }

    #endregion
}
