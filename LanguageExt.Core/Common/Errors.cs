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
    /// Bottom error code
    /// </summary>
    public const int BottomCode = -2000000001;
        
    /// <summary>
    /// An error that indicates a value from an operation couldn't be evaluated.  This is a hard
    /// fail for systems that depend on expressions to produce results. 
    /// </summary>
    public static readonly Error Bottom = BottomError.Default;

    /// <summary>
    /// Cancelled error text
    /// </summary>
    public const string CancelledText = "cancelled";

    /// <summary>
    /// Cancelled error code
    /// </summary>
    public const int CancelledCode = -2000000000;

    /// <summary>
    /// Cancelled error
    /// </summary>
    public static readonly Error Cancelled = (CancelledCode, CancelledText);
 
    /// <summary>
    /// Timed-out error text
    /// </summary>
    public const string TimedOutText = "timed out";

    /// <summary>
    /// Timed-out error code
    /// </summary>
    public const int TimedOutCode = -2000000002;

    /// <summary>
    /// Timed-out error
    /// </summary>
    public static readonly Error TimedOut = (TimedOutCode, TimedOutText);    

    /// <summary>
    /// Sequence-empty error text
    /// </summary>
    public const string SequenceEmptyText = "sequence empty";

    /// <summary>
    /// Sequence-empty error code
    /// </summary>
    public const int SequenceEmptyCode = -2000000003;

    /// <summary>
    /// Sequence-empty error
    /// </summary>
    public static readonly Error SequenceEmpty = (SequenceEmptyCode, SequenceEmptyText);    

    /// <summary>
    /// Closed error text
    /// </summary>
    public const string ClosedText = "closed";

    /// <summary>
    /// Closed error code
    /// </summary>
    public const int ClosedCode = -2000000004;

    /// <summary>
    /// Closed error
    /// </summary>
    public static readonly Error Closed = (ClosedCode, ClosedText);    

    /// <summary>
    /// Parse error error code
    /// </summary>
    public const int ParseErrorCode = -2000000005;

    /// <summary>
    /// Parse error
    /// </summary>
    public static Error ParseError(string msg) => (ParseErrorCode, msg);

    /// <summary>
    /// Many errors code
    /// </summary>
    public const int ManyErrorsCode = -2000000006;

    /// <summary>
    /// IO monad not in transformer stack error text
    /// </summary>
    public const string LiftIONotSupportedText = 
        "The IO monad is not in the monad-transformer stack or MonadIO.LiftIO has not been implemented in the trait "   +
        "implementation for your monad-type.  Therefore it's not possible to leverage `MonadIO` lifting functionality. " +
        "To resolve this, implement `MonadIO.LiftIO`.";

    /// <summary>
    /// IO monad not in transformer stack error code
    /// </summary>
    public const int LiftIONotSupportedCode = -2000000007;

    /// <summary>
    /// IO monad not in transformer stack error
    /// </summary>
    public static readonly Error LiftIONotSupported = (LiftIONotSupportedCode, LiftIONotSupportedText);

    /// <summary>
    /// Transformer stack has no unliftIO support error text
    /// </summary>
    public const string UnliftIONotSupportedText =
        "The IO monad is not in the monad-transformer stack or MonadIO.ToIO has not been implemented in the trait " +
        "implementation for your monad-type.  Therefore it's not possible to leverage `MonadIO` unlifting trait "   +
        "functionality. To resolve this, implement `MonadIO.ToIO` and/or `MonadIO,MapIO`.";

    /// <summary>
    /// Transformer stack has no unliftIO support error code
    /// </summary>
    public const int UnliftIONotSupportedCode = -2000000008;

    /// <summary>
    /// Transformer stack has no unliftIO support error
    /// </summary>
    public static readonly Error UnliftIONotSupported = (UnliftIONotSupportedCode, UnliftIONotSupportedText);

    /// <summary>
    /// End-of-stream error text
    /// </summary>
    public const string EndOfStreamText = 
        "end of stream";

    /// <summary>
    /// End-of-stream error code
    /// </summary>
    public const int EndOfStreamCode = -2000000009;

    /// <summary>
    /// End-of-stream error
    /// </summary>
    public static readonly Error EndOfStream = (EndOfStreamCode, EndOfStreamText);

    /// <summary>
    /// Validation failed error text
    /// </summary>
    public const string ValidationFailedText = 
        "validation failed";

    /// <summary>
    /// Validation failed error code
    /// </summary>
    public const int ValidationFailedCode = -2000000010;

    /// <summary>
    /// Validation failed error
    /// </summary>
    public static readonly Error ValidationFailed = (ValidationFailedCode, ValidationFailedText);

    /// <summary>
    /// Source completed error text
    /// </summary>
    public const string SourceCompletedText =
        "source completed";
    
    /// <summary>
    /// Source completed error code
    /// </summary>
    public const int SourceCompletedCode = -2000000011;

    /// <summary>
    /// Source completed error
    /// </summary>
    public static readonly Error SourceCompleted = (SourceCompletedCode, SourceCompletedText);

    /// <summary>
    /// IO DSL extension error text
    /// </summary>
    public const string IODSLExtensionText =
        "If you are trying to extend the `IO` type then you must use: `InvokeAsync`, `InvokeSync`, `InvokeAsyncIO`, "+
        "`InvokeSyncIO`, or `IOCatch` as the base-type, not `IO`";
    
    /// <summary>
    /// IO DSL extension error code
    /// </summary>
    public const int IODSLExtensionCode = -2000000012;

    /// <summary>
    /// IO DSL extension error
    /// </summary>
    public static readonly Error IODSLExtension = new Exceptional(IODSLExtensionText, IODSLExtensionCode);
}
