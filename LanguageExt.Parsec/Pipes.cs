using System;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using LanguageExt.Traits;

namespace LanguageExt;

public static class ParsecPipes
{
    /// <summary>
    /// Pipe a string to a PString
    /// </summary>
    public static Pipe<string, PString, Unit> toParserString =>
        Proxy.awaiting<string>().Bind(x => Proxy.yield(PString.Zero.SetValue(x)));

    /// <summary>
    /// Pipe tokens to a PString
    /// </summary>
    public static Pipe<A[], PString<A>, Unit> toTokenString<A>(Func<A, Pos>? tokenPos) =>
        Proxy.awaiting<A[]>().Bind(xs => Proxy.yield(new PString<A>(xs, 0, xs.Length, None, tokenPos ?? (_ => Pos.Zero))));

    /// <summary>
    /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
    /// If the parser fails then the pipe fails
    /// </summary>
    public static Pipe<PString, OUT, M, Unit> ToPipe<M, OUT>(this Parser<OUT> ma)
        where M : Monad<M> =>
        from t in Pipe.awaiting<M, PString, OUT>()
        from r in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => IO.Pure(x),
                      Either.Left<string, OUT> (var e)  => IO.Fail<OUT>(Errors.ParseError(e)),
                      _                                 => throw new NotSupportedException()
                  }
        from _ in Pipe.yield<PString, OUT, M>(r)
        select unit;

    /// <summary>
    /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
    /// The value is only forwarded if the parsing succeeds
    /// </summary>
    public static Pipe<PString, OUT, Unit> ToPipe<OUT>(this Parser<OUT> ma) =>
        from t in Proxy.awaiting<PString>()
        from _ in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => Proxy.yield(x),
                      Either.Left<string, OUT>          => Pure<Unit>(default),
                      _                                 => throw new NotSupportedException()
                  }
        select unit;

    /// <summary>
    /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
    /// The value is only forwarded if the parsing succeeds
    /// </summary>
    public static Pipe<PString<IN>, OUT, Unit> ToPipe<IN, OUT>(this Parser<IN, OUT> ma) =>
        from t in Proxy.awaiting<PString<IN>>()
        from _ in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => Proxy.yield(x),
                      Either.Left<string, OUT>          => Pure<Unit>(default),
                      _                                 => throw new NotSupportedException()
                  }
        select unit;

    /// <summary>
    /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
    /// If the parser fails then the pipe fails
    /// </summary>
    public static Pipe<PString<IN>, OUT, M, Unit> ToPipe<M, IN, OUT>(this Parser<IN, OUT> ma)
        where M : Monad<M> =>
        from t in Pipe.awaiting<M, PString<IN>, OUT>()
        from r in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => IO.Pure(x),
                      Either.Left<string, OUT> (var e)  => IO.Fail<OUT>(Errors.ParseError(e)),
                      _                                 => throw new NotSupportedException()
                  }
        from _ in Pipe.yield<PString<IN>, OUT, M>(r)
        select unit;
}
