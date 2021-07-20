using System;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Pipes
{
    public class Pure<A>
    {
        public readonly A Value;
        public Pure(A value) =>
            Value = value;
    }

    public abstract class Producer<OUT, A>
    {
        public class Pure : Producer<OUT, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;
        }

        public class Yield : Producer<OUT, A>
        {
            public readonly OUT Value;
            public readonly Func<Unit, Producer<OUT, A>> Next;
            
            public Yield(OUT value, Func<Unit, Producer<OUT, A>> next) =>
                (Value, Next) = (value, next);
        }
    }
    
    public abstract class Consumer<IN, A>
    {
        public class Pure : Consumer<IN, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;
        }

        public class Await : Consumer<IN, A>
        {
            public readonly Func<IN, Consumer<IN, A>> Next;
            public Await(Func<IN, Consumer<IN, A>> next) =>
                Next = next;
        }
    }

    public abstract class Pipe<IN, OUT, A>
    {
        public class Pure : Pipe<IN, OUT, A> 
        {
            public readonly A Value;
            public Pure(A value) =>
                Value = value;
        }

        public class Await : Pipe<IN, OUT, A>
        {
            public readonly Func<IN, Pipe<IN, OUT, A>> Next;
            public Await(Func<IN, Pipe<IN, OUT, A>> next) =>
                Next = next;
        }

        public class Yield : Pipe<IN, OUT, A>
        {
            public readonly OUT Value;
            public readonly Func<Unit, Pipe<IN, OUT, A>> Next;
            
            public Yield(OUT value, Func<Unit, Pipe<IN, OUT, A>> next) =>
                (Value, Next) = (value, next);
        }
    }

    public static class PureProxy
    {
        public static Pure<A> Pure<A>(A value) =>
            new Pure<A>(value);

        public static Pipe<IN, OUT, A> PipePure<IN, OUT, A>(A value) =>
            new Pipe<IN, OUT, A>.Pure(value);

        public static Producer<OUT, A> ProducerPure<OUT, A>(A value) =>
            new Producer<OUT, A>.Pure(value);

        public static Consumer<IN, A> ConsumerPure<IN, A>(A value) =>
            new Consumer<IN, A>.Pure(value);

        public static Consumer<IN, IN> ConsumerAwait<IN>() =>
            new Consumer<IN, IN>.Await(ConsumerPure<IN, IN>);

        public static Pipe<IN, OUT, IN> PipeAwait<IN, OUT>() =>
            new Pipe<IN, OUT, IN>.Await(PipePure<IN, OUT, IN>);

        public static Producer<OUT, Unit> ProducerYield<OUT>(OUT value) =>
            new Producer<OUT, Unit>.Yield(value, ProducerPure<OUT, Unit>);

        public static Pipe<IN, OUT, Unit> PipeYield<IN, OUT>(OUT value) =>
            new Pipe<IN, OUT, Unit>.Yield(value, PipePure<IN, OUT, Unit>);

        public static Pipe<IN, OUT, B> Select<IN, OUT, A, B>(this Pipe<IN, OUT, A> ma, Func<A, B> f) =>
            ma switch
            {
                Pipe<IN, OUT, A>.Pure v  => PipePure<IN, OUT, B>(f(v.Value)),
                Pipe<IN, OUT, A>.Await v => new Pipe<IN, OUT, B>.Await(n => v.Next(n).Select(f)),
                Pipe<IN, OUT, A>.Yield v => new Pipe<IN, OUT, B>.Yield(v.Value, n => v.Next(n).Select(f)),
                _                           => throw new System.InvalidOperationException()
            };

        public static Producer<OUT, B> Select<OUT, A, B>(this Producer<OUT, A> ma, Func<A, B> f) =>
            ma switch
            {
                Producer<OUT, A>.Pure v  => ProducerPure<OUT, B>(f(v.Value)),
                Producer<OUT, A>.Yield v => new Producer<OUT, B>.Yield(v.Value, n => v.Next(n).Select(f)),
                _                           => throw new System.InvalidOperationException()
            };

        public static Consumer<IN, B> Select<IN, A, B>(this Consumer<IN, A> ma, Func<A, B> f) =>
            ma switch
            {
                Consumer<IN, A>.Pure v  => ConsumerPure<IN, B>(f(v.Value)),
                Consumer<IN, A>.Await v => new Consumer<IN, B>.Await(n => v.Next(n).Select(f)),
                _                          => throw new System.InvalidOperationException()
            };

        public static Pipe<IN, OUT, B> SelectMany<IN, OUT, A, B>(this Pipe<IN, OUT, A> ma, Func<A, Pipe<IN, OUT, B>> f) =>
            ma switch
            {
                Pipe<IN, OUT, A>.Pure v  => f(v.Value),
                Pipe<IN, OUT, A>.Await v => new Pipe<IN, OUT, B>.Await(n => v.Next(n).SelectMany(f)),
                Pipe<IN, OUT, A>.Yield v => new Pipe<IN, OUT, B>.Yield(v.Value, n => v.Next(n).SelectMany(f)),
                _                           => throw new System.InvalidOperationException()
            };

        public static Pipe<IN, OUT, C> SelectMany<IN, OUT, A, B, C>(this Pipe<IN, OUT, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));

        public static Producer<OUT, B> SelectMany<OUT, A, B>(this Producer<OUT, A> ma, Func<A, Producer<OUT, B>> f) =>
            ma switch
            {
                Producer<OUT, A>.Pure v  => f(v.Value),
                Producer<OUT, A>.Yield v => new Producer<OUT, B>.Yield(v.Value, n => v.Next(n).SelectMany(f)),
                _                           => throw new System.InvalidOperationException()
            };
                
        public static Producer<OUT, C> SelectMany<OUT, A, B, C>(this Producer<OUT, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));

        public static Consumer<IN, B> SelectMany<IN, A, B>(this Consumer<IN, A> ma, Func<A, Consumer<IN, B>> f) =>
            ma switch
            {
                Consumer<IN, A>.Pure v  => f(v.Value),
                Consumer<IN, A>.Await v => new Consumer<IN, B>.Await(n => v.Next(n).SelectMany(f)),
                _                          => throw new System.InvalidOperationException()
            };
                
        public static Consumer<IN, C> SelectMany<IN, A, B, C>(this Consumer<IN, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));

        public static Pipe<IN, OUT, B> SelectMany<IN, OUT, A, B>(this Producer<OUT, A> ma, Func<A, Pipe<IN, OUT, B>> f) =>
            ma switch
            {
                Producer<OUT, A>.Pure v  => f(v.Value),
                Producer<OUT, A>.Yield v => new Pipe<IN, OUT, B>.Yield(v.Value, n => v.Next(n).SelectMany(f)),
                _                           => throw new System.InvalidOperationException()
            };
                
        public static Pipe<IN, OUT, C> SelectMany<IN, OUT, A, B, C>(this Producer<OUT, A> ma, Func<A, Pipe<IN, OUT, B>> f, Func<A, B, C> project) =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));

        public static Pipe<IN, OUT, B> SelectMany<IN, OUT, A, B>(this Producer<OUT, A> ma, Func<A, Consumer<IN, B>> f)
        {
            return ma switch
                   {
                       Producer<OUT, A>.Pure v  => Go<IN, OUT, B>(f(v.Value)),
                       Producer<OUT, A>.Yield v => new Pipe<IN, OUT, B>.Yield(v.Value, n => v.Next(n).SelectMany(f)),
                       _                           => throw new System.InvalidOperationException()
                   };

            static Pipe<I, O, X> Go<I, O, X>(Consumer<I, X> ma) =>
                ma switch
                {
                    Consumer<I, X>.Pure w  => PipePure<I, O, X>(w.Value),
                    Consumer<I, X>.Await w => new Pipe<I, O, X>.Await(n => Go<I, O, X>(w.Next(n))),
                    _                         => throw new System.InvalidOperationException()
                };
        }
        
        public static Pipe<IN, OUT, C> SelectMany<IN, OUT, A, B, C>(this Producer<OUT, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));

        public static Pipe<IN, OUT, B> SelectMany<IN, OUT, A, B>(this Consumer<IN, A> ma, Func<A, Producer<OUT, B>> f)
        {
            return ma switch
                   {
                       Consumer<IN, A>.Pure v  => Go<IN, OUT, B>(f(v.Value)),
                       Consumer<IN, A>.Await v => new Pipe<IN, OUT, B>.Await(n => v.Next(n).SelectMany(f)),
                       _                       => throw new System.InvalidOperationException()
                   };

            static Pipe<I, O, X> Go<I, O, X>(Producer<O, X> ma) =>
                ma switch
                {
                    Producer<O, X>.Pure w  => PipePure<I, O, X>(w.Value),
                    Producer<O, X>.Yield w => new Pipe<I, O, X>.Yield(w.Value, n => Go<I, O, X>(w.Next(n))),
                    _                      => throw new System.InvalidOperationException()
                };
        }
        
        public static Pipe<IN, OUT, C> SelectMany<IN, OUT, A, B, C>(this Consumer<IN, A> ma, Func<A, Producer<OUT, B>> f, Func<A, B, C> project) =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));        

        public static Pipe<IN, OUT, B> SelectMany<IN, OUT, A, B>(this Pipe<IN, OUT, A> ma, Func<A, Consumer<IN, B>> f)
        {
            return ma switch
                   {
                       Pipe<IN, OUT, A>.Pure v  => Go<IN, OUT, B>(f(v.Value)),
                       Pipe<IN, OUT, A>.Yield v => new Pipe<IN, OUT, B>.Yield(v.Value, n => v.Next(n).SelectMany(f)),
                       _                        => throw new System.InvalidOperationException()
                   };

            static Pipe<I, O, X> Go<I, O, X>(Consumer<I, X> ma) =>
                ma switch
                {
                    Consumer<I, X>.Pure w  => PipePure<I, O, X>(w.Value),
                    Consumer<I, X>.Await w => new Pipe<I, O, X>.Await(n => Go<I, O, X>(w.Next(n))),
                    _                      => throw new System.InvalidOperationException()
                };
        }

        public static Pipe<IN, OUT, C> SelectMany<IN, OUT, A, B, C>(this Pipe<IN, OUT, A> ma, Func<A, Consumer<IN, B>> f, Func<A, B, C> project) =>
            ma.SelectMany(a => f(a).Select(b => project(a, b)));

        public static Pipe<RT, IN, OUT, A> Interpret<RT, IN, OUT, A>(this Pipe<IN, OUT, A> ma) where RT : struct, HasCancel<RT> =>
            ma switch
            {
                Pipe<IN, OUT, A>.Pure v  => Pipe.Pure<RT, IN, OUT, A>(v.Value),
                Pipe<IN, OUT, A>.Await v => from x in Pipe.await<RT, IN, OUT>()
                                            from n in Interpret<RT, IN, OUT, A>(v.Next(x))
                                            select n,
                Pipe<IN, OUT, A>.Yield v => from x in Pipe.yield<RT, IN, OUT>(v.Value)
                                            from n in Interpret<RT, IN, OUT, A>(v.Next(x))
                                            select n,
                _                        => throw new System.InvalidOperationException()
            };

        public static Consumer<RT, IN, A> Interpret<RT, IN, A>(this Consumer<IN, A> ma) where RT : struct, HasCancel<RT> =>
            ma switch
            {
                Consumer<IN, A>.Pure v  => Consumer.Pure<RT, IN, A>(v.Value),
                Consumer<IN, A>.Await v => from x in Consumer.await<RT, IN>()
                                           from n in Interpret<RT, IN, A>(v.Next(x))
                                           select n,
                _                       => throw new System.InvalidOperationException()
            };

        public static Producer<RT, OUT, A> Interpret<RT, OUT, A>(this Producer<OUT, A> ma) where RT : struct, HasCancel<RT> =>
            ma switch
            {
                Producer<OUT, A>.Pure v  => Producer.Pure<RT, OUT, A>(v.Value),
                Producer<OUT, A>.Yield v => from x in Producer.yield<RT, OUT>(v.Value)
                                            from n in Interpret<RT, OUT, A>(v.Next(x))
                                            select n,
                _                        => throw new System.InvalidOperationException()
            };

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, Pipe<IN, OUT, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT, IN, OUT, B>()
            select r;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<RT, IN, OUT, A> ma, Func<A, Pipe<IN, OUT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT, IN, OUT, B>()
            select project(a, r);

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, Producer<OUT, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT, OUT, B>()
            select r;

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<RT, OUT, A> ma, Func<A, Producer<OUT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT, OUT, B>()
            select project(a, r);

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, Consumer<IN, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT, IN, B>()
            select r;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<RT, IN, A> ma, Func<A, Consumer<IN, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma
            from r in bind(a).Interpret<RT, IN, B>()
            select project(a, r);

        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Pipe<IN, OUT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT, IN, OUT, A>()
            from r in bind(a)
            select r;

        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Pipe<IN, OUT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT, IN, OUT, A>()
            from r in bind(a)
            select project(a, r);

        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Producer<OUT, A> ma, Func<A, Producer<RT, OUT, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT, OUT, A>()
            from r in bind(a)
            select r;

        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Producer<OUT, A> ma, Func<A, Producer<RT, OUT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT, OUT, A>()
            from r in bind(a)
            select project(a, r);

        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Consumer<IN, A> ma, Func<A, Consumer<RT, IN, B>> bind) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT, IN, A>()
            from r in bind(a)
            select r;

        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Consumer<IN, A> ma, Func<A, Consumer<RT, IN, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            from a in ma.Interpret<RT, IN, A>()
            from r in bind(a)
            select  project(a, r);
    }
}
