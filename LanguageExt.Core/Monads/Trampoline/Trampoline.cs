using System;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class Trampoline
{
    public static Trampoline<A> Pure<A>(A value) =>
        new Trampoline<A>.PureStep(value);
    
    public static Trampoline<A> More<A>(Func<Trampoline<A>> value) =>
        new Trampoline<A>.MoreStep(value);

    public static Trampoline<B> Bind<A, B>(Trampoline<A> ma, Func<A, Trampoline<B>> f) =>
        ma switch
        {
            Trampoline<A>.BindStep self => self.Fix(f),
            _                       => new Trampoline<B>.BindStep<A>(ma, f)
        };
}

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Based on: https://blog.higher-order.com/assets/trampolines.pdf
/// </remarks>
/// <typeparam name="A"></typeparam>
public abstract record Trampoline<A>
{
    public A Run()
    {
        var f = () => this;
        while (true)
        {
            switch (f().Resume())
            {
                case Either.Left<Func<Trampoline<A>>, A> (var nf):
                    f = nf;
                    break;

                case Either.Right<Func<Trampoline<A>>, A> (var value):
                    return value;
            }
        }
    }
    
    public Trampoline<B> Bind<B>(Func<A, Trampoline<B>> f) =>
        Trampoline.Bind(this, f);
    
    public Trampoline<B> Map<B>(Func<A, B> f) =>
        Trampoline.Bind(this, x => Trampoline.Pure(f(x)));
    
    public Trampoline<B> Select<B>(Func<A, B> f) =>
        Trampoline.Bind(this, x => Trampoline.Pure(f(x)));

    public Trampoline<C> SelectMany<B, C>(Func<A, Trampoline<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    protected abstract Either<Func<Trampoline<A>>, A> Resume();

    internal record PureStep(A Result) : Trampoline<A>
    {
        protected override Either<Func<Trampoline<A>>, A> Resume() =>
            Result;
    }

    internal record MoreStep(Func<Trampoline<A>> Next) : Trampoline<A>
    {
        protected override Either<Func<Trampoline<A>>, A> Resume() =>
            Next;
    }

    internal abstract record BindStep : Trampoline<A>
    {
        public abstract Trampoline<B> Fix<B>(Func<A, Trampoline<B>> f);
    }

    internal record BindStep<X>(Trampoline<X> Sub, Func<X, Trampoline<A>> Next) : BindStep
    {
        public override Trampoline<B> Fix<B>(Func<A, Trampoline<B>> f) =>
            new Trampoline<B>.BindStep<X>(Sub, x => Trampoline.Bind(Next(x), f));

        protected override Either<Func<Trampoline<A>>, A> Resume() =>
            Sub switch
            {
                Trampoline<X>.PureStep (var x) =>
                    Next(x).Resume(),

                Trampoline<X>.MoreStep (var f) =>
                    Left(() => Trampoline.Bind(f(), Next)),

                Trampoline<X>.BindStep bind =>
                    bind.Resume() switch
                    {
                        Either.Right<Func<Trampoline<X>>, X> (var x) => Left(() => Next(x)),
                        Either.Left<Func<Trampoline<X>>, X> (var f)  => Left(() => Trampoline.Bind(f(), Next)),
                        _                                            => throw new NotSupportedException()
                    },

                _ => throw new NotSupportedException()
            };
    }
}
