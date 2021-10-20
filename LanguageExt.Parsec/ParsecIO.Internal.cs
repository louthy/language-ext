using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Common;
using static LanguageExt.Parsec.PrimIO;
using static LanguageExt.Parsec.ParserResultIO;

namespace LanguageExt.Parsec
{
    static class InternalIO
    {
        public static ParserResult<I, I> newstate<I>(PString<I> inp)
        {
            var x = inp.Value[inp.Index];

            return ConsumedOK(x,
                new PString<I>(
                    inp.Value,
                    inp.Index + 1,
                    inp.EndIndex,
                    inp.UserState,
                    inp.TokenPos));
        }

        /// <summary>
        /// Imperative implementation of the choice parser, which in a non-stack 
        /// overflow utopia would look similar to this:
        /// 
        ///     either(ps[index], choicei(ps, index + 1))
        /// 
        /// </summary>
        public static Parser<I,O> choicei<I, O>(Seq<Parser<I, O>> ps) =>
            ps.IsEmpty
                ? unexpected<I, O>("choice parser with no choices")
                : inp =>
            {
                var results = new List<O>();
                ParserError error = null;

                foreach (var p in ps)
                {
                    var t = p(inp);

                    // cok
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                    {
                        return t;
                    }

                    // eok
                    if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                    {
                        return EmptyOK<I, O>(t.Reply.Result, t.Reply.State, mergeError(error, t.Reply.Error));
                    }

                    // cerr
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                    {
                        return ConsumedError<I, O>(mergeError(error, t.Reply.Error), inp.TokenPos);
                    }

                    error = mergeError(error, t.Reply.Error);
                }

                return EmptyError<I, O>(error, inp.TokenPos);
            };

        /// <summary>
        /// Imperative implementation of chain, which in a non-stack overflow utopia
        /// would look similar to this:
        /// 
        ///     from x in ps[index]
        ///     from y in chaini(ps, index + 1)
        ///     select x.Cons(y);
        /// 
        /// </summary>
        public static Parser<I, Seq<O>> chaini<I, O>(Seq<Parser<I, O>> ps) =>
            ps.IsEmpty
                ? unexpected<I, Seq<O>>("chain parser with 0 items")
                : inp =>
            {
                if( ps.Count == 1)
                {
                    return ps.Head.Map(x => x.Cons())(inp);
                }

                var current = inp;
                var results = new List<O>();
                ParserError error = null;
                ParserResult<I,O> last = null;
                int count = ps.Count;

                foreach (var p in ps)
                {
                    count--;
                    var t = p(current);

                    if( last == null)
                    {
                        // cerr
                        if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                        {
                            return ConsumedError<I, Seq<O>>(t.Reply.Error, inp.TokenPos);
                        }
                        // eerr
                        else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                        {
                            return EmptyError<I, Seq<O>>(t.Reply.Error, inp.TokenPos);
                        }
                        // cok
                        else if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                        {
                            results.Add(t.Reply.Result);
                            last = t;
                            error = t.Reply.Error;
                            current = t.Reply.State;
                        }
                        // eok
                        else //if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                        {
                            results.Add(t.Reply.Result);
                            last = t;
                            error = t.Reply.Error;
                        }
                    }
                    else
                    {
                        if (last.Tag == ResultTag.Consumed)
                        {
                            // cok, cerr
                            if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                            {
                                return ConsumedError<I, Seq<O>>(t.Reply.Error, inp.TokenPos);
                            }
                            // cok, eerr
                            else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                            {
                                return ConsumedError<I, Seq<O>>(mergeError(error, t.Reply.Error), inp.TokenPos);
                            }
                            // cok, cok
                            else if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                            {
                                if (count == 0)
                                {
                                    results.Add(t.Reply.Result);
                                    return ConsumedOK(toSeq(results), t.Reply.State, t.Reply.Error);
                                }
                                else
                                {
                                    results.Add(t.Reply.Result);
                                    last = t;
                                    error = t.Reply.Error;
                                    current = t.Reply.State;
                                }
                            }
                            // cok, eok
                            else //if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                            {
                                if (count == 0)
                                {
                                    // cok, eok -> cok  (not a typo, this should be -> cok)
                                    results.Add(t.Reply.Result);
                                    return ConsumedOK(toSeq(results), t.Reply.State, mergeError(error, t.Reply.Error));
                                }
                                else
                                {
                                    results.Add(t.Reply.Result);
                                    last = t;
                                    error = mergeError(error, t.Reply.Error);
                                }
                            }
                        }
                        else if (last.Tag == ResultTag.Empty)
                        {
                            // eok, cerr
                            if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                            {
                                return ConsumedError<I, Seq<O>>(t.Reply.Error, inp.TokenPos);
                            }
                            // eok, eerr
                            else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                            {
                                return EmptyError<I, Seq<O>>(mergeError(error, t.Reply.Error), inp.TokenPos);
                            }
                            // eok, cok
                            else if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                            {
                                if (count == 0)
                                {
                                    results.Add(t.Reply.Result);
                                    return ConsumedOK(toSeq(results), t.Reply.State, t.Reply.Error);
                                }
                                else
                                {
                                    results.Add(t.Reply.Result);
                                    last = t;
                                    error = t.Reply.Error;
                                    current = t.Reply.State;
                                }
                            }
                            // eok, eok
                            else //if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                            {
                                if (count == 0)
                                {
                                    results.Add(t.Reply.Result);
                                    return EmptyOK(toSeq(results), t.Reply.State, mergeError(error, t.Reply.Error));
                                }
                                else
                                {
                                    results.Add(t.Reply.Result);
                                    last = t;
                                    error = mergeError(error, t.Reply.Error);
                                }
                            }
                        }
                    }
                }
                return ConsumedOK(toSeq(results), current, error);
            };


        public static Parser<I, Seq<O>> counti<I, O>(int n, Parser<I, O> p) =>
           n <= 0
                ? result<I, Seq<O>>(Seq<O>.Empty)
                : from x in p
                  from y in counti(n-1, p)
                  select x.Cons(y);

    }
}
