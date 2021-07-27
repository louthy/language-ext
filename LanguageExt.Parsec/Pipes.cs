using System;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
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
        public static Pipe<A[], PString<A>, Unit> toTokenString<A>(Func<A, Pos> tokenPos) =>
            Proxy.awaiting<A[]>().Bind(xs => Proxy.yield(new PString<A>(xs, 0, xs.Length, None, tokenPos ?? (_ => Pos.Zero))));  
        
        /// <summary>
        /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Pipe<RT, PString, OUT, Unit> ToPipe<RT, OUT>(this Parser<OUT> ma) where RT : struct, HasCancel<RT> =>
            from t in Pipe.awaiting<RT, PString, OUT>()
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(t).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Pipe.yield<RT, PString, OUT>(r)
            select unit;

        /// <summary>
        /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
        /// The value is only forwarded if the parsing succeeds
        /// </summary>
        public static Pipe<PString, OUT, Unit> ToPipe<OUT>(this Parser<OUT> ma) =>
            from t in Proxy.awaiting<PString>()
            from _ in ma.Parse(t).ToEither().Case switch
                      {
                          OUT x    => Proxy.yield(x),
                          string e => Proxy.Pure<Unit>(default),
                          _        => throw new NotSupportedException()
                      }
            select unit;        

        /// <summary>
        /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
        /// The value is only forwarded if the parsing succeeds
        /// </summary>
        public static Pipe<PString<IN>, OUT, Unit> ToPipe<IN, OUT>(this Parser<IN, OUT> ma) =>
            from t in Proxy.awaiting<PString<IN>>()
            from _ in ma.Parse(t).ToEither().Case switch
                      {
                          OUT x    => Proxy.yield(x),
                          string e => Proxy.Pure<Unit>(default),
                          _        => throw new NotSupportedException()
                      }
            select unit;        

        /// <summary>
        /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Pipe<RT, PString<IN>, OUT, Unit> ToPipe<RT, IN, OUT>(this Parser<IN, OUT> ma) where RT : struct, HasCancel<RT> =>
            from t in Pipe.awaiting<RT, PString<IN>, OUT>()
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(t).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Pipe.yield<RT, PString<IN>, OUT>(r)
            select unit;
    }
}
