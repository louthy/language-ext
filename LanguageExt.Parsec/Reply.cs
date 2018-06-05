using System;
using static LanguageExt.Prelude;
using System.Diagnostics;

namespace LanguageExt.Parsec
{
    public enum ReplyTag
    {
        OK,
        Error
    }

    public static partial class Reply
    {
        public static Reply<T> OK<T>(T result, PString remaining, ParserError error = null) =>
            new Reply<T>(result, remaining, error);

        public static Reply<T> Error<T>(ParserErrorTag tag, Pos pos, string message, Lst<string> expected) =>
            new Reply<T>(new ParserError(tag, pos, message, expected, null));

        public static Reply<T> Error<T>(ParserError error) =>
            new Reply<T>(error);
    }

    public class Reply<T>
    {
        public readonly ReplyTag Tag;
        public readonly T Result;
        public readonly PString State;
        public readonly ParserError Error;

        internal Reply(ParserError error)
        {
            Debug.Assert(error != null);

            Tag = ReplyTag.Error;
            Error = error;
            State = PString.Zero;
        }

        internal Reply(T result, PString state, ParserError error = null)
        {
            Debug.Assert(notnull(result));

            Tag = ReplyTag.OK;
            State = state;
            Result = result;
            Error = error;
        }

        internal Reply(ReplyTag tag, T result, PString state, ParserError error)
        {
            Tag = tag;
            Result = result;
            State = state;
            Error = error;
        }

        public Reply<U> Project<S, U>(S s, Func<S, T, U> project) =>
            Tag == ReplyTag.Error
                ? Reply.Error<U>(Error)
                : Reply.OK(project(s, Result), State, Error);

        public Reply<U> Select<U>(Func<T, U> map) =>
            Tag == ReplyTag.Error
                ? Reply.Error<U>(Error)
                : Reply.OK(map(Result), State, Error);

        internal Reply<T> SetEndIndex(int endIndex) =>
            new Reply<T>(Tag, Result, State.SetEndIndex(endIndex), Error);
    }
}
