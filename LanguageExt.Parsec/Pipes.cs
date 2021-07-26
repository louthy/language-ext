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
        /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Pipe<RT, string, OUT, Unit> ToPipe<RT, OUT>(this Parser<OUT> ma) where RT : struct, HasCancel<RT> =>
            from t in Pipe.await<RT, string, OUT>()
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(t).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Pipe.yield<RT, string, OUT>(r)
            select unit;

        /// <summary>
        /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Pipe<RT, PString, OUT, Unit> ToPipe2<RT, OUT>(this Parser<OUT> ma) where RT : struct, HasCancel<RT> =>
            from t in Pipe.await<RT, PString, OUT>()
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
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Pipe<RT, Seq<IN>, OUT, Unit> ToPipe<RT, IN, OUT>(this Parser<IN, OUT> ma, Func<IN, Pos> tokenPos) where RT : struct, HasCancel<RT> =>
            from t in Pipe.await<RT, Seq<IN>, OUT>()
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(new PString<IN>(t.ToArray(), 0, t.Count, None, tokenPos ?? (_ => Pos.Zero))).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Pipe.yield<RT, Seq<IN>, OUT>(r)
            select unit;

        /// <summary>
        /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Pipe<RT, PString<IN>, OUT, Unit> ToPipe<RT, IN, OUT>(this Parser<IN, OUT> ma) where RT : struct, HasCancel<RT> =>
            from t in Pipe.await<RT, PString<IN>, OUT>()
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(t).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Pipe.yield<RT, PString<IN>, OUT>(r)
            select unit;
        
        
        /// <summary>
        /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Producer<RT, OUT, Unit> ToProducer<RT, OUT>(this Parser<OUT> ma, string input) where RT : struct, HasCancel<RT> =>
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(input).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Producer.yield<RT, OUT>(r)
            select unit;

        /// <summary>
        /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Producer<RT, OUT, Unit> ToProducer<RT, OUT>(this Parser<OUT> ma, PString input) where RT : struct, HasCancel<RT> =>
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(input).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Producer.yield<RT, OUT>(r)
            select unit;

        /// <summary>
        /// Convert a parser to a pipe that awaits a string and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Producer<RT, OUT, Unit> ToProducer<RT, IN, OUT>(this Parser<IN, OUT> ma, Seq<IN> tokens, Func<IN, Pos> tokenPos) where RT : struct, HasCancel<RT> =>
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(new PString<IN>(tokens.ToArray(), 0, tokens.Count, None, tokenPos ?? (_ => Pos.Zero))).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Producer.yield<RT, OUT>(r)
            select unit;

        /// <summary>
        /// Convert a parser to a pipe that awaits a PString and yields the result of the parse operation
        /// If the parser fails then the pipe fails
        /// </summary>
        public static Producer<RT, OUT, Unit> ToProducer<RT, IN, OUT>(this Parser<IN, OUT> ma, PString<IN> tokens) where RT : struct, HasCancel<RT> =>
            from r in EffMaybe<RT, OUT>(_ => ma.Parse(tokens).ToEither().Case switch
                                             {
                                                 OUT x    => FinSucc(x),
                                                 string e => FinFail<OUT>(Errors.ParseError(e)),
                                                 _        => throw new NotSupportedException()
                                             })
            from _ in Producer.yield<RT, OUT>(r)
            select unit;
                
    }
}
