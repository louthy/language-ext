using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.OptionalAsync;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Discriminated union type.  Can be in one of two states:
    /// 
    ///     Some(a)
    ///     None
    ///     
    /// Typeclass instances available for this type:
    /// 
    ///     Applicative   : ApplOptionAsync
    ///     BiFoldable    : MOptionAsync
    ///     Foldable      : MOptionAsync
    ///     Functor       : FOptionAsync
    ///     Monad         : MOptionAsync
    ///     OptionalAsync : MOptionAsync
    ///     
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct OptionAsync<A> :
// TODO: Re-add when we move to netstandard2.1
//#if NETCORE
//        IAsyncEnumerable<A>,
//#endif
        IOptionalAsync
    {
        internal readonly Task<(bool IsSome, A Value)> data;

        /// <summary>
        /// None
        /// </summary>
        public static readonly OptionAsync<A> None = new OptionAsync<A>((false, default(A)).AsTask());

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> Some(A value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : new OptionAsync<A>((true, value).AsTask());

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> SomeAsync(Task<A> value) =>
            new OptionAsync<A>(value.Map(v => isnull(v)
                ? throw new ValueIsNullException()
                : (true, v)));

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> Optional(A value) =>
             new OptionAsync<A>((notnull(value), value).AsTask());

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> OptionalAsync(Task<A> value) =>
            new OptionAsync<A>(value.Map(v => (notnull(v), v)));

        /// <summary>
        /// Data accessor
        /// </summary>
        internal Task<(bool IsSome, A Value)> Data => 
            data ?? None.data;

        /// <summary>
        /// Takes the value-type OptionV<A>
        /// </summary>
        internal OptionAsync(Task<(bool IsSome, A Value)> data) =>
            this.data = data;

        /// <summary>
        /// Ctor that facilitates serialisation
        /// </summary>
        /// </summary>
        /// <param name="option">None or Some A.</param>
        [Pure]
        public OptionAsync(IEnumerable<A> option)
        {
            var first = option.Take(1).ToArray();
            this.data = first.Length == 0
                ? (false, default(A)).AsTask()
                : (true, first[0]).AsTask();
        }

        /// <summary>
        /// Reference version of option for use in pattern-matching
        /// </summary>
        [Pure]
        public Task<OptionCase<A>> Case =>
            GetCase();

        [Pure]
        async Task<OptionCase<A>> GetCase()
        {
            var (isSome, value) = await data;
            return isSome
                ? SomeCase<A>.New(value)
                : NoneCase<A>.Default;
        }

        /// <summary>
        /// Implicit conversion operator from A to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator OptionAsync<A>(A a) =>
            Optional(a);

        /// <summary>
        /// Implicit conversion operator from A to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator OptionAsync<A>(Task<A> a) =>
            OptionalAsync(a);

        /// Implicit conversion operator from None to Option<A>
        /// </summary>
        /// <param name="a">None value</param>
        [Pure]
        public static implicit operator OptionAsync<A>(OptionNone a) =>
            default;

        /// <summary>
        /// Coalescing operator
        /// </summary>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">Right hand side of the operation</param>
        /// <returns>if lhs is Some then lhs, else rhs</returns>
        [Pure]
        public static OptionAsync<A> operator |(OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            MOptionAsync<A>.Inst.Plus(lhs, rhs);
        
        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public async Task<bool> Equals<EqA>(OptionAsync<A> rhs) where EqA : struct, EqAsync<A>
        {
            var a = await Data;
            var b = await rhs.Data;
            if(a.IsSome != b.IsSome) return false; 
            if(!a.IsSome && !b.IsSome) return true; 
            return await default(EqA).EqualsAsync(a.Value, b.Value);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public override bool Equals(object _) =>
            throw new NotSupportedException(
                "The standard Equals override is not supported for OptionAsync because it's an asynchronous type and " +
                "the return value is synchronous.  Use the typed version of Equals or the == operator to get a bool " +
                " Task that can be awaited");

        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public Task<bool> Equals(OptionAsync<A> rhs) =>
            Equals<EqDefaultAsync<A>>(rhs);

        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public static Task<bool> operator ==(OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        [Pure]
        public static Task<bool> operator !=(OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.Equals(rhs).Map(not);

        /// <summary>
        /// Ordering
        /// </summary>
        [Pure]
        public async Task<int> CompareTo<OrdA>(OptionAsync<A> rhs) where OrdA : struct, Ord<A>
        {
            var a = await Data;
            var b = await rhs.Data;
            var c = default(OrdBool).Compare(a.IsSome, b.IsSome);
            if (c != 0) return c;
            return default(OrdA).Compare(a.Value, b.Value);
        }

        /// <summary>
        /// Ordering
        /// </summary>
        [Pure]
        public Task<int> CompareTo(OptionAsync<A> rhs) =>
            CompareTo<OrdDefault<A>>(rhs);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator < (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.CompareTo(rhs).Map(x => x < 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator <= (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.CompareTo(rhs).Map(x => x <= 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator > (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.CompareTo(rhs).Map(x => x > 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator >= (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.CompareTo(rhs).Map(x => x >= 0);

        /// <summary>
        /// Calculate the hash-code from the bound value, unless the Option is in a None
        /// state, in which case the hash-code will be 0
        /// </summary>
        /// <returns>Hash-code from the bound value, unless the Option is in a None
        /// state, in which case the hash-code will be 0</returns>
        [Pure]
        public override int GetHashCode() =>
            throw new NotSupportedException("Call GetHashCodeAsync instead");

        /// <summary>
        /// Calculate the hash-code from the bound value, unless the Option is in a None
        /// state, in which case the hash-code will be 0
        /// </summary>
        /// <returns>Hash-code from the bound value, unless the Option is in a None
        /// state, in which case the hash-code will be 0</returns>
        [Pure]
        public Task<int> GetHashCodeAsync() =>
            data?.Map(a => a.GetHashCode()) ?? 0.AsTask();

        /// <summary>
        /// Get a string representation of the Option
        /// </summary>
        /// <returns>String representation of the Option</returns>
        [Pure]
        public override string ToString() =>
            "OptionAsync";

        /// <summary>
        /// Get a string representation of the Option
        /// </summary>
        /// <returns>String representation of the Option</returns>
        [Pure]
        public async Task<string> ToStringAsync()
        {
            var (isSome, value) = await data;
            return isSome
                ? $"Some({value})"
                : "None";
        }

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        [Pure]
        public Task<bool> IsSome =>
            Data.Map(a => a.IsSome);

        /// <summary>
        /// Is the option in a None state
        /// </summary>
        [Pure]
        public Task<bool> IsNone =>
            Data.Map(a => !a.IsSome);

        /// <summary>
        /// Helper accessor for the bound value
        /// </summary>
        internal Task<A> Value =>
            Data.Map(a => a.Value);

        /// <summary>
        /// Custom awaiter that turns an OptionAsync into an Option
        /// </summary>
        public TaskAwaiter<Option<A>> GetAwaiter() =>
            Data.Map(d => d.IsSome ? Option<A>.Some(d.Value) : Option<A>.None).GetAwaiter();
        
        /// <summary>
        /// Impure iteration of the bound value in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public OptionAsync<A> Do(Action<A> f) =>
            Map(x => { f(x); return x; });

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionAsync<B> Select<B>(Func<A, B> f) =>
            default(MOptionAsync<A>)
                .Bind<MOptionAsync<B>, OptionAsync<B>, B>(this, x => OptionAsync<B>.Some(f(x)));

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionAsync<B> Map<B>(Func<A, B> f) =>
            default(MOptionAsync<A>)
                .Bind<MOptionAsync<B>, OptionAsync<B>, B>(this, x => OptionAsync<B>.Some(f(x)));

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionAsync<B> MapAsync<B>(Func<A, Task<B>> f) =>
            default(MOptionAsync<A>)
                .BindAsync<MOptionAsync<B>, OptionAsync<B>, B>(this, async x => OptionAsync<B>.Some(await f(x)));

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionAsync<B> Bind<B>(Func<A, OptionAsync<B>> f) =>
            default(MOptionAsync<A>).Bind<MOptionAsync<B>, OptionAsync<B>, B>(this, f);

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionAsync<B> BindAsync<B>(Func<A, Task<OptionAsync<B>>> f) =>
            default(MOptionAsync<A>).BindAsync<MOptionAsync<B>, OptionAsync<B>, B>(this, f);

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionAsync<C> SelectMany<B, C>(
            Func<A, OptionAsync<B>> bind,
            Func<A, B, C> project) =>
            default(MOptionAsync<A>).Bind<MOptionAsync<C>, OptionAsync<C>, C>(this,    a =>
            default(MOptionAsync<B>).Bind<MOptionAsync<C>, OptionAsync<C>, C>(bind(a), b =>
            default(MOptionAsync<C>).ReturnAsync(project(a, b).AsTask())));

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public Task<R> MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(this, Some, None);

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public Task<R> MatchUntyped<R>(Func<object, Task<R>> Some, Func<R> None) =>
            matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(this, Some, None);

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public Task<R> MatchUntypedAsync<R>(Func<object, R> Some, Func<Task<R>> None) =>
            matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(this, Some, None);

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public Task<R> MatchUntypedAsync<R>(Func<object, Task<R>> Some, Func<Task<R>> None) =>
            matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(this, Some, None);

        /// <summary>
        /// Get the Type of the bound value
        /// </summary>
        /// <returns>Type of the bound value</returns>
        [Pure]
        public Type GetUnderlyingType() => 
            typeof(A);

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public Task<Arr<A>> ToArray() =>
            toArrayAsync<MOptionAsync<A>, OptionAsync<A>, A>(this);

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public Task<Lst<A>> ToList() =>
            toListAsync<MOptionAsync<A>, OptionAsync<A>, A>(this);

        /// <summary>
        /// Convert the Option to an enumerable sequence of zero or one items
        /// </summary>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public Task<Seq<A>> ToSeq() =>
            toSeqAsync<MOptionAsync<A>, OptionAsync<A>, A>(this);

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public Task<IEnumerable<A>> AsEnumerable() =>
            asEnumerableAsync<MOptionAsync<A>, OptionAsync<A>, A>(this);

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
        /// <returns>An Either representation of the structure</returns>
        [Pure]
        public EitherAsync<L, A> ToEither<L>(L defaultLeftValue) =>
            toEitherAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(this, defaultLeftValue);

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        /// <param name="defaultLeftValue">Function to invoke to get a default value if the 
        /// structure is in a None state</param>
        /// <returns>An Either representation of the structure</returns>
        [Pure]
        public EitherAsync<L, A> ToEither<L>(Func<L> Left) =>
            toEitherAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(this, Left);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
        /// <returns>An EitherUnsafe representation of the structure</returns>
        [Pure]
        public Task<EitherUnsafe<L, A>> ToEitherUnsafe<L>(L defaultLeftValue) =>
            toEitherUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(this, defaultLeftValue);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        /// <param name="defaultLeftValue">Function to invoke to get a default value if the 
        /// structure is in a None state</param>
        /// <returns>An EitherUnsafe representation of the structure</returns>
        [Pure]
        public Task<EitherUnsafe<L, A>> ToEitherUnsafe<L>(Func<L> Left) =>
            toEitherUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(this, Left);

        /// <summary>
        /// Convert the structure to a Option
        /// </summary>
        /// <returns>An Option representation of the structure</returns>
        [Pure]
        public Task<Option<A>> ToOption() =>
            Match(x => Option<A>.Some(x), () => Option<A>.None);

        /// <summary>
        /// Convert the structure to a OptionUnsafe
        /// </summary>
        /// <returns>An OptionUnsafe representation of the structure</returns>
        [Pure]
        public Task<OptionUnsafe<A>> ToOptionUnsafe() =>
            toOptionUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this);

        /// <summary>
        /// Convert the structure to a TryOptionAsync
        /// </summary>
        /// <returns>A TryOptionAsync representation of the structure</returns>
        [Pure]
        public TryOptionAsync<A> ToTryOption() =>
            toTryOptionAsync<MOptionAsync<A>, OptionAsync<A>, A>(this);

        /// <summary>
        /// Convert the structure to a TryAsync
        /// </summary>
        /// <returns>A TryAsync representation of the structure</returns>
        [Pure]
        public TryAsync<A> ToTry() =>
            toTryAsync<MOptionAsync<A>, OptionAsync<A>, A>(this);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public Task<B> Match<B>(Func<A, B> Some, Func<B> None) =>
            MOptionAsync<A>.Inst.Match(this, Some, None);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public Task<B> MatchAsync<B>(Func<A, Task<B>> Some, Func<B> None) =>
            MOptionAsync<A>.Inst.MatchAsync(this, Some, None);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public Task<B> MatchAsync<B>(Func<A, B> Some, Func<Task<B>> None) =>
            MOptionAsync<A>.Inst.MatchAsync(this, Some, None);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public Task<B> MatchAsync<B>(Func<A, Task<B>> Some, Func<Task<B>> None) =>
            MOptionAsync<A>.Inst.MatchAsync(this, Some, None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public Task<B> MatchUnsafe<B>(Func<A, B> Some, Func<B> None) =>
            MOptionAsync<A>.Inst.MatchUnsafe(this, Some, None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public Task<B> MatchUnsafeAsync<B>(Func<A, Task<B>> Some, Func<B> None) =>
            MOptionAsync<A>.Inst.MatchUnsafeAsync(this, Some, None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public Task<B> MatchUnsafeAsync<B>(Func<A, B> Some, Func<Task<B>> None) =>
            MOptionAsync<A>.Inst.MatchUnsafeAsync(this, Some, None);

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public Task<B> MatchUnsafeAsync<B>(Func<A, Task<B>> Some, Func<Task<B>> None) =>
            MOptionAsync<A>.Inst.MatchUnsafeAsync(this, Some, None);

        /// <summary>
        /// Match the two states of the Option
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        public Task<Unit> Match(Action<A> Some, Action None) =>
            MOptionAsync<A>.Inst.Match(this, Some, None);

        /// <summary>
        /// Invokes the action if Option is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">async Action to invoke if Option is in the Some state</param>
        public Task<Unit> IfSome(Func<A, Task> f) =>
            ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, f);

        /// <summary>
        /// Invokes the action if Option is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">Action to invoke if Option is in the Some state</param>
        public Task<Unit> IfSome(Action<A> f) =>
            ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, f);

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if Option is in the Some state</param>
        public Task<Unit> IfSomeAsync(Func<A, Task<Unit>> f) =>
            ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, f);

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if Option is in the Some state</param>
        public Task<Unit> IfSomeAsync(Func<A, Task> f) =>
            ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, f);

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if Option is in the Some state</param>
        public Task<Unit> IfSome(Func<A, Unit> f) =>
            ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, f);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public Task<A> IfNone(Func<A> None) =>
            ifNoneAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, None);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public Task<A> IfNoneAsync(Func<Task<A>> None) =>
            ifNoneAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, None);

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public Task<A> IfNone(A noneValue) =>
            ifNoneAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, noneValue);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow null the be returned from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public Task<A> IfNoneUnsafe(Func<A> None) =>
            OptionalUnsafeAsync.ifNoneUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, None);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow null the be returned from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public Task<A> IfNoneUnsafeAsync(Func<Task<A>> None) =>
            OptionalUnsafeAsync.ifNoneUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, None);

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow noneValue to be null</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public Task<A> IfNoneUnsafe(A noneValue) =>
            OptionalUnsafeAsync.ifNoneUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, noneValue);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if Option is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> Fold<S>(S state, Func<S, A, S> folder) =>
            MOptionAsync<A>.Inst.Fold(this, state, folder)(unit);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if Option is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> FoldAsync<S>(S state, Func<S, A, Task<S>> folder) =>
            MOptionAsync<A>.Inst.FoldAsync(this, state, folder)(unit);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if Option is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> FoldBack<S>(S state, Func<S, A, S> folder) =>
            MOptionAsync<A>.Inst.FoldBack(this, state, folder)(unit);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if Option is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> FoldBackAsync<S>(S state, Func<S, A, Task<S>> folder) =>
            MOptionAsync<A>.Inst.FoldBackAsync(this, state, folder)(unit);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFold<S>(S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            MOptionAsync<A>.Inst.BiFold(this, state, Some, None);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFold<S>(S state, Func<S, A, Task<S>> Some, Func<S, Unit, S> None) =>
            MOptionAsync<A>.Inst.BiFoldAsync(this, state, Some, None);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> None) =>
            MOptionAsync<A>.Inst.BiFoldAsync(this, state, Some, None);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, A, Task<S>> Some, Func<S, Unit, Task<S>> None) =>
            MOptionAsync<A>.Inst.BiFoldAsync(this, state, Some, None);

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFold<S>(S state, Func<S, A, S> Some, Func<S, S> None) =>
            MOptionAsync<A>.Inst.BiFold(this, state, Some, (s, _) => None(s));

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, A, Task<S>> Some, Func<S, S> None) =>
            MOptionAsync<A>.Inst.BiFoldAsync(this, state, Some, (s, _) => None(s));

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, A, S> Some, Func<S, Task<S>> None) =>
            MOptionAsync<A>.Inst.BiFoldAsync(this, state, Some, (s, _) => None(s));

        /// <summary>
        /// <para>
        /// Option types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// <para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if Option is in a Some state</param>
        /// <param name="None">Folder function, applied if Option is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, A, Task<S>> Some, Func<S, Task<S>> None) =>
            MOptionAsync<A>.Inst.BiFoldAsync(this, state, Some, (s, _) => None(s));

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionAsync<B> BiMap<B>(Func<A, B> Some, Func<Unit, B> None) =>
            FOptionAsync<A, B>.Inst.BiMapAsync(this, Some, None);

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionAsync<B> BiMap<B>(Func<A, B> Some, Func<B> None) =>
            FOptionAsync<A, B>.Inst.BiMapAsync(this, Some, _ => None());

        /// <summary>
        /// <para>
        /// Return the number of bound values in this structure:
        /// </para>
        /// <para>
        ///     None = 0
        /// </para>
        /// <para>
        ///     Some = 1
        /// </para>
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Task<int> Count() =>
            MOptionAsync<A>.Inst.Count(this)(unit);

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then True is returned (because the predicate applies for-all values).
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the Option is in a None state then True is returned (because 
        /// the predicate applies for-all values).  If the Option is in a Some state
        /// the value is the result of running applying the bound value to the 
        /// predicate supplied.</returns>
        [Pure]
        public Task<bool> ForAll(Func<A, bool> pred) =>
            forallAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then True is returned (because the predicate applies for-all values).
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the Option is in a None state then True is returned (because 
        /// the predicate applies for-all values).  If the Option is in a Some state
        /// the value is the result of running applying the bound value to the 
        /// predicate supplied.</returns>
        [Pure]
        public Task<bool> ForAllAsync(Func<A, Task<bool>> pred) =>
            forallAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then True is returned if invoking None returns True.
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the Option is in a None state then True is returned if 
        /// invoking None returns True. If the Option is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public Task<bool> BiForAll(Func<A, bool> Some, Func<Unit, bool> None) =>
            biForAllAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(this, Some, None);

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then True is returned if invoking None returns True.
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the Option is in a None state then True is returned if 
        /// invoking None returns True. If the Option is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public Task<bool> BiForAll(Func<A, bool> Some, Func<bool> None) =>
            biForAllAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(this, Some, _ => None());

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then False is returned.
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the Option is in a None state then False is returned.
        /// If the Option is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public Task<bool> Exists(Func<A, bool> pred) =>
            existsAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then True is returned if invoking None returns True.
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the Option is in a None state then True is returned if 
        /// invoking None returns True. If the Option is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public Task<bool> ExistsAsync(Func<A, Task<bool>> pred) =>
            existsAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then True is returned if invoking None returns True.
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the Option is in a None state then True is returned if 
        /// invoking None returns True. If the Option is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public Task<bool> BiExists(Func<A, bool> Some, Func<Unit, bool> None) =>
            biExistsAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(this, Some, None);

        /// <summary>
        /// Apply a predicate to the bound value.  If the Option is in a None state
        /// then True is returned if invoking None returns True.
        /// If the Option is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the Option is in a None state then True is returned if 
        /// invoking None returns True. If the Option is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public Task<bool> BiExists(Func<A, bool> Some, Func<bool> None) =>
            biExistsAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(this, Some, _ => None());

        /// <summary>
        /// Invoke an action for the bound value (if in a Some state)
        /// </summary>
        /// <param name="Some">Action to invoke</param>
        [Pure]
        public Task<Unit> Iter(Action<A> Some) =>
            iterAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, Some);

        /// <summary>
        /// Invoke an action for the bound value (if in a Some state)
        /// </summary>
        /// <param name="Some">Action to invoke</param>
        [Pure]
        public Task<Unit> Iter(Func<A, Task<Unit>> Some) =>
            iterAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, Some);

        /// <summary>
        /// Invoke an action depending on the state of the Option
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        [Pure]
        public Task<Unit> BiIter(Action<A> Some, Action<Unit> None) =>
            biIterAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(this, Some, None);

        /// <summary>
        /// Invoke an action depending on the state of the Option
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        [Pure]
        public Task<Unit> BiIter(Action<A> Some, Action None) =>
            biIterAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(this, Some, _ => None());

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> Filter(Func<A, bool> pred) =>
            filterAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> FilterAsync(Func<A, Task<bool>> pred) =>
            filterAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> Where(Func<A, bool> pred) =>
            filterAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> Where(Func<A, Task<bool>> pred) =>
            filterAsync<MOptionAsync<A>, OptionAsync<A>, A>(this, pred);

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public OptionAsync<D> Join<B, C, D>(
            OptionAsync<B> inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, D> project) =>
            joinAsync<EqDefault<C>, MOptionAsync<A>, MOptionAsync<B>, MOptionAsync<D>, OptionAsync<A>, OptionAsync<B>, OptionAsync<D>, A, B, C, D>(
                this, inner, outerKeyMap, innerKeyMap, project
                );

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public OptionAsync<D> Join<B, C, D>(
            OptionAsync<B> inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, Task<D>> project) =>
            joinAsync<EqDefault<C>, MOptionAsync<A>, MOptionAsync<B>, MOptionAsync<D>, OptionAsync<A>, OptionAsync<B>, OptionAsync<D>, A, B, C, D>(
                this, inner, outerKeyMap, innerKeyMap, project
                );

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public OptionAsync<D> Join<B, C, D>(
            OptionAsync<B> inner,
            Func<A, Task<C>> outerKeyMap,
            Func<B, Task<C>> innerKeyMap,
            Func<A, B, Task<D>> project) =>
            joinAsync<EqDefault<C>, MOptionAsync<A>, MOptionAsync<B>, MOptionAsync<D>, OptionAsync<A>, OptionAsync<B>, OptionAsync<D>, A, B, C, D>(
                this, inner, outerKeyMap, innerKeyMap, project
                );

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public OptionAsync<D> Join<B, C, D>(
            OptionAsync<B> inner,
            Func<A, Task<C>> outerKeyMap,
            Func<B, Task<C>> innerKeyMap,
            Func<A, B, D> project) =>
            joinAsync<EqDefault<C>, MOptionAsync<A>, MOptionAsync<B>, MOptionAsync<D>, OptionAsync<A>, OptionAsync<B>, OptionAsync<D>, A, B, C, D>(
                this, inner, outerKeyMap, innerKeyMap, project
                );

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public OptionAsync<Func<B, C>> ParMap<B, C>(Func<A, B, C> func) =>
            Map(curry(func));

        /// <summary>
        /// Partial application map
        /// </summary>
        [Pure]
        public OptionAsync<Func<B, Func<C, D>>> ParMap<B, C, D>(Func<A, B, C, D> func) =>
            Map(curry(func));

// TODO: Re-add when we move to netstandard2.1
//#if NETCORE
//        /// <summary>
//        /// Enumerate asynchronously
//        /// </summary>
//        [Pure]
//        public async IAsyncEnumerator<A> GetAsyncEnumerator(CancellationToken cancellationToken = default)
//        {
//            var (isSome, value) = await Data;
//            if(isSome)
//            {
//                yield return value;
//            }
//        }
//#endif
    }
}
