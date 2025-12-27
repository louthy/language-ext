namespace LanguageExt.Common;

public static class ErrorCodes
{
    /// <summary>
    /// An error that indicates a value from an operation couldn't be evaluated.  This is a hard
    /// fail for systems that depend on expressions to produce results. 
    /// </summary>
    public const int Bottom = -2000000000;

    /// <summary>
    /// Cancelled error
    /// </summary>
    public const int Cancelled = -2000000001;

    /// <summary>
    /// Timed-out error
    /// </summary>
    public const int TimedOut = -2000000002;

    /// <summary>
    /// Sequence-empty error
    /// </summary>
    public const int SequenceEmpty = -2000000003;

    /// <summary>
    /// Closed error
    /// </summary>
    public const int Closed = -2000000004;

    /// <summary>
    /// Parsing error
    /// </summary>
    public const int ParseError = -2000000005;

    /// <summary>
    /// Error code that represents multiple errors
    /// </summary>
    public const int ManyErrors = -2000000006;

    /// <summary>
    /// IO monad not in transformer stack or `MonadIO.LiftIO` not implemented
    /// </summary>
    public const int LiftIONotSupported = -2000000007;

    /// <summary>
    /// IO monad not in transformer stack or `MonadIO.Fork` not implemented 
    /// </summary>
    public const int ForkIONotSupported = -2000000008;

    /// <summary>
    /// IO monad not in transformer stack or `MonadUnliftIO.ToIO` not implemented 
    /// </summary>
    public const int ToIONotSupported = -2000000009;

    /// <summary>
    /// End of stream error code
    /// </summary>
    public const int EndOfStream = -2000000010;

    /// <summary>
    /// Validation failed
    /// </summary>
    public const int ValidationFailed = -2000000011;

    /// <summary>
    /// Source complete
    /// </summary>
    public const int SourceCompleted = -2000000012;

    /// <summary>
    /// Source is closed
    /// </summary>
    public const int SourceClosed = -2000000013;

    /// <summary>
    /// The programmer is trying to extend the `IO` type, they need to use specific base-types:
    ///
    ///     * `InvokeAsync`
    ///     * `InvokeSync`
    ///     * `InvokeAsyncIO`
    ///     * `InvokeSyncIO`
    ///     * `IOCatch`
    /// </summary>
    public const int IODSLExtension = -2000000014;

    /// <summary>
    /// Sink full error
    /// </summary>
    public const int SinkFull = -2000000015;
}

