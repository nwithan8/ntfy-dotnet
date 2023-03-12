namespace ntfy;

/// <summary>
///     A user to use when sending and receiving messages.
/// </summary>
public class User
{
    /// <summary>
    ///     The user's basic authentication header.
    /// </summary>
    internal string AuthHeaderValue
    {
        get
        {
            if (Username != null)
                return Utilities.BasicAuthHeaderValue(Username, Password ?? ""); // Users could have an empty password (https://github.com/binwiederhier/ntfy/issues/374)
            if (AccessToken != null)
                return Utilities.TokenAuthHeaderValue(AccessToken);
            throw new System.Exception("User has no authentication information.");
        }
    }

    /// <summary>
    ///     The user's username.
    /// </summary>
    internal string? Username { get; }

    /// <summary>
    ///     The user's password.
    /// </summary>
    internal string? Password { get; } // Users could have an empty password (https://github.com/binwiederhier/ntfy/issues/374)

    /// <summary>
    ///     The user's access token.
    /// </summary>
    internal string? AccessToken { get; }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <param name="password">The user's password.</param>
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="accessToken">The user's access token.</param>
    public User(string accessToken)
    {
        AccessToken = accessToken;
    }

    /// <summary>
    ///     Get the user as a string (the username).
    /// </summary>
    /// <returns>The user as a string (the username).</returns>
    public override string ToString()
    {
        return Username ?? "Auth Token User";
    }
}
