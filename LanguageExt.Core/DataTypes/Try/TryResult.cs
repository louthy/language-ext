//using System;
//using System.Diagnostics.Contracts;

//namespace LanguageExt
//{
//    public static class TryResult
//    {
//        [Pure]
//        public static TryResult<T> Cast<T>(T value) =>
//            new TryResult<T>(value);
//    }

//    /// <summary>
//    /// Holds the state of the Try post invocation.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public struct TryResult<T>
//    {
//        internal readonly T Value;
//        internal Exception Exception;

//        public TryResult(T value)
//        {
//            Value = value;
//            Exception = null;
//        }

//        public TryResult(Exception e)
//        {
//            Exception = e;
//            Value = default(T);
//        }

//        [Pure]
//        public static implicit operator TryResult<T>(T value) =>
//            new TryResult<T>(value);

//        [Pure]
//        internal bool IsFaulted => Exception != null;

//        [Pure]
//        public override string ToString() =>
//            IsFaulted
//                ? Exception.ToString()
//                : Value.ToString();

//        public readonly static TryResult<T> Bottom =
//            new TryResult<T>(new BottomException());
//    }
//}
