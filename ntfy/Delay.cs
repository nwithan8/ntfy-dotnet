using Enum = NetTools.Enum;

namespace ntfy;

/// <summary>
///     An enum representation of the available units for a delay.
/// </summary>
public class DelayUnit : Enum
{
    /// <summary>
    ///     Enum representation of a day delay unit.
    /// </summary>
    public static readonly DelayUnit Days = new(3, "d");
    /// <summary>
    ///     Enum representation of a hour delay unit.
    /// </summary>
    public static readonly DelayUnit Hours = new(2, "h");
    /// <summary>
    ///     Enum representation of a minute delay unit.
    /// </summary>
    public static readonly DelayUnit Minutes = new(1, "m");
    /// <summary>
    ///     Enum representation of a second delay unit.
    /// </summary>
    public static readonly DelayUnit Seconds = new(0, "s");

    /// <summary>
    ///     Suffix string for the delay unit.
    /// </summary>
    internal readonly string Suffix;

    /// <summary>
    ///     Constructor for the DelayUnit enum.
    /// </summary>
    /// <param name="id">Internal enum ID.</param>
    /// <param name="suffix">String suffix for the delay.</param>
    private DelayUnit(int id, string suffix) : base(id)
    {
        Suffix = suffix;
    }
}

/// <summary>
///     A delay represented as a statement.
/// </summary>
public class DelayStatement : Delay
{
    /// <summary>
    ///     Constructor for the DelayStatement class.
    /// </summary>
    /// <param name="value">The delay statement string.</param>
    public DelayStatement(string value) : base(value)
    {
    }
}

/// <summary>
///     A delay represented as a number and unit.
/// </summary>
public class DelayDuration : Delay
{
    /// <summary>
    ///     Constructor for the DelayDuration class.
    /// </summary>
    /// <param name="value">Numerical value of the delay duration.</param>
    /// <param name="unit">Unit of the delay duration.</param>
    public DelayDuration(int value, DelayUnit unit) : base($"{value}{unit.Suffix}")
    {
    }
}

/// <summary>
///     A delay represented as a timestamp.
/// </summary>
public class DelayTime : Delay
{
    /// <summary>
    ///     Constructor for the DelayTime class.
    /// </summary>
    /// <param name="timestamp">Timestamp of the delay.</param>
    public DelayTime(int timestamp) : base(timestamp.ToString())
    {
    }
}

public abstract class Delay
{
    /// <summary>
    ///     The delay represented as a string.
    /// </summary>
    internal string Value { get; set; }

    /// <summary>
    ///     Constructor for a Delay object.
    /// </summary>
    /// <param name="value">String representation of the delay.</param>
    protected Delay(string value)
    {
        Value = value;
    }
}
