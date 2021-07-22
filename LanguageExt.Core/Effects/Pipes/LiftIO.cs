using System;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
    public abstract class Lift<RT, A> where RT : struct, HasCancel<RT>
    {
        public abstract Lift<RT, B> Map<B>(Func<A, B> f);
        public abstract Lift<RT, B> Bind<B>(Func<A, Lift<RT, B>> f);
        public abstract Producer<RT, OUT, A> ToProducer<OUT>();
        public abstract Consumer<RT, IN, A> ToConsumer<IN>();
        public abstract ConsumerLift<RT, IN, A> ToConsumerLift<IN>();
        public abstract Pipe<RT, IN, OUT, A> ToPipe<IN, OUT>();
        
        public class Pure : Lift<RT, A>
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;

            public override Lift<RT, B> Map<B>(Func<A, B> f) =>
                new Lift<RT, B>.Pure(f(Value));

            public override Lift<RT, B> Bind<B>(Func<A, Lift<RT, B>> f) =>
                f(Value);

            public override Producer<RT, OUT, A> ToProducer<OUT>() =>
                Producer.Pure<RT, OUT, A>(Value);

            public override Consumer<RT, IN, A> ToConsumer<IN>() =>
                Consumer.Pure<RT, IN, A>(Value);

            public override ConsumerLift<RT, IN, A> ToConsumerLift<IN>() =>
                new ConsumerLift<RT, IN, A>.Pure(Value);

            public override Pipe<RT, IN, OUT, A> ToPipe<IN, OUT>() =>
                Pipe.Pure<RT, IN, OUT, A>(Value);
        }
        
        public class LiftAff<X> : Lift<RT, A>
        {
            public readonly Aff<RT, X> Effect;
            public readonly Func<X, Lift<RT, A>> Next;
            public LiftAff(Aff<RT, X> value, Func<X, Lift<RT, A>> next) =>
                (Effect, Next) = (value, next);

            public override Lift<RT, B> Map<B>(Func<A, B> f) =>
                new Lift<RT, B>.LiftAff<X>(Effect, n => Next(n).Select(f));

            public override Lift<RT, B> Bind<B>(Func<A, Lift<RT, B>> f) =>
                new Lift<RT, B>.LiftAff<X>(Effect, n => Next(n).Bind(f));

            public override ConsumerLift<RT, IN, A> ToConsumerLift<IN>() =>
                new ConsumerLift<RT, IN, A>.Lift<X>(Effect, x => Next(x).ToConsumerLift<IN>());

            public override Producer<RT, OUT, A> ToProducer<OUT>() =>
                from x in Producer.liftIO<RT, OUT, X>(Effect)
                from r in Next(x).ToProducer<OUT>()
                select r;

            public override Consumer<RT, IN, A> ToConsumer<IN>() =>
                from x in Consumer.liftIO<RT, IN, X>(Effect)
                from r in Next(x).ToConsumer<IN>()
                select r;

            public override Pipe<RT, IN, OUT, A> ToPipe<IN, OUT>() =>
                from x in Pipe.liftIO<RT, IN, OUT, X>(Effect)
                from r in Next(x).ToPipe<IN, OUT>()
                select r;
        }
        public class LiftEff<X> : Lift<RT, A>
        {
            public readonly Eff<RT, X> Effect;
            public readonly Func<X, Lift<RT, A>> Next;
            public LiftEff(Eff<RT, X> value, Func<X, Lift<RT, A>> next) =>
                (Effect, Next) = (value, next);

            public override Lift<RT, B> Map<B>(Func<A, B> f) =>
                new Lift<RT, B>.LiftEff<X>(Effect, n => Next(n).Select(f));

            public override Lift<RT, B> Bind<B>(Func<A, Lift<RT, B>> f) =>
                new Lift<RT, B>.LiftEff<X>(Effect, n => Next(n).Bind(f));

            public override ConsumerLift<RT, IN, A> ToConsumerLift<IN>() =>
                new ConsumerLift<RT, IN, A>.Lift<X>(Effect, x => Next(x).ToConsumerLift<IN>());

            public override Producer<RT, OUT, A> ToProducer<OUT>() =>
                from x in Producer.liftIO<RT, OUT, X>(Effect)
                from r in Next(x).ToProducer<OUT>()
                select r;

            public override Consumer<RT, IN, A> ToConsumer<IN>() =>
                from x in Consumer.liftIO<RT, IN, X>(Effect)
                from r in Next(x).ToConsumer<IN>()
                select r;

            public override Pipe<RT, IN, OUT, A> ToPipe<IN, OUT>() =>
                from x in Pipe.liftIO<RT, IN, OUT, X>(Effect)
                from r in Next(x).ToPipe<IN, OUT>()
                select r;
        }
    }

    public static class Lift
    {
        public static Lift<RT, A> Pure<RT, A>(A value) where RT : struct, HasCancel<RT> =>
            new Lift<RT, A>.Pure(value);

        public static Lift<RT, A> Aff<RT, A>(Aff<RT, A> value) where RT : struct, HasCancel<RT> =>
            new Lift<RT, A>.LiftAff<A>(value, Pure<RT, A>);

        public static Lift<RT, A> Eff<RT, A>(Eff<RT, A> value) where RT : struct, HasCancel<RT> =>
            new Lift<RT, A>.LiftEff<A>(value, Pure<RT, A>);

        public static Lift<RT, B> Select<RT, A, B>(this Lift<RT, A> ma, Func<A, B> f) where RT : struct, HasCancel<RT> =>
            ma.Map(f);

        public static Lift<RT, B> SelectMany<RT, A, B>(this Lift<RT, A> ma, Func<A, Lift<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(f);

        public static Lift<RT, C> SelectMany<RT, A, B, C>(this Lift<RT, A> ma, Func<A, Lift<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));

        public static Lift<RT, B> SelectMany<RT, A, B>(this Lift<RT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftAff<B>(f(x), Pure<RT, B>));

        public static Lift<RT, C> SelectMany<RT, A, B, C>(this Lift<RT, A> ma, Func<A, Aff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftAff<B>(f(x), Pure<RT, B>).Map(y => project(x, y)));

        public static Lift<RT, B> SelectMany<RT, A, B>(this Lift<RT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftEff<B>(f(x), Pure<RT, B>));

        public static Lift<RT, C> SelectMany<RT, A, B, C>(this Lift<RT, A> ma, Func<A, Eff<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftEff<B>(f(x), Pure<RT, B>).Map(y => project(x, y)));

        public static Lift<RT, B> SelectMany<RT, A, B>(this Lift<RT, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftAff<B>(f(x), Pure<RT, B>));

        public static Lift<RT, C> SelectMany<RT, A, B, C>(this Lift<RT, A> ma, Func<A, Aff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftAff<B>(f(x), Pure<RT, B>).Map(y => project(x, y)));

        public static Lift<RT, B> SelectMany<RT, A, B>(this Lift<RT, A> ma, Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftEff<B>(f(x), Pure<RT, B>));

        public static Lift<RT, C> SelectMany<RT, A, B, C>(this Lift<RT, A> ma, Func<A, Eff<B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => new Lift<RT, B>.LiftEff<B>(f(x), Pure<RT, B>).Map(y => project(x, y)));

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Lift<RT, A> ma, Func<A, Consumer<IN, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.ToConsumer<IN>()
            from b in f(a)
            select b;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Lift<RT, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.ToConsumer<IN>()
            from b in f(a)
            select project(a, b);

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Lift<RT, A> ma, Func<A, Producer<OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.ToProducer<OUT>()
            from b in f(a)
            select b;

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Lift<RT, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.ToProducer<OUT>()
            from b in f(a)
            select project(a, b);

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Lift<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.ToPipe<IN, OUT>()
            from b in f(a)
            select b;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Lift<RT, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.ToPipe<IN, OUT>()
            from b in f(a)
            select project(a, b);        

        public static ConsumerLift<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<IN, A> ma, Func<A, Lift<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.ToConsumerLift<RT>()
            from b in f(a).ToConsumerLift<IN>()
            select b;

        public static ConsumerLift<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<IN, A> ma, Func<A, Lift<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.ToConsumerLift<RT>()
            from b in f(a).ToConsumerLift<IN>()
            select project(a, b);

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<OUT, A> ma, Func<A, Lift<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToProducer<OUT>()
            select b;

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<OUT, A> ma, Func<A, Lift<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToProducer<OUT>()
            select project(a, b);

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<IN, OUT, A> ma, Func<A, Lift<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToPipe<IN, OUT>()
            select b;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<IN, OUT, A> ma, Func<A, Lift<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToPipe<IN, OUT>()
            select project(a, b);
        

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Lift<RT, A> ma, Func<A, Consumer<RT, IN, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.ToConsumer<IN>()
            from b in f(a)
            select b;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Lift<RT, A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.ToConsumer<IN>()
            from b in f(a)
            select project(a, b);

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Lift<RT, A> ma, Func<A, Producer<RT, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.ToProducer<OUT>()
            from b in f(a)
            select b;

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Lift<RT, A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.ToProducer<OUT>()
            from b in f(a)
            select project(a, b);

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Lift<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma.ToPipe<IN, OUT>()
            from b in f(a)
            select b;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Lift<RT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.ToPipe<IN, OUT>()
            from b in f(a)
            select project(a, b);        

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Lift<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a)
            select b;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Lift<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToConsumer<IN>()
            select project(a, b);

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, Lift<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToProducer<OUT>()
            select b;

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Lift<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToProducer<OUT>()
            select project(a, b);

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Lift<RT, B>> f) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToPipe<IN, OUT>()
            select b;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Lift<RT, B>> f, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from b in f(a).ToPipe<IN, OUT>()
            select project(a, b);          
    }
}
