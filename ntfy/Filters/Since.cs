namespace ntfy.Filters;

/// <summary>
///     A filter for received messages, filtering by the message's age.
/// </summary>
public class Since
{
    /// <summary>
    ///     A filter to receive all messages, regardless of age.
    /// </summary>
    public static readonly Since All = new("all");

    /// <summary>
    ///     The string value of the filter.
    /// </summary>
    internal readonly string Value;

    /// <summary>
    ///     Construct a new <see cref="Since" /> filter using a <see cref="DelayDuration" />.
    ///     Will filter out messages older than the specified duration.
    /// </summary>
    /// <param name="time">A <see cref="DelayDuration" /> delay to receive since.</param>
    public Since(DelayDuration time)
    {
        Value = time.Value;
    }

    /// <summary>
    ///     Construct a new <see cref="Since" /> filter using a <see cref="DelayTime" />.
    ///     Will filter out messages older than the specified timestamp.
    /// </summary>
    /// <param name="time"><see cref="DelayTime" /> delay to receive since.</param>
    public Since(DelayTime time)
    {
        Value = time.Value;
    }

    /// <summary>
    ///     Construct a new <see cref="Since" /> filter using a message ID.
    ///     Will filter out messages older than the specified message ID.
    /// </summary>
    /// <param name="messageId">A message ID to receive since.</param>
    public Since(string messageId)
    {
        Value = messageId;
    }

    /// <summary>
    ///     Construct a new <see cref="Since" /> filter by casting a timestamp to a <see cref="Since" />.
    /// </summary>
    /// <param name="timestamp">Timestamp to use in filter.</param>
    /// <returns>A <see cref="Since" /> instance configured with the timestamp.</returns>
    public static implicit operator Since(int timestamp)
    {
        return new Since(new DelayTime(timestamp));
    }

    /// <summary>
    ///     Construct a new <see cref="Since" /> filter by casting a <see cref="DelayDuration" /> to a <see cref="Since" />.
    /// </summary>
    /// <param name="time"><see cref="DelayDuration" /> instance to use in filter.</param>
    /// <returns>A <see cref="Since" /> instance configured with the delay duration.</returns>
    public static implicit operator Since(DelayDuration time)
    {
        return new Since(time);
    }

    /// <summary>
    ///     Construct a new <see cref="Since" /> filter by casting a <see cref="DelayTime" /> to a <see cref="Since" />.
    /// </summary>
    /// <param name="time"><see cref="DelayTime" /> instance to use in filter.</param>
    /// <returns>A <see cref="Since" /> instance configured with the delay timestamp.</returns>
    public static implicit operator Since(DelayTime time)
    {
        return new Since(time);
    }

    /// <summary>
    ///     Construct a new <see cref="Since" /> filter by casting a message ID to a <see cref="Since" />.
    /// </summary>
    /// <param name="messageId">Message ID to use in filter.</param>
    /// <returns>A <see cref="Since" /> instance configured with the message ID.</returns>
    public static implicit operator Since(string messageId)
    {
        return new Since(messageId);
    }
}
