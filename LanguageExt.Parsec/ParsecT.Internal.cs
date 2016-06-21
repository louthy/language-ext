using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Parsec.Common;
using static LanguageExt.Parsec.PrimT;
using static LanguageExt.Parsec.ParserResultT;

namespace LanguageExt.Parsec
{
    static class InternalT
    {
        public static ParserResult<I,I> newstate<I>(PString<I> inp)
        {
            var x = inp.Value[inp.Index];

            var newpos = new Pos(inp.Pos.Line, inp.Pos.Column + 1);

            return ConsumedOK(x,
                new PString<I>(
                    inp.Value,
                    inp.Index + 1,
                    inp.EndIndex,
                    newpos,
                    inp.DefPos,
                    onside(newpos, inp.DefPos)
                        ? Sidedness.Onside
                        : Sidedness.Offside,
                    inp.UserState));
        }

        /// <summary>
        /// Imperative implementation of the choice parser, which in a non-stack 
        /// overflow utopia would look similar to this:
        /// 
        ///     either(ps[index], choicei(ps, index + 1))
        /// 
        /// </summary>
        public static Parser<I,O> choicei<I, O>(Parser<I, O>[] ps) =>
            ps.Length == 0
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
                        return ConsumedError<I, O>(mergeError(error, t.Reply.Error));
                    }

                    error = mergeError(error, t.Reply.Error);
                }

                return EmptyError<I, O>(error);
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
        public static Parser<I, IEnumerable<O>> chaini<I, O>(Parser<I, O>[] ps) =>
            ps.Length == 0
                ? unexpected<I, IEnumerable<O>>("chain parser with 0 items")
                : inp =>
            {
                if( ps.Length == 1)
                {
                    return ps[0].Map(x => new[] { x }.AsEnumerable())(inp);
                }

                var current = inp;
                var results = new List<O>();
                ParserError error = null;
                ParserResult<I,O> last = null;
                int count = ps.Length;

                foreach (var p in ps)
                {
                    count--;
                    var t = p(current);

                    if( last == null)
                    {
                        // cerr
                        if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                        {
                            return ConsumedError<I, IEnumerable<O>>(t.Reply.Error);
                        }
                        // eerr
                        else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                        {
                            return EmptyError<I, IEnumerable<O>>(t.Reply.Error);
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
                                return ConsumedError<I, IEnumerable<O>>(t.Reply.Error);
                            }
                            // cok, eerr
                            else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                            {
                                return ConsumedError<I, IEnumerable<O>>(mergeError(error, t.Reply.Error));
                            }
                            // cok, cok
                            else if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                            {
                                if (count == 0)
                                {
                                    results.Add(t.Reply.Result);
                                    return ConsumedOK<I, IEnumerable<O>>(results, t.Reply.State, t.Reply.Error);
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
                                    return ConsumedOK<I, IEnumerable<O>>(results, t.Reply.State, mergeError(error, t.Reply.Error));
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
                                return ConsumedError<I, IEnumerable<O>>(t.Reply.Error);
                            }
                            // eok, eerr
                            else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                            {
                                return EmptyError<I, IEnumerable<O>>(mergeError(error, t.Reply.Error));
                            }
                            // eok, cok
                            else if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                            {
                                if (count == 0)
                                {
                                    results.Add(t.Reply.Result);
                                    return ConsumedOK<I, IEnumerable<O>>(results, t.Reply.State, t.Reply.Error);
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
                                    return EmptyOK<I, IEnumerable<O>>(results, t.Reply.State, mergeError(error, t.Reply.Error));
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
                return ConsumedOK<I, IEnumerable<O>>(results, current, error);
            };


        public static Parser<I, IEnumerable<O>> counti<I, O>(int n, Parser<I, O> p) =>
           n <= 0
                ? result<I, IEnumerable<O>>(new O [0].AsEnumerable())
                : from x in p
                  from y in counti(n-1, p)
                  select x.Cons(y);

    }
}
