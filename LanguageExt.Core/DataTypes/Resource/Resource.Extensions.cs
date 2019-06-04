
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;


public static class ResourceExtensions
{
    public static A Run<A>(this Resource<A> self)
    {
        var (a, resources, faulted) = self();
        resources.Dispose();
        return a;
    }

    public static TryOption<A> ToTryOption<A>(this Resource<A> self) => TryOption(self.Run);

    public static Try<A> ToTry<A>(this Resource<A> self) => Try(self.Run);

    public static Unit Dispose(this HashMap<IDisposable, bool> self)
    {
        foreach (var (resource, _) in self.Where((_, active) => active))
        {
            resource.Dispose();
        }
        return unit;
    }

    public static Resource<Unit> Release<R>(this Resource<R> ma) where R : IDisposable =>
        default(MResource<R>).Release(ma);

    public static Resource<B> Select<A, B>(this Resource<A> self, Func<A, B> f) =>
        FResource<A, B>.Inst.Map(self, f);

    public static Resource<C> SelectMany<A, B, C>(this Resource<A> self, Func<A, Resource<B>> bind, Func<A, B, C> project) =>
        MResource<A>.Inst.Bind<MResource<C>, Resource<C>, C>(self, a =>
            MResource<B>.Inst.Bind<MResource<C>, Resource<C>, C>(bind(a), b =>
                MResource<C>.Inst.Return(_ => project(a, b))));
}