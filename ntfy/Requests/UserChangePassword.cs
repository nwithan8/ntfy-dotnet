using Newtonsoft.Json;

namespace ntfy.Requests;

/// <summary>
///     A user change password request sent to the server.
/// </summary>
internal class UserChangePassword
{
    /// <summary>
    ///     Construct a new <see cref="UserChangePassword" /> instance.
    /// </summary>
    /// <param name="oldPassword">The old password.</param>
    /// <param name="newPassword">The new password.</param>
    internal UserChangePassword(string oldPassword, string newPassword)
    {
        OldPassword = oldPassword;
        NewPassword = newPassword;
    }

    /// <summary>
    ///     Convert this request to a JSON data string.
    /// </summary>
    /// <returns>A JSON data string representation of this request payload.</returns>
    internal string ToData()
    {
        return JsonConvert.SerializeObject(this);
    }

    #region JSON Properties

    /// <summary>
    ///     The old password of the user.
    /// </summary>
    [JsonProperty("password", Required = Required.Always)]
    internal string OldPassword { get; set; }

    /// <summary>
    ///     The new password of the user.
    /// </summary>
    [JsonProperty("new_password", Required = Required.Always)]
    internal string NewPassword { get; set; }

    #endregion
}