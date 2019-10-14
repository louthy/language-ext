using System;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Common;
using static LanguageExt.Parsec.ParserResult;
using static LanguageExt.Parsec.ParserResultIO;
using System.Diagnostics;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Parser delegate type - Parses an input PString and returns a ParserResult
    /// </summary>
    /// <typeparam name="T">Parsed value result type</typeparam>
    /// <param name="input">Input string</param>
    /// <returns>Parsed value or error report</returns>
    public delegate ParserResult<T> Parser<T>(PString input);
}

public static class ParserExtensions
{
    public static Parser<char, T> ToParserIO<T>(this Parser<T> self) =>
        inp =>
        {
            var res = self(new PString(new string(inp.Value), inp.Index, inp.EndIndex, inp.Pos, inp.DefPos, inp.Side, inp.UserState));

            var state = res.Reply.State;

            var pstr = new PString<char>(
                        state.Value.ToCharArray(),
                        state.Index,
                        state.EndIndex,
                        state.Pos,
                        state.DefPos,
                        state.Side,
                        state.UserState);

            var reply = res.Reply;

            return new ParserResult<char, T>(
                res.Tag,
                new Reply<char, T>(
                    reply.Tag,
                    reply.Result,
                    pstr,
                    reply.Error));
        };


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

    public static Parser<T> Where<T>(this Parser<T> self, Func<T, bool> pred) =>
        inp =>
            self(inp).Match(
                EmptyOK: (x, rem, msg) => pred(x) ? EmptyOK(x, rem, msg) : EmptyError<T>(ParserError.SysUnexpect(inp.Pos, $"\"{x}\"")),
                EmptyError: msg => EmptyError<T>(msg),
                ConsumedOK: (x, rem, msg) => pred(x) ? ConsumedOK(x, rem, msg) : EmptyError<T>(ParserError.SysUnexpect(inp.Pos, $"\"{x}\"")),
                ConsumedError: msg => ConsumedError<T>(msg));

    public static Parser<U> Map<T, U>(this Parser<T> self, Func<T, U> map) =>
        self.Select(map);

    public static Parser<U> Select<T, U>(this Parser<T> self, Func<T, U> map) =>
        inp => self(inp).Select(map);

    public static Parser<V> SelectMany<T, U, V>(
        this Parser<T> self,
        Func<T, Parser<U>> bind,
        Func<T, U, V> project) =>
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
                        return ConsumedError<V>(u.Reply.Error);
                    }

                    if (u.Tag == ResultTag.Empty && u.Reply.Tag == ReplyTag.OK)
                    {
                        // cok, eok -> cok  (not a typo, this should be -> cok)
                        var v = project(t.Reply.Result, u.Reply.Result);
                        return ConsumedOK(v, u.Reply.State, mergeError(t.Reply.Error, u.Reply.Error));
                    }

                    // cok, eerr
                    return ConsumedError<V>(mergeError(t.Reply.Error, u.Reply.Error));
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
                        return ConsumedError<V>(u.Reply.Error);
                    }

                    // eok, eerr
                    return EmptyError<V>(mergeError(t.Reply.Error, u.Reply.Error));
                }

                // cerr
                if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                {
                    return ConsumedError<V>(t.Reply.Error);
                }

                // eerr
                return EmptyError<V>(t.Reply.Error);
            };

    public static Parser<T> Flatten<T>(this Parser<Option<T>> p, Func<string> failureText) =>
        from value in p
        from returnValue in value.Match(result, compose(failureText, failure<T>))
        select returnValue;

    public static Parser<R> Flatten<L, R>(this Parser<Either<L, R>> p, Func<L, string> failureText) =>
        from value in p
        from returnValue in value.Match(result, compose(failureText, failure<R>))
        select returnValue;
    
    public static Parser<R> Flatten<R>(this Parser<Either<string,R>> p) => Flatten(p, identity);
}
