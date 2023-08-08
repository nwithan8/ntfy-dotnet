using Newtonsoft.Json;
using ntfy.Requests;
using ntfy.Responses;

namespace ntfy.Actions;

/// <summary>
///     An Http action that can be included on a <see cref="ReceivedMessage" /> or <see cref="SendingMessage" />.
///     Sends a HTTP request when the action button is tapped.
/// </summary>
public class Http : Action
{
    /// <summary>
    ///     Constructor for an Http action.
    /// </summary>
    /// <param name="label">The label for the action.</param>
    /// <param name="url">The URL to which the HTTP request will be sent.</param>
    public Http(string label, Uri url) : base(label)
    {
        Url = url;
    }

    /// <summary>
    ///     The type of this action, as an <see cref="ActionType" /> enum.
    /// </summary>
    [JsonIgnore]
    public override ActionType Type { get; } = ActionType.Http;

    /// <summary>
    ///     The HTTP method to use when sending the request.
    ///     Defaults to <c>HttpMethod.Post</c>
    /// </summary>
    [JsonIgnore]
    public HttpMethod? Method
    {
        get => new(MethodString!);
        set => MethodString = value?.Method;
    }

    #region JSON Properties

    /// <summary>
    ///     Optional body to include in the Http request.
    /// </summary>
    [JsonProperty("body", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Body { get; set; } = string.Empty;

    /// <summary>
    ///     Whether to clear the associated notification after the HTTP request succeeds.
    ///     If the request fails, the notification is not cleared.
    ///     Defaults to <c>false</c>.
    /// </summary>
    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;

    /// <summary>
    ///     Optional headers to include in the Http request.
    /// </summary>
    [JsonProperty("headers", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    ///     The HTTP method to use when sending the request, as a string.
    ///     Defaults to <c>"POST"</c>
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("method", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? MethodString { get; set; } = "POST";

    /// <summary>
    ///     The URL to which the HTTP request will be sent.
    /// </summary>
    [JsonProperty("url", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; }

    #endregion
}