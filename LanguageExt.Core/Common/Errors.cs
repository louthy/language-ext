
namespace LanguageExt.Common
{
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
            "In a bottom state and therefore cannot proceed.  This can happen when an expression couldn't " +
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
        /// Parse error code
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
    }
}
