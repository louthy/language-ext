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
    public static class ReplyIO
    {
        public static Reply<I, O> OK<I, O>(O result, PString<I> remaining, ParserError error = null) =>
            new Reply<I, O>(result, remaining, error);

        public static Reply<I, O> Error<I, O>(ParserErrorTag tag, Pos pos, string message, Lst<string> expected) =>
            new Reply<I, O>(new ParserError(tag, pos, message, expected, null));

        public static Reply<I, O> Error<I, O>(ParserError error) =>
            new Reply<I, O>(error);
    }

    public class Reply<I, O>
    {
        public readonly ReplyTag Tag;
        public readonly O Result;
        public readonly PString<I> State;
        public readonly ParserError Error;

        internal Reply(ParserError error)
        {
            Debug.Assert(error != null);

            Tag = ReplyTag.Error;
            Error = error;
            State = PString<I>.Zero;
        }

        internal Reply(O result, PString<I> state, ParserError error = null)
        {
            Debug.Assert(notnull(result));

            Tag = ReplyTag.OK;
            State = state;
            Result = result;
            Error = error;
        }

        internal Reply(ReplyTag tag, O result, PString<I> state, ParserError error)
        {
            Tag = tag;
            Result = result;
            State = state;
            Error = error;
        }

        public Reply<I, U> Project<S, U>(S s, Func<S, O, U> project) =>
            Tag == ReplyTag.Error
                ? ReplyIO.Error<I, U>(Error)
                : ReplyIO.OK(project(s, Result), State, Error);

        public Reply<I, U> Select<U>(Func<O, U> map) =>
            Tag == ReplyTag.Error
                ? ReplyIO.Error<I, U>(Error)
                : ReplyIO.OK(map(Result), State, Error);

        internal Reply<I, O> SetEndIndex(int endIndex) =>
            new Reply<I, O>(Tag, Result, State.SetEndIndex(endIndex), Error);
    }
}
