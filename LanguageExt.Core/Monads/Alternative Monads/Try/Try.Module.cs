using System;
using LanguageExt.Common;

namespace LanguageExt;

public partial class Try
{
    public static Try<A> Succ<A>(A value) => 
        Try<A>.Succ(value);

    public static Try<A> Fail<A>(Error value) => 
        Try<A>.Fail(value);

    public static Try<A> lift<A>(Func<Fin<A>> ma) => 
        Try<A>.Lift(ma);

    public static Try<A> lift<A>(Fin<A> ma) => 
        Try<A>.Lift(ma);

    public static Try<A> lift<A>(Pure<A> ma) => 
        Try<A>.Lift(ma);

    public static Try<A> lift<A>(Fail<Error> ma) => 
        Try<A>.Lift(ma);

    public static Try<A> lift<A>(Error ma) => 
        Try<A>.Lift(ma);

    public static Try<A> lift<A>(Func<A> f) => 
        Try<A>.Lift(f);

    public static Try<Unit> lift(Action f) =>
        Try<Unit>.Lift(
            () =>
            {
                f();
                return default;
            });

    public static Fin<B> match<A, B>(Try<A> ma, Func<A, B> Succ, Func<Error, B> Fail) => 
        ma.Match(Succ, Fail);
}
