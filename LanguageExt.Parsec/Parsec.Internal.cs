using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Common;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.ParserResult;

namespace LanguageExt.Parsec
{
    static class Internal
    {
        public static ParserResult<char> newstate(PString inp)
        {
            var x = inp.Value[inp.Index];

            var newpos = x == '\n' ? new Pos(inp.Pos.Line + 1, 0)
                       : x == '\t' ? new Pos(inp.Pos.Line, ((inp.Pos.Column / 4) + 1) * 4)
                       : new Pos(inp.Pos.Line, inp.Pos.Column + 1);

            return ConsumedOK(x,
                new PString(
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
        public static Parser<T> choicei<T>(Seq<Parser<T>> ps) =>
            ps.IsEmpty
                ? unexpected<T>("choice parser with no choices")
                : inp =>
            {
                List<T> results = new List<T>();
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
                        return EmptyOK(t.Reply.Result, t.Reply.State, mergeError(error, t.Reply.Error));
                    }

                    // cerr
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                    {
                        return ConsumedError<T>(mergeError(error, t.Reply.Error));
                    }

                    error = mergeError(error, t.Reply.Error);
                }

                return EmptyError<T>(error);
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
        public static Parser<Seq<T>> chaini<T>(Seq<Parser<T>> ps) =>
            ps.IsEmpty
                ? unexpected<Seq<T>>("chain parser with 0 items")
                : inp =>
            {
                if( ps.Count == 1)
                {
                    return ps.Head.Map(x => x.Cons())(inp);
                }

                var current = inp;
                List<T> results = new List<T>();
                ParserError error = null;
                ParserResult<T> last = null;
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
                            return ConsumedError<Seq<T>>(t.Reply.Error);
                        }
                        // eerr
                        else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                        {
                            return EmptyError<Seq<T>>(t.Reply.Error);
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
                                return ConsumedError<Seq<T>>(t.Reply.Error);
                            }
                            // cok, eerr
                            else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                            {
                                return ConsumedError<Seq<T>>(mergeError(error, t.Reply.Error));
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
                                return ConsumedError<Seq<T>>(t.Reply.Error);
                            }
                            // eok, eerr
                            else if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.Error)
                            {
                                return EmptyError<Seq<T>>(mergeError(error, t.Reply.Error));
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


        public static Parser<Seq<T>> counti<T>(int n, Parser<T> p) =>
           n <= 0
                ? result(Seq<T>.Empty)
                : from x in p
                  from y in counti(n-1, p)
                  select x.Cons(y);

    }
}
