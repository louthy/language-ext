using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec;
using static LanguageExt.ParserResult;

namespace LanguageExt
{
    /// <summary>
    /// Parser delegate type - Parses an input PString and returns a ParserResult
    /// </summary>
    /// <typeparam name="T">Parsed value result type</typeparam>
    /// <param name="input">Input string</param>
    /// <returns>Parsed value or error report</returns>
    public delegate ParserResult<T> Parser<T>(PString input);
}

public static class ___ParserExt
{
    /// <summary>
    /// A label for the parser
    /// </summary>
    /// <param name="expected">What was expected</param>
    public static Parser<T> label<T>(this Parser<T> p, string expected) =>
        inp =>
        {
            var res = p(inp);
            if (res.Tag == ResultTag.Consumed)
            {
                return res;
            }
            if (res.Reply.Tag == ReplyTag.Error)
            {
                return EmptyError<T>(ParserError.Expect(inp.Pos, res.Reply.Error.Msg, expected));
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

    public static ParserResult<T> Parse<T>(this Parser<T> self, PString input) =>
        self(input);

    public static ParserResult<T> Parse<T>(this Parser<T> self, string input) =>
        self(PString.Zero.SetValue(input));

    public static Parser<T> Filter<T>(this Parser<T> self, Func<T, bool> pred) =>
        self.Where(pred);

    public static Parser<T> Where<T>(this Parser<T> self, Func<T,bool> pred) =>
        inp =>
            self(inp).Match(
                EmptyOK:        (x,rem,msg) => pred(x) ? EmptyOK(x,rem, msg) : EmptyError<T>(ParserError.SysUnExpect(inp.Pos, $"\"{x}\"")),
                EmptyError:     msg         => EmptyError<T>(msg),
                ConsumedOK:     (x,rem,msg) => pred(x) ? ConsumedOK(x,rem, msg) : EmptyError<T>(ParserError.SysUnExpect(inp.Pos, $"\"{x}\"")),
                ConsumedError:  msg         => ConsumedError<T>(msg));

    public static Parser<U> Map<T, U>(this Parser<T> self, Func<T, U> map) =>
        self.Select(map);

    public static Parser<U> Select<T, U>(this Parser<T> self, Func<T, U> map) =>
        inp =>
            self(inp).Match(
                EmptyOK:        (x,rem,msg) => EmptyOK(map(x),rem, msg),
                EmptyError:     msg         => EmptyError<U>(msg),
                ConsumedOK:     (x,rem,msg) => ConsumedOK(map(x),rem, msg),
                ConsumedError:  msg         => ConsumedError<U>(msg));

    public static Parser<V> SelectMany<T, U, V>(
        this Parser<T> self,
        Func<T, Parser<U>> bind,
        Func<T, U, V> project) =>
        state =>
            self(state).Match(

                // consumed-okay case for self
                ConsumedOK: (x, s, err) =>
                    bind(x)(s).Match(

                        // if bind(x) consumes, those go straigt up
                        ConsumedOK:    (x1, s1, err1) => ConsumedOK(project(x,x1), s1, err1),
                        ConsumedError: err1           => ConsumedError<V>(err1),

                        // if bind(x) doesn't consume input, but is okay,
                        // we still return in the consumed continuation
                        EmptyOK:       (x1, s1, err1) => EmptyOK(project(x, x1), s1, merge(err, err1)),

                        // if bind(x) doesn't consume input, but errors,
                        // we return the error in the 'consumed-error'
                        // continuation
                        EmptyError:    err1           => ConsumedError<V>(merge(err, err1))
                    ),

                // empty-ok case for self
                EmptyOK: (x, s, err) =>
                    bind(x)(s).Match(
                        // in these cases, (k x) can return as empty
                        ConsumedOK:    (x1, s1, err1) => ConsumedOK(project(x, x1), s1, err1),
                        EmptyOK:       (x1, s1, err1) => EmptyOK(project(x, x1), s1, merge(err, err1)),
                        ConsumedError: err1           => ConsumedError<V>(err1),
                        EmptyError:    err1           => EmptyError<V>(merge(err, err1))
                    ),

                ConsumedError: err =>
                    ConsumedError<V>(err),

                EmptyError: err =>
                    EmptyError<V>(err)
            );
}
