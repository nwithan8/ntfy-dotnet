using System.Diagnostics.CodeAnalysis;

namespace ntfy;

[SuppressMessage("ReSharper", "IdentifierTypo")]
public class NftyException : Exception
{
    protected NftyException(string message) : base(message)
    {
    }
}

public class UnauthorizedException : NftyException
{
    public UnauthorizedException(User? user = null) : base($"{user?.Username ?? "user"} is not authorized to perform this action.")
    {
    }
}

public class EntityTooLargeException : NftyException
{
    public EntityTooLargeException() : base("The payload is too large.")
    {
    }
}

public class TooManyRequestsException : NftyException
{
    public TooManyRequestsException() : base("Too many requests.")
    {
    }
}

public class UnexpectedException : NftyException
{
    public UnexpectedException(string message) : base(message)
    {
    }
}

public class InvalidTopicException : NftyException
{
    public InvalidTopicException(string topic) : base($"Invalid topic: {topic}. Topic must be 1-64 characters long and contain only letters and numbers.")
    {
    }
}
