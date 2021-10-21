using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type represents a resource using computation.  It is
    /// useful for tidying up resources explicitly (if required).  
    /// </summary>
    /// <remarks>
    /// The `Proxy` system will tidy up resources automatically, however you must wait until the end of the
    /// computation.  This case allows earlier clean up.
    /// </remarks>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public abstract class Use<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal abstract Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps);
    }

    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type represents a resource using computation.  It is
    /// useful for tidying up resources explicitly (if required).  
    /// </summary>
    /// <remarks>
    /// The `Proxy` system will tidy up resources automatically, however you must wait until the end of the
    /// computation.  This case allows earlier clean up.
    /// </remarks>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public class Use<RT, UOut, UIn, DIn, DOut, X, A> : Use<RT, UOut, UIn, DIn, DOut, A> 
        where RT : struct, HasCancel<RT>
    {
        public readonly Func<Aff<RT, X>> Acquire;
        public readonly Func<X, Unit> Release; 
        public readonly Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;

        public Use(Func<Aff<RT, X>> acquire, Func<X, Unit> release, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Acquire, Release, Next) = (acquire, release, next);
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            new Use<RT, UOut, UIn, DIn, DOut, X, B>(Acquire, Release, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f) =>
            new Use<RT, UOut, UIn, DIn, DOut, X, B>(Acquire, Release, x => Next(x).Map(f));
        
        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            new Use<RT, UOut, UIn, C1, C, X, A>(Acquire, Release, x => Next(x).For(body));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> rhs) =>
            new Use<RT, UOut, UIn, DIn, DOut, X, B>(Acquire, Release, x => Next(x).Action(rhs));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs) =>
            new Use<RT, UOutA, AUInA, DIn, DOut, X, A>(Acquire, Release, x => Next(x).ComposeRight(lhs));
        
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Use<RT, UOutA, AUInA, DIn, DOut, X, A>(Acquire, Release, x => Next(x).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Use<RT, UOut, UIn, DInC, DOutC, X, A>(Acquire, Release, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Use<RT, UOut, UIn, DInC, DOutC, X, A>(Acquire, Release, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Use<RT, DOut, DIn, UIn, UOut, X, A>(Acquire, Release, x => Next(x).Reflect());
             
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new Use<RT, UOut, UIn, DIn, DOut, X, A>(Acquire, Release, x => Next(x).Observe());

        [Pure]
        public void Deconstruct(out Func<Aff<RT, X>> acquire, out Func<X, Unit> release, out Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (acquire, release, next) = (Acquire, Release, Next);

        [Pure]
        internal override Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps) =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Acquire().Map(x =>
                              {
                                  disps.TryAdd(x, new Disp(() => Release(x)));
                                  return x;
                              }).Map(Next));
    }    
        
    internal class Disp : IDisposable
    {
        readonly Action dispose;
        int disposed = 0;

        public Disp(Action dispose) =>
            this.dispose = dispose; 
            
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
            {
                dispose?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type represents a resource releasing computation.  It is
    /// useful for tidying up resources that have been used (explicitly).  
    /// </summary>
    /// <remarks>
    /// The `Proxy` system will tidy up resources automatically, however you must wait until the end of the
    /// computation.  This case allows earlier clean up.
    /// </remarks>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public abstract class Release<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal abstract Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps);
    }

    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type represents a resource releasing computation.  It is
    /// useful for tidying up resources that have been used (explicitly).  
    /// </summary>
    /// <remarks>
    /// The `Proxy` system will tidy up resources automatically, however you must wait until the end of the
    /// computation.  This case allows earlier clean up.
    /// </remarks>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public class Release<RT, UOut, UIn, DIn, DOut, X, A> : Release<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly X Value;
        public readonly Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;

        public Release(X value, Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Value, Next) = (value, next);
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Bind<B>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, B>> f) =>
            new Release<RT, UOut, UIn, DIn, DOut, X, B>(Value, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Map<B>(Func<A, B> f) =>
            new Release<RT, UOut, UIn, DIn, DOut, X, B>(Value, x => Next(x).Map(f));
        
        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            new Release<RT, UOut, UIn, C1, C, X, A>(Value, x => Next(x).For(body));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, B> Action<B>(Proxy<RT, UOut, UIn, DIn, DOut, B> rhs) =>
            new Release<RT, UOut, UIn, DIn, DOut, X, B>(Value, x => Next(x).Action(rhs));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs) =>
            new Release<RT, UOutA, AUInA, DIn, DOut, X, A>(Value, x => Next(x).ComposeRight(lhs));
        
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Release<RT, UOutA, AUInA, DIn, DOut, X, A>(Value, x => Next(x).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Release<RT, UOut, UIn, DInC, DOutC, X, A>(Value, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Release<RT, UOut, UIn, DInC, DOutC, X, A>(Value, x => Next(x).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Release<RT, DOut, DIn, UIn, UOut, X, A>(Value, x => Next(x).Reflect());
             
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new Release<RT, UOut, UIn, DIn, DOut, X, A>(Value, x => Next(x).Observe());

        [Pure]
        public void Deconstruct(out X value, out Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (value, next) = (Value, Next);

        [Pure]
        internal override Proxy<RT, UOut, UIn, DIn, DOut, A> Run(ConcurrentDictionary<object, IDisposable> disps) =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Prelude.Eff<RT, Unit>(_ =>
                                      {
                                          if (disps.TryRemove(Value, out var d))
                                          {
                                              d.Dispose();
                                          }
                                          return Prelude.unit;
                                      }).Map(Next));
    }    
}
