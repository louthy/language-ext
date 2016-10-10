﻿using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Common;
using static LanguageExt.Parsec.ParserResultIO;
using System.Diagnostics;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Parser delegate type - Parses an input PString and returns a ParserResult
    /// </summary>
    /// <typeparam name="I">Input stream element type</typeparam>
    /// <typeparam name="O">Parsed value result type</typeparam>
    /// <param name="input">Input string</param>
    /// <returns>Parsed value or error report</returns>
    public delegate ParserResult<I, O> Parser<I, O>(PString<I> input);
}

public static class ___ParserIOExt
{
    public static Parser<T> ToParser<T>(this Parser<char, T> self) =>
        inp =>
        {
            var res = self(new PString<char>(inp.Value.ToCharArray(), inp.Index, inp.EndIndex, inp.Pos, inp.DefPos, inp.Side, inp.UserState));

            var state = res.Reply.State;

            var pstr = new PString(
                        new string(state.Value),
                        state.Index,
                        state.EndIndex,
                        state.Pos,
                        state.DefPos,
                        state.Side,
                        state.UserState);

            var reply = res.Reply;

            return new ParserResult<T>(
                res.Tag,
                new Reply<T>(
                    reply.Tag,
                    reply.Result,
                    pstr,
                    reply.Error));
        };

    /// <summary>
    /// A label for the parser
    /// </summary>
    /// <param name="expected">What was expected</param>
    public static Parser<I, O> label<I, O>(this Parser<I, O> p, string expected) =>
        inp =>
        {
            var res = p(inp);
            if (res.Tag == ResultTag.Consumed)
            {
                return res;
            }
            if (res.Reply.Tag == ReplyTag.Error)
            {
                return EmptyError<I, O>(ParserError.Expect(inp.Pos, res.Reply.Error.Msg, expected));
            }
            if (res.Reply.Error == null || res.Reply.Error.Tag == ParserErrorTag.Unknown)
            {
                return res;
            }
            else
            {
                return EmptyOK(res.Reply.Result, res.Reply.State, ParserError.Expect(inp.Pos, res.Reply.Error.Msg, expected));
            }
        };

    public static ParserResult<I, O> Parse<I, O>(this Parser<I, O> self, PString<I> input) =>
        self(input);

    public static ParserResult<I, O> Parse<I, O>(this Parser<I, O> self, IEnumerable<I> input) =>
        self(PString<I>.Zero.SetValue(input.ToArray()));

    public static Parser<I, O> Filter<I, O>(this Parser<I, O> self, Func<O, bool> pred) =>
        self.Where(pred);

    public static Parser<I, O> Where<I, O>(this Parser<I, O> self, Func<O, bool> pred) =>
        inp =>
            self(inp).Match(
                EmptyOK: (x, rem, msg) => pred(x) ? EmptyOK(x, rem, msg) : EmptyError<I, O>(ParserError.SysUnexpect(inp.Pos, $"\"{x}\"")),
                EmptyError: msg => EmptyError<I, O>(msg),
                ConsumedOK: (x, rem, msg) => pred(x) ? ConsumedOK(x, rem, msg) : EmptyError<I, O>(ParserError.SysUnexpect(inp.Pos, $"\"{x}\"")),
                ConsumedError: msg => ConsumedError<I, O>(msg));

    public static Parser<I, U> Map<I, O, U>(this Parser<I, O> self, Func<O, U> map) =>
        self.Select(map);

    public static Parser<I, U> Select<I, O, U>(this Parser<I, O> self, Func<O, U> map) =>
        inp => self(inp).Select(map);

    public static Parser<I, V> SelectMany<I, O, U, V>(
        this Parser<I, O> self,
        Func<O, Parser<I, U>> bind,
        Func<O, U, V> project) =>
            inp =>
            {
                Debug.Assert(inp != null);

                var t = self(inp);

                // cok
                if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                {
                    var u = bind(t.Reply.Result)(t.Reply.State);

                    if (u.Tag == ResultTag.Consumed && u.Reply.Tag == ReplyTag.OK)
                    {
                        // cok, cok -> cok
                        var v = project(t.Reply.Result, u.Reply.Result);
                        return ConsumedOK(v, u.Reply.State, u.Reply.Error);
                    }

                    if (u.Tag == ResultTag.Consumed && u.Reply.Tag == ReplyTag.Error)
                    {
                        // cok, cerr -> cerr
                        return ConsumedError<I, V>(u.Reply.Error);
                    }

                    if (u.Tag == ResultTag.Empty && u.Reply.Tag == ReplyTag.OK)
                    {
                        // cok, eok -> cok  (not a typo, this should be -> cok)
                        var v = project(t.Reply.Result, u.Reply.Result);
                        return ConsumedOK(v, u.Reply.State, mergeError(t.Reply.Error, u.Reply.Error));
                    }

                    // cok, eerr
                    return ConsumedError<I, V>(mergeError(t.Reply.Error, u.Reply.Error));
                }

                // eok
                if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                {
                    var u = bind(t.Reply.Result)(t.Reply.State);

                    if (u.Tag == ResultTag.Consumed && u.Reply.Tag == ReplyTag.OK)
                    {
                        // eok, cok -> cok
                        var v = project(t.Reply.Result, u.Reply.Result);
                        return ConsumedOK(v, u.Reply.State, u.Reply.Error);
                    }

                    if (u.Tag == ResultTag.Empty && u.Reply.Tag == ReplyTag.OK)
                    {
                        // eok, eok -> eok
                        var v = project(t.Reply.Result, u.Reply.Result);
                        return EmptyOK(v, u.Reply.State, mergeError(t.Reply.Error, u.Reply.Error));
                    }

                    if (u.Tag == ResultTag.Consumed && u.Reply.Tag == ReplyTag.Error)
                    {
                        // eok, cerr -> cerr
                        return ConsumedError<I, V>(u.Reply.Error);
                    }

                    // eok, eerr
                    return EmptyError<I, V>(mergeError(t.Reply.Error, u.Reply.Error));
                }

                // cerr
                if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                {
                    return ConsumedError<I, V>(t.Reply.Error);
                }

                // eerr
                return EmptyError<I, V>(t.Reply.Error);
            };
}
