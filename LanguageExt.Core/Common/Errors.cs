namespace LanguageExt.Common;

public static class Errors
{
    /// <summary>
    /// An error state without any error values
    /// </summary>
    public static readonly Error None = new ManyErrors(Seq<Error>.Empty);

    /// <summary>
    /// Bottom error text
    /// </summary>
    public const string BottomText =
        "In a bottom state and therefore cannot proceed.  This can happen when an expression couldn't "           +
        "evaluate to a value.  This might be due to filter/where, or sometimes if a `struct` wasn't initialised " +
        "properly (i.e. via `default(T)` or an uninitialised member).";
        
    /// <summary>
    /// An error that indicates a value from an operation couldn't be evaluated.  This is a hard
    /// fail for systems that depend on expressions to produce results. 
    /// </summary>
    public static readonly Error Bottom = BottomError.Default;

    /// <summary>
    /// Cancelled error
    /// </summary>
    public static readonly Error Cancelled = (ErrorCodes.Cancelled, "cancelled");

    /// <summary>
    /// Timed-out error
    /// </summary>
    public static readonly Error TimedOut = (ErrorCodes.TimedOut, "timed out");    

    /// <summary>
    /// Sequence-empty error
    /// </summary>
    public static readonly Error SequenceEmpty = (ErrorCodes.SequenceEmpty, "sequence empty");    

    /// <summary>
    /// Closed error
    /// </summary>
    public static readonly Error Closed = (ErrorCodes.Closed, "closed");    

    /// <summary>
    /// Parse error
    /// </summary>
    public static Error ParseError(string msg) => (ErrorCodes.ParseError, msg);

    /// <summary>
    /// IO monad not in transformer stack or `MonadIO.LiftIO` not implemented
    /// </summary>
    public static readonly Error LiftIONotSupported = 
        (ErrorCodes.LiftIONotSupported, 
         "The IO monad is not in the monad-transformer stack or MonadIO.LiftIO has not been implemented in the trait "   +
         "implementation for your monad-type.  Therefore it's not possible to leverage `MonadIO` lifting functionality. " +
         "To resolve this, implement `MonadIO.LiftIO`.");

    /// <summary>
    /// Transformer stack has no `ToIO` support error
    /// </summary>
    public static readonly Error ToIONotSupported = 
        (ErrorCodes.ToIONotSupported, 
         "The IO monad is not in the monad-transformer stack or MonadIO.ToIO has not been implemented in the trait " +
         "implementation for your monad-type.  Therefore it's not possible to leverage `MonadIO` unlifting trait "   +
         "functionality. To resolve this, implement `MonadIO.ToIO` and/or `MonadIO.MapIO`.");

    /// <summary>
    /// Transformer stack has no `Fork` support error
    /// </summary>
    public static readonly Error ForkNotSupported =
        (ErrorCodes.ForkIONotSupported,
         "The IO monad is not in the monad-transformer stack or MonadIO.Fork has not been implemented in the trait " +
         "implementation for your monad-type.");

    /// <summary>
    /// Transformer stack has no `Await` support error
    /// </summary>
    public static readonly Error AwaitNotSupported =
        (ErrorCodes.ForkIONotSupported,
         "The IO monad is not in the monad-transformer stack or MonadIO.Await has not been implemented in the trait " +
         "implementation for your monad-type.");

    /// <summary>
    /// End-of-stream error
    /// </summary>
    public static readonly Error EndOfStream = (ErrorCodes.EndOfStream, "end of stream");

    /// <summary>
    /// Validation failed error
    /// </summary>
    public static readonly Error ValidationFailed = (ErrorCodes.ValidationFailed, "validation failed");

    /// <summary>
    /// Source completed error
    /// </summary>
    public static readonly Error SourceCompleted = (ErrorCodes.SourceCompleted,  "source completed");

    /// <summary>
    /// IO DSL extension error
    /// </summary>
    public static readonly Error IODSLExtension = new Exceptional(
        "If you are trying to extend the `IO` type then you must use: `InvokeAsync`, `InvokeSync`, `InvokeAsyncIO`, "+
        "`InvokeSyncIO`, or `IOCatch` as the base-type, not `IO`",
        ErrorCodes.IODSLExtension);

    /// <summary>
    /// Sink is full error
    /// </summary>
    public static readonly Error SinkFull = new Expected("Sink is full", ErrorCodes.SinkFull);

    /// <summary>
    /// Source is closed error
    /// </summary>
    public static readonly Error SourceClosed = new Expected("Source is closed", ErrorCodes.SourceClosed);
}
