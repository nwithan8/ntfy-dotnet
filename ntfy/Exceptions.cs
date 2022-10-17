using System.Diagnostics.CodeAnalysis;

namespace ntfy;

/// <summary>
///     Base exception for all exceptions thrown by the ntfy library.
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class NftyException : Exception
{
    /// <summary>
    ///     Constructs a new exception with the given message.
    /// </summary>
    /// <param name="message">Message to include when throwing exception.</param>
    protected NftyException(string message) : base(message)
    {
    }
}

/// <summary>
///     Exception thrown when an unauthorized operation is attempted.
/// </summary>
public class UnauthorizedException : NftyException
{
    /// <summary>
    ///     Constructs a new exception.
    /// </summary>
    /// <param name="user">Optional specific user attempting unauthorized operation.</param>
    internal UnauthorizedException(User? user = null) : base($"{user?.Username ?? "user"} is not authorized to perform this action.")
    {
    }
}

/// <summary>
///     Exception thrown when a payload is too large.
/// </summary>
public class EntityTooLargeException : NftyException
{
    /// <summary>
    ///     Constructs a new exception.
    /// </summary>
    internal EntityTooLargeException() : base("The payload is too large.")
    {
    }
}

/// <summary>
///     Exception thrown when the server is imposing a rate limit.
/// </summary>
public class TooManyRequestsException : NftyException
{
    /// <summary>
    ///     Constructs a new exception.
    /// </summary>
    internal TooManyRequestsException() : base("Too many requests.")
    {
    }
}

/// <summary>
///     Exception thrown when an unexpected error occurs.
/// </summary>
public class UnexpectedException : NftyException
{
    /// <summary>
    ///     Constructs a new exception.
    /// </summary>
    /// <param name="message">Message to include when throwing exception.</param>
    internal UnexpectedException(string message) : base(message)
    {
    }
}

/// <summary>
///     Exception thrown when a topic is invalid.
/// </summary>
public class InvalidTopicException : NftyException
{
    /// <summary>
    ///     Constructs a new exception.
    /// </summary>
    /// <param name="topic">Name of the invalid topic.</param>
    internal InvalidTopicException(string topic) : base($"Invalid topic: {topic}. Topic must be 1-64 characters long and contain only letters and numbers.")
    {
    }
}

/// <summary>
///     Exception thrown when a parameter is invalid.
/// </summary>
public class InvalidParameterException : NftyException
{
    /// <summary>
    ///     Constructs a new exception.
    /// </summary>
    /// <param name="parameterName">Name of the invalid parameter.</param>
    internal InvalidParameterException(string parameterName) : base($"Invalid parameter: {parameterName}.")
    {
    }
}
