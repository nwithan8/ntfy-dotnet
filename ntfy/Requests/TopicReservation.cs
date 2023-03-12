using NetTools.HTTP;
using Newtonsoft.Json;

namespace ntfy.Requests;

/// <summary>
///     A topic reservation request sent to the server.
/// </summary>
internal class TopicReservation
{
    #region JSON Properties

    /// <summary>
    ///     The name of the topic to reserve.
    /// </summary>
    [JsonProperty("topic", Required = Required.Always)]
    internal string Topic { get; set; }
    
    /// <summary>
    ///     The permission to grant to others for the topic.
    /// </summary>
    [JsonProperty("everyone", Required = Required.Always)]
    internal string Permission { get; set; }

    #endregion
    
    /// <summary>
    ///     Construct a new <see cref="TopicReservation" /> instance.
    /// </summary>
    /// <param name="topic">The topic to reserve.</param>
    /// <param name="permissionForOthers">What permissions other users have on this topic.</param>
    internal TopicReservation(string topic, Permission permissionForOthers)
    {
        Topic = topic;
        Permission = permissionForOthers.ToString()!;
    }

    /// <summary>
    ///     Convert this request to a JSON data string.
    /// </summary>
    /// <returns>A JSON data string representation of this request payload.</returns>
    internal string ToData()
    {
        return JsonSerialization.ConvertObjectToJson(this);
    }
}
