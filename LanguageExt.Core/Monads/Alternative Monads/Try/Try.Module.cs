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
    
    public static Try<B> bind<A, B>(Try<A> ma, Func<A, Try<B>> f) => 
        ma.As().Bind(f);

    public static Try<B> map<A, B>(Func<A, B> f, Try<A> ma) =>  
        ma.As().Map(f);

    public static Try<B> apply<A, B>(Try<Func<A, B>> mf, Try<A> ma) => 
        mf.Apply(ma);

    public static Try<B> action<A, B>(Try<A> ma, Try<B> mb) => 
        ma.Action(mb);

    public static Fin<B> match<A, B>(Try<A> ma, Func<A, B> Succ, Func<Error, B> Fail) => 
        ma.Match(Succ, Fail);
}
