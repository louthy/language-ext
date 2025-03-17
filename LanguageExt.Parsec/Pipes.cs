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
    public static PipeT<string, PString, M, Unit> toParserStringT<M>()
        where M : MonadIO<M> =>
        PipeT.map<M, string, PString>(x => PString.Zero.SetValue(x));

    /// <summary>
    /// Pipe tokens to a PString
    /// </summary>
    public static PipeT<A[], PString<A>, M, Unit> toTokenStringT<M, A>(Func<A, Pos>? tokenPos) 
        where M : MonadIO<M> =>
        PipeT.map<M, A[], PString<A>>(xs => new PString<A>(xs, 0, xs.Length, None, tokenPos ?? (_ => Pos.Zero)));
    
    /// <summary>
    /// Pipe a string to a PString
    /// </summary>
    public static Pipe<RT, string, PString, Unit> toParserString<RT>() =>
        Pipe.map<RT, string, PString>(x => PString.Zero.SetValue(x));

    /// <summary>
    /// Pipe tokens to a PString
    /// </summary>
    public static Pipe<RT, A[], PString<A>, Unit> toTokenString<RT, A>(Func<A, Pos>? tokenPos) =>
        Pipe.map<RT, A[], PString<A>>(xs => new PString<A>(xs, 0, xs.Length, None, tokenPos ?? (_ => Pos.Zero)));

    /// <summary>
    /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
    /// If the parser fails then the pipe fails
    /// </summary>
    public static PipeT<PString, OUT, M, Unit> ToPipeT<M, OUT>(this Parser<OUT> ma)
        where M : MonadIO<M> =>
        from t in PipeT.awaiting<M, PString, OUT>()
        from r in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => IO.pure(x),
                      Either.Left<string, OUT> (var e)  => IO.fail<OUT>(Errors.ParseError(e)),
                      _                                 => throw new NotSupportedException()
                  }
        from _ in PipeT.yield<M, PString, OUT>(r)
        select unit;

    /// <summary>
    /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
    /// The value is only forwarded if the parsing succeeds
    /// </summary>
    public static PipeT<PString<IN>, OUT, M, Unit> ToPipeT<M, IN, OUT>(this Parser<IN, OUT> ma) 
        where M : MonadIO<M> =>
        from t in PipeT.awaiting<M, PString<IN>, OUT>()
        from _ in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => PipeT.yield<M, PString<IN>, OUT>(x),
                      Either.Left<string, OUT>          => Pure<Unit>(default),
                      _                                 => throw new NotSupportedException()
                  }
        select unit;
    

    /// <summary>
    /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
    /// If the parser fails then the pipe fails
    /// </summary>
    public static Pipe<RT, PString, OUT, Unit> ToPipe<RT, OUT>(this Parser<OUT> ma) =>
        from t in Pipe.awaiting<RT, PString, OUT>()
        from r in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => IO.pure(x),
                      Either.Left<string, OUT> (var e)  => IO.fail<OUT>(Errors.ParseError(e)),
                      _                                 => throw new NotSupportedException()
                  }
        from _ in Pipe.yield<RT, PString, OUT>(r)
        select unit;

    /// <summary>
    /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
    /// The value is only forwarded if the parsing succeeds
    /// </summary>
    public static Pipe<RT, PString<IN>, OUT, Unit> ToPipe<RT, IN, OUT>(this Parser<IN, OUT> ma) =>
        from t in Pipe.awaiting<RT, PString<IN>, OUT>()
        from _ in ma.Parse(t).ToEither() switch
                  {
                      Either.Right<string, OUT> (var x) => Pipe.yield<RT, PString<IN>, OUT>(x),
                      Either.Left<string, OUT>          => Pure<Unit>(default),
                      _                                 => throw new NotSupportedException()
                  }
        select unit;    
}
