using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics;

namespace LanguageExt.Parsec
{
    public static class ParserResult
    {
        public static ParserResult<T> Consumed<T>(Reply<T> reply) =>
            new ParserResult<T>(ResultTag.Consumed, reply);

        public static ParserResult<T> Empty<T>(Reply<T> reply) =>
            new ParserResult<T>(ResultTag.Empty, reply);

        public static ParserResult<T> EmptyOK<T>(T value, PString input, ParserError error = null) =>
            new ParserResult<T>(ResultTag.Empty, Reply.OK(value, input, error));

        public static ParserResult<T> EmptyError<T>(ParserError error) =>
            new ParserResult<T>(ResultTag.Empty, Reply.Error<T>(error));

        public static ParserResult<T> ConsumedOK<T>(T value, PString input) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.OK(value, input));

        public static ParserResult<T> ConsumedOK<T>(T value, PString input, ParserError error) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.OK(value, input, error));

        public static ParserResult<T> ConsumedError<T>(ParserError error) =>
            new ParserResult<T>(ResultTag.Consumed, Reply.Error<T>(error));

    }

    public enum ResultTag
    {
        Consumed,
        Empty
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

        public ParserResult<T> SetEndIndex(int endIndex) =>
            new ParserResult<T>(Tag, Reply.SetEndIndex(endIndex));

        public ParserResult<U> Project<S, U>(S s, Func<S, T, U> project) =>
            new ParserResult<U>(Tag, Reply.Project(s, project));

        public override string ToString() =>
            IsFaulted
                ? Reply?.Error?.ToString() ?? "Error"
                : $"Success({Reply.Result})";

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

        public ParserResult<U> Select<U>(Func<T,U> map) =>
            new ParserResult<U>(Tag, Reply.Select(map));

        public Either<string, T> ToEither() =>
            IsFaulted
                ? Left<string, T>(ToString())
                : Right(Reply.Result);

        public Either<ERROR, T> ToEither<ERROR>(Func<string, ERROR> f) =>
            IsFaulted
                ? Left<ERROR, T>(f(ToString()))
                : Right(Reply.Result);

        public Option<T> ToOption() =>
            IsFaulted
                ? None
                : Some(Reply.Result);
    }
}
