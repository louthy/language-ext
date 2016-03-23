using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics;

namespace LanguageExt
{
    public static class ParserResult
    {
        public static ParserResult<T> Consumed<T>(Reply<T> reply) =>
            new ParserResult<T>(ResultTag.Consumed, reply);

        public static ParserResult<T> Empty<T>(Reply<T> reply) =>
            new ParserResult<T>(ResultTag.Empty, reply);

        public static ParserResult<T> EmptyOK<T>(T value, PString input, ParserError error = null) =>
            new ParserResult<T>(ResultTag.Empty, Reply.OK(value, input, error));

        public static ParserResult<T> EmptyError<T>(Pos pos, string message) =>
            new ParserResult<T>(ResultTag.Empty, Reply.Error<T>(pos, message, List.empty<string>()));

        public static ParserResult<T> EmptyError<T>(ParserError error) =>
            new ParserResult<T>(ResultTag.Empty, Reply.Error<T>(error));

        public static ParserResult<T> ConsumedOK<T>(T value, PString input) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.OK(value, input));

        public static ParserResult<T> ConsumedOK<T>(T value, PString input, ParserError error) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.OK(value, input, error));

        public static ParserResult<T> ConsumedError<T>(Pos pos, string message, Lst<string> expected) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.Error<T>(pos, message, expected));

        public static ParserResult<T> ConsumedError<T>(ParserError error) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.Error<T>(error));

    }

    public enum ResultTag
    {
        Consumed,
        Empty
    }

    public enum ReplyTag
    {
        OK,
        Error
    }

    public class ParserResult<T>
    {
        public readonly ResultTag Tag;
        public readonly Reply<T> Reply;

        internal ParserResult(ResultTag tag, Reply<T> reply)
        {
            Tag = tag;
            Reply = reply;
        }

        public ParserResult<U> Project<S, U>(S s, Func<S, T, U> project) =>
            new ParserResult<U>(Tag, Reply.Project(s, project));

        public override string ToString()
        {
            if(Reply.Error != null)
            {
                var err = Reply.Error;
                var sb = new StringBuilder();
                while(err != null)
                {
                    sb.AppendLine(err.ToString());
                    sb.AppendLine();
                    err = err.Inner;
                }
                return sb.ToString();
            }
            else
            {
                return "success";
            }
        }
        public bool IsFaulted =>
            Reply.Tag == ReplyTag.Error;

        public R Match<R>(
            Func<ParserError, R> EmptyError,
            Func<ParserError, R> ConsumedError,
            Func<ParserResult<T>, R> Otherwise
            )
        {
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.Error)
            {
                return EmptyError(Reply.Error);
            }
            if (Tag == ResultTag.Consumed && Reply.Tag == ReplyTag.Error)
            {
                return ConsumedError(Reply.Error);
            }
            return Otherwise(this);
        }

        public R Match<R>(
            Func<ParserError, R> EmptyError,
            Func<ParserResult<T>, R> Otherwise
            )
        {
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.Error)
            {
                return EmptyError(Reply.Error);
            }
            return Otherwise(this);
        }

        public R Match<R>(
            Func<Reply<T>, R> Empty,
            Func<ParserResult<T>, R> Otherwise
            )
        {
            if (Tag == ResultTag.Empty)
            {
                return Empty(Reply);
            }
            return Otherwise(this);
        }

        public R Match<R>(
            Func<Reply<T>, R> Empty,
            Func<Reply<T>, R> Consumed
            )
        {
            if (Tag == ResultTag.Empty)
            {
                return Empty(Reply);
            }
            return Consumed(Reply);
        }

        public R Match<R>(
            Func<Reply<T>, R> Consumed,
            Func<T, PString, ParserError, R> EmptyOK,
            Func<ParserError, R> EmptyError
            )
        {
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.OK)
            {
                return EmptyOK(Reply.Result, Reply.State, Reply.Error);
            }
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.Error)
            {
                return EmptyError(Reply.Error);
            }
            return Consumed(Reply);
        }

        public R Match<R>(
            Func<T, PString, ParserError, R> ConsumedOK,
            Func<ParserError, R> ConsumedError,
            Func<T, PString, ParserError, R> EmptyOK,
            Func<ParserError, R> EmptyError
            )
        {
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.OK)
            {
                return EmptyOK(Reply.Result, Reply.State, Reply.Error);
            }
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.Error)
            {
                return EmptyError(Reply.Error);
            }
            if (Tag == ResultTag.Consumed && Reply.Tag == ReplyTag.OK)
            {
                return ConsumedOK(Reply.Result, Reply.State, Reply.Error);
            }
            return ConsumedError(Reply.Error);
        }
    }

    public static class Reply
    {
        public static Reply<T> OK<T>(T result, PString remaining, ParserError error = null) =>
            new Reply<T>(result, remaining, error);

        public static Reply<T> Error<T>(Pos pos, string message, Lst<string> expected) =>
            new Reply<T>(new ParserError(pos, message, expected, null));

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

        internal Reply(T result, PString remaining, ParserError error = null)
        {
            Debug.Assert(notnull(result));

            Tag = ReplyTag.OK;
            State = remaining;
            Result = result;
            Error = error;
        }

        public Reply<U> Project<S, U>(S s, Func<S, T, U> project) =>
            Tag == ReplyTag.Error
                ? Reply.Error<U>(Error)
                : Reply.OK(project(s, Result), State, Error);
    }
}
