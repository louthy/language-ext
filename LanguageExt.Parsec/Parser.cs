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
            p(inp).Match(
                EmptyError: msg           => EmptyError<T>(msg.Expect(expected)),
                EmptyOK:    (x, st, msg)  => EmptyOK(x, st, msg == null ? null : msg.Expect(expected)),
                Consumed:   other         => other
                );

    public static ParserResult<T> Parse<T>(this Parser<T> self, PString input) =>
        self(input);

    public static ParserResult<T> Parse<T>(this Parser<T> self, string input) =>
        self(PString.Zero.SetValue(input));

    public static Parser<T> Filter<T>(this Parser<T> self, Func<T, bool> pred) =>
        self.Where(pred);

    public static Parser<T> Where<T>(this Parser<T> self, Func<T,bool> pred) =>
        inp =>
            self(inp).Match(
                EmptyOK:        (x,rem,msg) => pred(x) ? EmptyOK(x,rem, msg) : EmptyError<T>(inp.Pos,""),
                EmptyError:     msg         => EmptyError<T>(msg),
                ConsumedOK:     (x,rem,msg) => pred(x) ? ConsumedOK(x,rem, msg) : EmptyError<T>(inp.Pos, ""),
                ConsumedError:  (rem,msg)   => ConsumedError<T>(rem,msg));

    public static Parser<U> Map<T, U>(this Parser<T> self, Func<T, U> map) =>
        self.Select(map);

    public static Parser<U> Select<T, U>(this Parser<T> self, Func<T, U> map) =>
        inp =>
            self(inp).Match(
                EmptyOK:        (x,rem,msg) => EmptyOK(map(x),rem, msg),
                EmptyError:     msg         => EmptyError<U>(msg),
                ConsumedOK:     (x,rem,msg) => ConsumedOK(map(x),rem, msg),
                ConsumedError:  (rem,msg)   => ConsumedError<U>(rem,msg));

    public static Parser<U> Bind<T, U>(this Parser<T> self, Func<T, Parser<U>> bind) =>
        inp =>
            self(inp).Match(
                EmptyOK:        (x,rem,msg) => bind(x)(rem),
                EmptyError:     msg         => EmptyError<U>(msg),
                ConsumedOK:     (x,rem,msg) => bind(x)(rem),
                ConsumedError:  (rem,msg)   => ConsumedError<U>(rem,msg));

    public static Parser<V> SelectMany<T, U, V>(
        this Parser<T> self,
        Func<T, Parser<U>> bind,
        Func<T, U, V> project) =>
        inp =>
            self(inp).Match(
                EmptyOK: (t, rem1, msg1) =>  
                    bind(t)(rem1).Match(
                        EmptyOK:        (u, rem2, msg2) => mergeOk(project(t, u), rem2, msg1, msg2),
                        EmptyError:     msg2            => mergeError<V>(msg1, msg2),
                        ConsumedOK:     (u, rem, msg2)  => ConsumedOK(project(t, u), rem, msg1),
                        ConsumedError:  (rem, msg2)     => ConsumedError<V>(rem, merge(msg1, msg2))),

                EmptyError: msg => 
                    EmptyError<V>(msg),

                ConsumedOK: (t, rem1, msg1) => 
                    bind(t)(rem1).Match(
                        EmptyOK:        (u, rem2, msg2) => mergeOk(project(t, u), rem2, msg1, msg2),
                        EmptyError:     msg2            => mergeError<V>(msg1, msg2),
                        ConsumedOK:     (u, rem2, msg2) => ConsumedOK<V>(project(t, u), rem2, msg2),
                        ConsumedError:  (rem, msg2)     => ConsumedError<V>(rem, msg2)),

                ConsumedError: (rem, msg) =>        
                    ConsumedError<V>(rem, msg));
}
