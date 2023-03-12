using NetTools.HTTP;
using Newtonsoft.Json;

namespace ntfy.Requests;

/// <summary>
///     A user signup request sent to the server.
/// </summary>
internal class UserSignup
{
    #region JSON Properties

    /// <summary>
    ///     The username of the user to create.
    /// </summary>
    [JsonProperty("username", Required = Required.Always)]
    internal string Username { get; set; }
    
    /// <summary>
    ///     The password of the user to create.
    /// </summary>
    [JsonProperty("password", Required = Required.Always)]
    internal string Password { get; set; }

    #endregion
    
    /// <summary>
    ///     Construct a new <see cref="UserSignup" /> instance.
    /// </summary>
    /// <param name="username">The username to sign up with.</param>
    /// <param name="password">The password to sign up with.</param>
    internal UserSignup(string username, string password)
    {
        Username = username;
        Password = password;
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
