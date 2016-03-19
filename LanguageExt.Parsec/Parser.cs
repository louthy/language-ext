using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec;

namespace LanguageExt
{
    public delegate ParserResult<T> Parser<T>(PString input);
}

public static class ___ParserExt
{
    public static ParserResult<T> Parse<T>(this Parser<T> self, PString input) =>
        self(input);

    public static ParserResult<T> Parse<T>(this Parser<T> self, string input) =>
        self(PString.Zero.SetValue(input));

    public static Parser<T> Where<T>(this Parser<T> self, Func<T,bool> pred)
    {
        return inp =>
        {
            var t = self(inp);
            if (t.IsFaulted)
            {
                return Failure<T>(t.Errors);
            }
            else
            {
                var results = new List<Tuple<T, PString>>();

                foreach (var result in t.Results)
                {
                    var res = result.Item1;
                    var rem = result.Item2;

                    if (pred(res))
                    {
                        results.Add(Tuple(res, rem));
                    }
                }
                return new ParserResult<T>(results);
            }
        };
    }

    public static Parser<U> Select<T, U>(this Parser<T> self, Func<T, U> map)
    {
        return inp =>
        {
            var t = self(inp);
            if (t.IsFaulted)
            {
                return Failure<U>(t.Errors);
            }
            else
            {
                var results = new List<Tuple<U,PString>>();

                foreach (var result in t.Results)
                {
                    var res = result.Item1;
                    var rem = result.Item2;
                    var u = map(res);

                    results.Add(Tuple(u, rem));
                }
                return new ParserResult<U>(results);
            }
        };
    }

    public static Parser<V> SelectMany<T, U, V>(
        this Parser<T> self, 
        Func<T,Parser<U>> bind,
        Func<T, U, V> project)
    {
        return inp =>
        {
            var t = self(inp);
            if (t.IsFaulted)
            {
                return Failure<V>(t.Errors);
            }
            else
            {
                var results = new List<Tuple<V,PString>>();
                var errors = new List<ParserError>();

                foreach (var tresult in t.Results)
                {
                    var tres = tresult.Item1;
                    var trem = tresult.Item2;

                    var u = bind(tres);
                    var upres = u(trem);

                    if (upres.IsFaulted)
                    {
                        errors.AddRange(upres.Errors);
                    }
                    else
                    {
                        foreach (var uresult in upres.Results)
                        {
                            var ures = uresult.Item1;
                            var urem = uresult.Item2;

                            var v = project(tres, ures);

                            results.Add(Tuple(v, urem));
                        }
                    }
                }

                if (errors.Count > 0)
                {
                    return Failure<V>(errors);
                }
                else
                {
                    return new ParserResult<V>(results);
                }
            }
        };
    }
}
