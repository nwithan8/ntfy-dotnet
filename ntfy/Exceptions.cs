namespace ntfy;

public class UnauthorizedException : System.Exception
{
    public UnauthorizedException(User? user = null) : base($"{user?.Username ?? "user"} is not authorized to perform this action.")
    {
    }
}

public class EntityTooLargeException : System.Exception
{
    public EntityTooLargeException() : base("The payload is too large.")
    {
    }
}

public class TooManyRequestsException : System.Exception
{
    public TooManyRequestsException() : base("Too many requests.")
    {
    }
}

public class UnexpectedException : System.Exception
{
    public UnexpectedException(string message) : base(message)
    {
    }
}
