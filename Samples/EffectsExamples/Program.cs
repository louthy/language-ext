using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Sys.Live;
using LanguageExt;

namespace EffectsExamples;

class Program
{
    static void Main(string[] args) =>
        Menu<Runtime>
           .menu
           .Run(Runtime.New(), EnvIO.New())
           .ThrowIfFail();
}

public interface Selectable<SELF, A>
    where SELF : Selectable<SELF, A>
{
    public static abstract SELF Select<B>(
        SELF list,
        Func<A, B> f);
}

public record MyList<A>(A[] values) : 
    Selectable<MyList<A>, A>
{
    public static MyList<A> Select<B>(
        MyList<A> list, Func<A, B> f) =>
        new(list.values.Select(f).ToArray());
}
