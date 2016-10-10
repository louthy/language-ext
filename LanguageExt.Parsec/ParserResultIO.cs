﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics;

namespace LanguageExt.Parsec
{
    public static partial class ParserResultIO
    {
        public static ParserResult<I, O> Consumed<I, O>(Reply<I, O> reply) =>
            new ParserResult<I, O>(ResultTag.Consumed, reply);

        public static ParserResult<I, O> Empty<I, O>(Reply<I, O> reply) =>
            new ParserResult<I, O>(ResultTag.Empty, reply);

        public static ParserResult<I, O> EmptyOK<I, O>(O value, PString<I> input, ParserError error = null) =>
            new ParserResult<I, O>(ResultTag.Empty, ReplyIO.OK(value, input, error));

        public static ParserResult<I, O> EmptyError<I, O>(ParserError error) =>
            new ParserResult<I, O>(ResultTag.Empty, ReplyIO.Error<I, O>(error));

        public static ParserResult<I, O> ConsumedOK<I, O>(O value, PString<I> input) =>
            new ParserResult<I, O>(ResultTag.Consumed, ReplyIO.OK(value, input));

        public static ParserResult<I, O> ConsumedOK<I, O>(O value, PString<I> input, ParserError error) =>
            new ParserResult<I, O>(ResultTag.Consumed, ReplyIO.OK(value, input, error));

        public static ParserResult<I, O> ConsumedError<I, O>(ParserError error) =>
            new ParserResult<I, O>(ResultTag.Consumed, ReplyIO.Error<I, O>(error));

    }

    public class ParserResult<I, O>
    {
        public readonly ResultTag Tag;
        public readonly Reply<I, O> Reply;

        internal ParserResult(ResultTag tag, Reply<I, O> reply)
        {
            Tag = tag;
            Reply = reply;
        }

        public ParserResult<I, O> SetEndIndex(int endIndex) =>
            new ParserResult<I, O>(Tag, Reply.SetEndIndex(endIndex));

        public ParserResult<I, U> Project<S, U>(S s, Func<S, O, U> project) =>
            new ParserResult<I, U>(Tag, Reply.Project(s, project));

        public override string ToString() =>
            Reply.Error == null
                ? "success"
                : Reply.Error.ToString();

        public bool IsFaulted =>
            Reply.Tag == ReplyTag.Error;

        public R Match<R>(
            Func<ParserError, R> EmptyError,
            Func<ParserError, R> ConsumedError,
            Func<ParserResult<I, O>, R> Otherwise
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
            Func<ParserResult<I, O>, R> Otherwise
            )
        {
            if (Tag == ResultTag.Empty && Reply.Tag == ReplyTag.Error)
            {
                return EmptyError(Reply.Error);
            }
            return Otherwise(this);
        }

        public R Match<R>(
            Func<Reply<I, O>, R> Empty,
            Func<ParserResult<I, O>, R> Otherwise
            )
        {
            if (Tag == ResultTag.Empty)
            {
                return Empty(Reply);
            }
            return Otherwise(this);
        }

        public R Match<R>(
            Func<Reply<I, O>, R> Empty,
            Func<Reply<I, O>, R> Consumed
            )
        {
            if (Tag == ResultTag.Empty)
            {
                return Empty(Reply);
            }
            return Consumed(Reply);
        }

        public R Match<R>(
            Func<O, PString<I>, ParserError, R> ConsumedOK,
            Func<ParserError, R> ConsumedError,
            Func<O, PString<I>, ParserError, R> EmptyOK,
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

        public ParserResult<I, U> Select<U>(Func<O, U> map) =>
            new ParserResult<I, U>(Tag, Reply.Select(map));
    }
}
