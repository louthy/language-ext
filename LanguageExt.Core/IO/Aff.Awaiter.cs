using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt
{
    /// <summary>
    /// Custom awaiter for the Aff<A> monad
    /// Works with AffMethodBuilder to allow Aff<A> to be used with async/await 
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct AffAwaiter<A> : INotifyCompletion
    {
        readonly ValueTaskAwaiter<Fin<A>> awaiter;

        internal AffAwaiter(Aff<A> ma) =>
            this.awaiter = ma.RunIO().GetAwaiter();

        public bool IsCompleted =>
            awaiter.IsCompleted;

        public A GetResult()
        {
            var result = awaiter.GetResult();
            return result.IsSucc
                ? result.Value
                : throw new AffException(result.Error);
        }

        public void OnCompleted(Action completion) =>
            awaiter.OnCompleted(completion);
    }
    
    public class AffException : Exception
    {
        public readonly Error Error;

        public AffException(Error error) =>
            Error = error;

        public override string Message =>
            Error.Message;

        public override string StackTrace =>
            Error.ToException()?.StackTrace ?? "[Stack trace not available]";
    }
}
