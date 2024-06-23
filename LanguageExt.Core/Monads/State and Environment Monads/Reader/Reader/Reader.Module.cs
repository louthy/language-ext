using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Reader
{
    public static Reader<Env, B> bind<Env, A, B>(Reader<Env, A> ma, Func<A, Reader<Env, B>> f) => 
        ma.As().Bind(f);

    public static Reader<Env, B> map<Env, A, B>(Func<A, B> f, Reader<Env, A> ma) =>  
        ma.As().Map(f);

    public static Reader<Env, A> Pure<Env, A>(A value) =>  
        Reader<Env, A>.Pure(value);

    public static Reader<Env, B> apply<Env, A, B>(Reader<Env, Func<A, B>> mf, Reader<Env, A> ma) =>  
        mf.As().Bind(ma.As().Map);

    public static Reader<Env, B> action<Env, A, B>(Reader<Env, A> ma, Reader<Env, B> mb) => 
        ma.As().Bind(_ => mb);
    
    public static Reader<Env, Env> ask<Env>() => 
        Reader<Env, Env>.Asks(Prelude.identity);

    public static Reader<Env, A> asks<Env, A>(Func<Env, A> f) =>  
        Reader<Env, A>.Asks(f);

    public static Reader<Env, A> asksM<Env, A>(Func<Env, Reader<Env, A>> f) =>
        Reader<Env, A>.AsksM(f);

    public static Reader<Env, A> local<Env, A>(Func<Env, Env> f, Reader<Env, A> ma) => 
        ma.As().Local(f);
}
