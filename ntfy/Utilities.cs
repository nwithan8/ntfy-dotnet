namespace ntfy;

/// <summary>
///     Utilities functions.
/// </summary>
public static class Utilities
{
    /// <summary>
    ///     Generate a random password.
    /// </summary>
    /// <param name="length">Length of the password. Default: 16 characters.</param>
    /// <param name="allowSpecialCharacters">Whether to allow special characters in the password. Default: true.</param>
    /// <returns>A random password of a specific length.</returns>
    public static string GenerateRandomPassword(int length = 16, bool allowSpecialCharacters = true)
    {
        return NetTools.Crypto.Passwords.GenerateRandomPassword(length, allowSpecialCharacters);
    }

    /// <summary>
    ///     Generate a random passphrase (human-readable words).
    /// </summary>
    /// <param name="minLength">Minimum length of the passphrase. Default: 16 characters.</param>
    /// <returns>A random passphrase of a specific length.</returns>
    public static string GenerateRandomPassphrase(int minLength = 16)
    {
        return NetTools.Crypto.Passwords.GenerateRandomPassphrase(minLength, 64);
    }

    /// <summary>
    ///     Generate a random topic name.
    /// </summary>
    /// <param name="useWords">Whether the topic should be human-readable words. Default: false.</param>
    /// <returns>A random topic name.</returns>
    public static string GenerateRandomTopic(bool useWords = false)
    {
        return useWords ? GenerateRandomPassphrase(32) : GenerateRandomPassword(32, false);
    }
}
