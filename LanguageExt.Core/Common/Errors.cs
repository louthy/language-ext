using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using static LanguageExt.Prelude;

namespace LanguageExt.Common
{
    public static class Errors
    {
        public const string CancelledText = "cancelled";
        public const int CancelledCode = -2000000000;
        public static readonly Error Cancelled = (CancelledCode, CancelledText);

        public const string BottomText = "bottom";
        public const int BottomCode = -2000000001;
        public readonly static Error Bottom = (BottomCode, BottomText);

        public const string TimedOutText = "timed out";
        public const int TimedOutCode = -2000000002;
        public static readonly Error TimedOut = (TimedOutCode, TimedOutText);    

        public const string SequenceEmptyText = "sequence empty";
        public const int SequenceEmptyCode = -2000000003;
        public static readonly Error SequenceEmpty = (SequenceEmptyCode, SequenceEmptyText);    

        public const string ClosedText = "closed";
        public const int ClosedCode = -2000000004;
        public static readonly Error Closed = (ClosedCode, ClosedText);    

        public const int ParseErrorCode = -2000000005;
        public static Error ParseError(string msg) => (ParseErrorCode, msg);    
    }
}
