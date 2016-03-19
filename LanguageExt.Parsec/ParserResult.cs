using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public enum ParserResultTag
    {
        Success,
        Successes,
        Failure
    }

    public class ParserResult<T>
    {
        readonly IEnumerable<Tuple<T, PString>> results;
        readonly IEnumerable<ParserError> errors;
        public readonly bool IsFaulted;

        public ParserResult(T result, PString remaining)
        {
            this.results = new[] { Tuple(result,remaining) };
            this.IsFaulted = false;
            this.errors = new ParserError[0];
        }

        public ParserResult(IEnumerable<Tuple<T,PString>> results)
        {
            this.results = results.Freeze();
            this.IsFaulted = results.Count() == 0;
            this.errors = new ParserError[0];
        }

        public ParserResult(IEnumerable<ParserError> errors)
        {
            this.IsFaulted = true;
            this.results = new Tuple<T, PString>[0];
            this.errors = errors.Freeze();
        }

        public Tuple<T, PString> Head =>
            IsFaulted
                ? raise<Tuple<T, PString>>(new ParserException("Parser result is failed.  Can't access result."))
                : results.Head();

        public IEnumerable<Tuple<T, PString>> Tail =>
            IsFaulted
                ? raise<IEnumerable<Tuple<T, PString>>>(new ParserException("Parser result is failed.  Can't access result."))
                : results.Tail();

        public IEnumerable<Tuple<T,PString>> Results =>
            IsFaulted
                ? new Tuple<T,PString>[0]
                : results;

        public T Result =>
            IsFaulted
                ? raise<T>(new ParserException("Parser result is failed.  Can't access result."))
                : Head.Item1;

        public PString Remaining =>
            IsFaulted
                ? raise<PString>(new ParserException("Parser result is failed.  Can't access result."))
                : Head.Item2;

        public IEnumerable<ParserError> Errors =>
            IsFaulted
                ? errors
                : new ParserError[0];

        public R MatchOne<R>(
            Func<T, PString, R> Succ,
            Func<IEnumerable<ParserError>, R> Fail
            ) =>
            IsFaulted
                ? Fail(errors)
                : Succ(Head.Item1, Head.Item2);

        public R MatchMany<R>(
            Func<IEnumerable<Tuple<T,PString>>, R> Succ,
            Func<IEnumerable<ParserError>, R> Fail
            ) =>
            IsFaulted
                ? Fail(errors)
                : Succ(results);
    }
}
