using System.Text;

namespace ntfy;

public class User
{
    internal string AuthHeader => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
    internal string Password { get; set; }
    internal string Username { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public override string ToString()
    {
        return Username;
    }
}
