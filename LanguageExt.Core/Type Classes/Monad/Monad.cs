using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="MA">The data-type to make monadic</typeparam>
    /// <typeparam name="A">The data-type bound value</typeparam>
    [Typeclass("M*")]
    public interface Monad<MA, A> : 
        Monad<Unit, Unit, MA, A>, 
        Foldable<MA, A>,
        Typeclass
    {
        /// <summary>
        /// Monad constructor function.  Provide the bound value A to construct
        /// a new monad of type MA.
        /// </summary>
        /// <param name="x">Value to bind</param>
        /// <returns>Monad of type MA</returns>
        MA Return(A x);
    }

    /// <summary>
    /// Monad type-class
    /// </summary>
    /// <typeparam name="Env">The input type to the monadic computation</typeparam>
    /// <typeparam name="Out">The output type of the monadic computation</typeparam>
    /// <typeparam name="MA">The data-type to make monadic</typeparam>
    /// <typeparam name="A">The data-type bound value</typeparam>
    [Typeclass("M*")]
    public interface Monad<Env, Out, MA, A> : 
        Foldable<Env, MA, A>,
        Typeclass
    {
        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="MONADB">Type-class of the return value</typeparam>
        /// <typeparam name="MB">Type of the monad to return</typeparam>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of type `MB` derived from Monad of `B`</returns>
        [Pure]
        MB Bind<MONADB, MB, B>(MA ma, Func<A, MB> f) 
            where MONADB : struct, Monad<Env, Out, MB, B>;

        [Pure]
        MB BindAsync<MonadB, MB, B>(MA ma, Func<A, MB> f) 
            where MonadB : struct, MonadAsync<Env, Out, MB, B>;

        /// <summary>
        /// Lazy monad constructor function.  Provide the bound value `A` to construct 
        /// a new monad of type `MA`.  This varies from the 'standard' construction
        /// of monadic types in that it takes an input parameter.  This function allows
        /// monads that take parameters (like `Reader` and `State`) to be unified with 
        /// non-parametric monads like (`Option`, `Either`, etc.), which take `Unit` as their
        /// input argument.
        /// </summary>
        /// <remarks>
        /// Any instance of this interface must be a `struct` to use the `Return` function 
        /// effectively. Then the instance can be used as a generic argument constrained 
        /// to:
        /// 
        ///     where MonadA : struct, Monad<Env, Out, MA, A>
        /// 
        /// And any consumer of the argument can call:
        ///
        ///     MA monad = default(MonadA).Return(a);
        ///  
        /// To construct a monad of type `MA`.  
        /// </remarks>
        /// <param name="x">Value to bind</param>
        /// <returns>Monad of type `MA`</returns>
        MA Return(Func<Env, A> f);

        /// <summary>
        /// Used for double dispatch by the bind function for monadic types that
        /// need to construct an output value/state (like `MState` and `MWriter`).  For
        /// all other monad types just return `mb`.
        /// </summary>
        /// <remarks>
        /// This is an example from the `Writer` monad.  Note how the `Output` argument for the `Writer`
        /// monad is `(W, bool)`.  `W` is what the `Writer` tells, and `bool` is a flag stating whether it's
        /// faulted or not.  The `Bind` function isn't able to combine the output from `ma` and `mb`, due 
        /// to limitations in the type-system's ability to explicitly constrain the return type of 
        /// `Bind` to be a `Writer` monad; the returned monad of `MB` could be any monad, but it must be 
        /// compatible with the input type of `Unit` and the output type of `(W, bool)`.  It restricts 
        /// `MB` to be:
        /// 
        ///     Monad<Unit, (W, bool), MB, B>
        /// 
        /// So, the Writer's `Bind` function calls `BindReturn` which double dispatches the job to the 
        /// `BindReturn` function on `MONADB`.  This is very much like the Visitor pattern in OO land. 
        /// 
        ///     public MB Bind<MONADB, MB, B>(Writer<MonoidW, W, A> ma, Func<A, MB> f) 
        ///         where MONADB : struct, Monad<Unit, (W, bool), MB, B> =>
        ///             default(MONADB).Id(_ =>
        ///             {
        ///                 var(a, output1, faulted) = ma();
        ///                 return faulted
        ///                     ? default(MONADB).Fail()
        ///                     : default(MONADB).BindReturn((output1, faulted), f(a));
        ///             });
        /// 
        /// Usually `MONADB` would be another `Writer` instance, because you would normally bind a `Writer`
        /// with a `Writer`, but it could be any monad that has the same input and output arguments.
        /// The `BindReturn` function is then able to invoke `mb`, because it knows its own context and
        /// combine the output from `ma()` and the output of `mb`.
        /// 
        ///     public Writer<MonoidW, W, A> BindReturn((W, bool) output, Writer<MonoidW, W, A> mb)
        ///     {
        ///         var (b, output2, faulted) = mb();
        ///         return () => faulted
        ///             ? (default(A), default(MonoidW).Empty(), true)
        ///             : (b, default(MonoidW).Append(output.Item1, output2), false);
        ///     }
        ///     
        /// The effect of this with the monadic types in Language-Ext is that Writers are only bindable
        /// to Writers.  However simpler monads like `Option` can be bound to `Either`, `Try`, etc.  Because
        /// their `BindReturn` function looks like this:
        /// 
        ///     public Option<A> BindReturn(Unit _, Option<A> mb) =>
        ///         mb;
        /// 
        /// The `Bind` function for `Option` doesn't call `BindReturn` at all:
        /// 
        ///     public MB Bind<MONADB, MB, B>(Option<A> ma, Func<A, MB> f) 
        ///         where MONADB : struct, Monad<Unit, Unit, MB, B> =>
        ///             ma.IsSome && f != null
        ///                 ? f(ma.Value)
        ///                 : default(MONADB).Fail();
        /// 
        /// So why implement it?  If someone tries to return an `Option` from a `Bind` call with the
        /// source monad of another type, it may call `BindReturn`.  And the `Option` response would be
        /// to just return itself.
        /// 
        /// So `Bind` and `BindReturn` should be seen as two halves of the same function.  They're there
        /// to make use of the instances knowledge about itself, but not its generic return types.
        /// </remarks>
        /// <param name="outputma">Output from the first part of a monadic bind</param>
        /// <param name="mb">Monadic to invoke.  Get the results from this to combine with
        /// `outputma` and then re-wrap</param>
        /// <returns>Monad with the combined output</returns>
        MA BindReturn(Out outputma, MA mb);

        /// <summary>
        /// The `Run` function allows the `Bind` function to construct a monad from a function rather
        /// than `MA`.  It's a form of double-dispatch like the `BindReturn` function.  It hands context
        /// to the type that knows how to construct.  This facilitates the unification of Monads that 
        /// take arguments (like `Reader`, `State`, etc.) with ones that don't (`Option`, `Try`, `Writer`, `Lst`, 
        /// `Either`, etc.)
        /// </summary>
        /// <remarks>
        /// For monads that don't take arguments, they will have an input type of `Unit`.  And so
        /// implementing `Run` is as simple as (for `Option<A>`):
        /// 
        ///     public Option<A> Run(Func<Unit, Option<A>> ma) =>
        ///         ma(unit);
        /// 
        /// The most complex example is the `State` monad.  It takes a type `S` which is the input state:
        /// 
        ///     public State<S, A> Run(Func<S, State<S, A>> ma) => 
        ///         state => ma(state)(state);
        /// 
        /// That appears to be ignoring the return state of `ma(state)`, but if you look at the `Bind` and
        /// `BindReturn` functions for `MState`:
        /// 
        ///     public MB Bind<MONADB, MB, B>(State<S, A> ma, Func<A, MB> f) 
        ///         where MONADB : struct, Monad<S, (S State, bool IsFaulted), MB, B> =>
        ///             default(MONADB).Run(state =>
        ///             {
        ///                 var (a, sa, faulted) = ma(state);
        ///                 return faulted
        ///                     ? default(MONADB).Fail()
        ///                     : default(MONADB).BindReturn((sa, faulted), f(a));
        ///             });
        ///
        ///     public State<S, A> BindReturn((S State, bool IsFaulted) output, State<S, A> mb) =>
        ///         _ => mb(output.State);
        ///  
        /// It should be clear that `Run` accepts the state (the first `state` in `ma(state)(state)`), and
        /// it's result is the return value of `BindReturn` which ignores its incoming state so that it
        /// can bind the output of the call to `ma(state)` in the `Bind` function.
        /// 
        /// Simple monads that don't take parameters simply ignore this in thier `Bind` functions:
        /// 
        ///     public MB Bind<MONADB, MB, B>(Option<A> ma, Func<A, MB> f) 
        ///         where MONADB : struct, Monad<Unit, Unit, MB, B> =>
        ///             ma.IsSome && f != null
        ///                 ? f(ma.Value)
        ///                 : default(MONADB).Fail();
        /// 
        /// The `Run` function would allow two monads of different types to be bound as long as their input
        /// and output types are the same.
        /// </remarks>
        MA Run(Func<Env, MA> ma);

        /// <summary>
        /// Produce a monad of `MA` in it's failed state
        /// </summary>
        [Pure]
        MA Fail(object err = null);

        /// <summary>
        /// Associative binary operation
        /// </summary>
        [Pure]
        MA Plus(MA ma, MA mb);

        /// <summary>
        /// Neutral element (`None` in `Option` for example)
        /// </summary>
        [Pure]
        MA Zero();

        /// <summary>
        /// Apply - used to facilitate default behavior for monad transformers 
        /// NOTE: Don't rely on this, it may not be a permanent addition to the project
        /// </summary>
        [Pure]
        MA Apply(Func<A, A, A> f, MA ma, MA mb);
    }
}
