using RestSharp.Authenticators;

namespace ntfy;

/// <summary>
///     A user to use when sending and receiving messages.
/// </summary>
public class User
{
    /// <summary>
    ///     The user's basic authentication header.
    /// </summary>
    internal HttpBasicAuthenticator AuthHeader => new(Username, Password);

    /// <summary>
    ///     The user's username.
    /// </summary>
    internal string Username { get; }

    /// <summary>
    ///     The user's password.
    /// </summary>
    private string Password { get; }

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
    ///     Get the user as a string (the username).
    /// </summary>
    /// <returns>The user as a string (the username).</returns>
    public override string ToString()
    {
        return Username;
    }
}
