namespace LanguageExt.Common;

public class Exceptions
{
    /// <summary>
    /// An error state without any error values
    /// </summary>
    public static readonly ErrorException None = new ManyExceptions([]);

    /// <summary>
    /// An error that indicates a value from an operation couldn't be evaluated.  This is a hard
    /// fail for systems that depend on expressions to produce results. 
    /// </summary>
    public static readonly ExceptionalException Bottom = new (Errors.Bottom.Message, Errors.Bottom.Code);

    /// <summary>
    /// Cancelled error
    /// </summary>
    public static readonly ExpectedException Cancelled = new (Errors.Cancelled.Message, Errors.Cancelled.Code);
 
    /// <summary>
    /// Timed-out error
    /// </summary>
    public static readonly ExpectedException TimedOut = new (Errors.TimedOut.Message, Errors.TimedOut.Code);

    /// <summary>
    /// Sequence-empty error
    /// </summary>
    public static readonly ExpectedException SequenceEmpty = new (Errors.SequenceEmpty.Message, Errors.SequenceEmpty.Code);
    /// <summary>
    /// Closed error
    /// </summary>
    public static readonly ExpectedException Closed = new (Errors.Closed.Message, Errors.Closed.Code);

    /// <summary>
    /// Parse error code
    /// </summary>
    public const int ParseErrorCode = -2000000005;

    /// <summary>
    /// Parse error
    /// </summary>
    public static ExpectedException ParseError(string msg) => new (msg, ErrorCodes.ParseError);

    /// <summary>
    /// IO monad not in transformer stack error
    /// </summary>
    public static readonly ExceptionalException LiftIONotSupported = new (Errors.LiftIONotSupported.Message, Errors.LiftIONotSupported.Code);

    /// <summary>
    /// Transformer stack has no unliftIO support error
    /// </summary>
    public static readonly ExceptionalException UnliftIONotSupported = new (Errors.ToIONotSupported.Message, Errors.ToIONotSupported.Code);

    /// <summary>
    /// End-of-stream error
    /// </summary>
    public static readonly ExpectedException EndOfStream = new (Errors.EndOfStream.Message, Errors.EndOfStream.Code);

    /// <summary>
    /// Validation failed error
    /// </summary>
    public static readonly ExpectedException ValidationFailed = new (Errors.ValidationFailed.Message, Errors.ValidationFailed.Code);
}
