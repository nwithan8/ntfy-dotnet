using System.Text;

namespace ntfy;

/// <summary>
///     Utilities functions.
/// </summary>
public static class Utilities
{
    /// <summary>
    ///     Generate a random topic name.
    /// </summary>
    /// <param name="useWords">Whether the topic should be human-readable words. Default: false.</param>
    /// <returns>A random topic name.</returns>
    public static string GenerateRandomTopic(bool useWords = false)
    {
        // In line with how the web does it (https://github.com/binwiederhier/ntfy/blob/e0d6a0b974ad2210af128a03ed6288291c19179f/web/src/components/SubscribeDialog.js#L21)
        return useWords ? NetTools.Crypto.Passwords.GenerateRandomPassphrase(16, 64) : NetTools.Crypto.Passwords.GenerateRandomPassword(16, false);
    }
    
    /*
    public static async Task<EphemeralUser> GenerateTemporaryUser(Client client)
    {
        var username = NetTools.Crypto.Passwords.GenerateRandomPassword(16, false);
        var password = NetTools.Crypto.Passwords.GenerateRandomPassword(16, false);
        
        var user = await client.SignUp(username, password);
        
        return new EphemeralUser(client, user.Username!, user.Password!);
    }
    */

    internal static string BasicAuthHeaderValue(string username, string password)
    {
        var encoded = Base64Encode($"{username}:{password}");
        return $"Basic {encoded}"; 
    }

    internal static string TokenAuthHeaderValue(string authToken)
    {
        return $"Bearer {authToken}";
    }
    
    internal static string Base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
}
