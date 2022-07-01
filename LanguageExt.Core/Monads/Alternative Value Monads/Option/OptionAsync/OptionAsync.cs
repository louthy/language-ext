#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.OptionalAsync;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Common;
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
    public readonly struct OptionAsync<A> :
        IAsyncEnumerable<A>,
        IOptionalAsync
    {
        internal readonly Aff<A> Effect;

        /// <summary>
        /// None
        /// </summary>
        public static readonly OptionAsync<A> None = new (FailAff<A>(Errors.None));

        public OptionAsync() =>
            Effect = None.Effect;

        /// <summary>
        /// Takes the value-type OptionV<A>
        /// </summary>
        internal OptionAsync(Aff<A> effect) =>
            Effect = effect;

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> Some(A value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : new OptionAsync<A>(SuccessAff(value));

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> SomeAsync(Task<A> value) =>
            value is null
                ? throw new ArgumentNullException(nameof(value))
                : new (Aff(async () => await value.ConfigureAwait(false))
                            .Memo()
                            .Bind(x => x is null 
                                          ? throw new ValueIsNullException() 
                                          : SuccessAff(x)));

        
        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> SomeAsync(ValueTask<A> value) =>
            new (Aff(() => value).Memo()
                                 .Bind(x => x is null 
                                                ? throw new ValueIsNullException() 
                                                : SuccessAff(x)));

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> Optional(A value) =>
            value is null
                ? new OptionAsync<A>(FailAff<A>(Errors.None))
                : new OptionAsync<A>(SuccessAff(value));

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> OptionalAsync(Task<A> value) =>
            new (Aff(async () => await value.ConfigureAwait(false))
                .Memo()
                .Bind(x => x is null 
                    ? FailAff<A>(Errors.None) 
                    : SuccessAff(x)));

        /// <summary>
        /// Construct an OptionAsync of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionAsync of A</returns>
        [Pure]
        public static OptionAsync<A> OptionalAsync(ValueTask<A> value) =>
            new (Aff(() => value).Memo()
                                 .Bind(x => x is null 
                                                ? FailAff<A>(Errors.None) 
                                                : SuccessAff(x)));

        /// <summary>
        /// Ctor that facilitates serialisation
        /// </summary>
        /// <param name="option">None or Some A.</param>
        public OptionAsync(IEnumerable<A> option)
        {
            var first = option.Take(1).ToArray();
            Effect = first.Length == 0
                ? None.Effect
                : SuccessAff(first[0]);
        }

        /// <summary>
        /// Reference version of option for use in pattern-matching
        /// </summary>
        /// <remarks>
        ///
        ///     Some = result is ValueTask<A>
        ///     None = result is ValueTask<null>
        ///
        /// </remarks>
        [Pure]
        public ValueTask<object?> Case =>
            GetCase();

        [Pure]
        async ValueTask<object?> GetCase() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsFail: true} => null,
                var r          => r.Value
            };

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

        /// <summary>
        /// Implicit conversion operator from A to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator OptionAsync<A>(ValueTask<A> a) =>
            OptionalAsync(a);

        /// <summary>
        /// Implicit conversion operator from None to Option<A>
        /// </summary>
        /// <param name="a">None value</param>
        [Pure]
        public static implicit operator OptionAsync<A>(OptionNone _) =>
            None;

        /// <summary>
        /// Coalescing operator
        /// </summary>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">Right hand side of the operation</param>
        /// <returns>if lhs is Some then lhs, else rhs</returns>
        [Pure]
        public static OptionAsync<A> operator |(OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            new (lhs.Effect | rhs.Effect);

        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public async ValueTask<bool> Equals<EqA>(OptionAsync<A> rhs) where EqA : struct, Eq<A> =>
            (await Effect.Run().ConfigureAwait(false)).Equals<EqA>(await rhs.Effect.Run().ConfigureAwait(false));
        
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
        public async ValueTask<bool> Equals(OptionAsync<A> rhs) =>
            (await Effect.Run().ConfigureAwait(false)).Equals(await rhs.Effect.Run().ConfigureAwait(false));

        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public static ValueTask<bool> operator ==(OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        [Pure]
        public static ValueTask<bool> operator !=(OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.Equals(rhs).Map(not);

        /// <summary>
        /// Ordering
        /// </summary>
        [Pure]
        public async ValueTask<int> CompareTo<OrdA>(OptionAsync<A> rhs) where OrdA : struct, Ord<A> =>
            (await Effect.Run().ConfigureAwait(false)).CompareTo<OrdA>(await rhs.Effect.Run().ConfigureAwait(false));

        /// <summary>
        /// Ordering
        /// </summary>
        [Pure]
        public ValueTask<int> CompareTo(OptionAsync<A> rhs) =>
            CompareTo<OrdDefault<A>>(rhs);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static ValueTask<bool> operator < (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.CompareTo(rhs).Map(x => x < 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static ValueTask<bool> operator <= (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.CompareTo(rhs).Map(x => x <= 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static ValueTask<bool> operator > (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
            lhs.CompareTo(rhs).Map(x => x > 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static ValueTask<bool> operator >= (OptionAsync<A> lhs, OptionAsync<A> rhs) =>
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
        public async ValueTask<int> GetHashCodeAsync() =>
            (await Effect.Run().ConfigureAwait(false)) switch
            {
                {IsFail: true} => 0,
                var x => x.Value?.GetHashCode() ?? 0
            };

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
        public async ValueTask<string> ToStringAsync() =>
            (await Effect.Run().ConfigureAwait(false)) switch
            {
                {IsFail: true} => "None",
                var x => $"Some({x.Value})"
            };

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        [Pure]
        public ValueTask<bool> IsSome =>
            GetIsSome();
        
        /// <summary>
        /// Is the option in a None state
        /// </summary>
        [Pure]
        public ValueTask<bool> IsNone =>
            GetIsNone();

        /// <summary>
        /// Helper accessor for the bound value
        /// </summary>
        internal ValueTask<A> Value =>
            GetValue();

        async ValueTask<bool> GetIsSome() =>
            await Effect.Run().ConfigureAwait(false) is {IsSucc: true};

        async ValueTask<bool> GetIsNone() =>
            await Effect.Run().ConfigureAwait(false) is {IsSucc: false};

        internal async ValueTask<(bool IsSome, A? Value)> GetData() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc: true} fx => (true, fx.Value),
                _                 => (false, default(A))
            };

        async ValueTask<A> GetValue() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc: true} fx => fx.Value,
                _ => throw new ValueIsNoneException()
            };

        async ValueTask<A?> GetValueNullable() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc: true} fx => fx.Value,
                _                 => default
            };
        
        /// <summary>
        /// Custom awaiter so OptionAsync can be used with async/await 
        /// </summary>
        public ValueTaskAwaiter<A?> GetAwaiter() =>
            GetValueNullable().GetAwaiter();
        
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
            new (Effect.Map(f));

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionAsync<B> Map<B>(Func<A, B> f) =>
            new (Effect.Map(f));

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionAsync<B> MapAsync<B>(Func<A, ValueTask<B>> f) =>
            new (Effect.MapAsync(f));

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionAsync<B> Bind<B>(Func<A, OptionAsync<B>> f) =>
            new(Effect.Bind(x => f(x).Effect));  

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionAsync<B> BindAsync<B>(Func<A, ValueTask<OptionAsync<B>>> f) =>
            new(Effect.BindAsync(async x => (await f(x).ConfigureAwait(false)).Effect));  

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionAsync<C> SelectMany<B, C>(
            Func<A, OptionAsync<B>> bind,
            Func<A, B, C> project) =>
            new(Effect.Bind(a => bind(a).Effect.Map(b => project(a, b))));

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public async ValueTask<R?> MatchUntyped<R>(Func<object?, R?> Some, Func<R?> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(fx.Value),
                _ => None()
            };

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public async ValueTask<R?> MatchUntyped<R>(Func<object?, ValueTask<R?>> Some, Func<R?> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(fx.Value).ConfigureAwait(false),
                _ => None()
            };

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public async ValueTask<R?> MatchUntypedAsync<R>(Func<object?, R?> Some, Func<ValueTask<R?>> None) =>
            await Effect.Run() switch
            {
                {IsSucc : true} fx => Some(fx.Value),
                _ => await None().ConfigureAwait(false)
            };

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public async ValueTask<R?> MatchUntypedAsync<R>(Func<object?, ValueTask<R?>> Some, Func<ValueTask<R?>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(fx.Value).ConfigureAwait(false),
                _ => await None().ConfigureAwait(false)
            };

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
        public async ValueTask<Arr<A>> ToArray() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Array(fx.Value),
                _                  => Arr<A>.Empty
            };
        
        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public async ValueTask<Lst<A>> ToList() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => List(fx.Value),
                _                  => Lst<A>.Empty
            };

        /// <summary>
        /// Convert the Option to an enumerable sequence of zero or one items
        /// </summary>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public async ValueTask<Seq<A>> ToSeq() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Seq1(fx.Value),
                _                  => Seq<A>.Empty
            };

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public async ValueTask<IEnumerable<A>> AsEnumerable() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => new [] { fx.Value },
                _                  => System.Array.Empty<A>()
            };
        
        /// <summary>
        /// Convert the structure to an Aff
        /// </summary>
        /// <returns>An Aff representation of the structure</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<A> ToAff() =>
            Effect;

        /// <summary>
        /// Convert the structure to an Aff
        /// </summary>
        /// <param name="Fail">Default value if the structure is in a None state</param>
        /// <returns>An Aff representation of the structure</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<A> ToAff(Error Fail) =>
            Effect | @catch(Fail);

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
        public async ValueTask<EitherUnsafe<L, A>> ToEitherUnsafe<L>(L defaultLeftValue) =>
            await toEitherUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(this, defaultLeftValue).ConfigureAwait(false);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        /// <param name="defaultLeftValue">Function to invoke to get a default value if the 
        /// structure is in a None state</param>
        /// <returns>An EitherUnsafe representation of the structure</returns>
        [Pure]
        public async ValueTask<EitherUnsafe<L, A>> ToEitherUnsafe<L>(Func<L> Left) =>
            await toEitherUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(this, Left).ConfigureAwait(false);

        /// <summary>
        /// Convert the structure to a Option
        /// </summary>
        /// <returns>An Option representation of the structure</returns>
        [Pure]
        public async ValueTask<Option<A>> ToOption() =>
            await Match(Option<A>.Some, () => Option<A>.None).ConfigureAwait(false);

        /// <summary>
        /// Convert the structure to a OptionUnsafe
        /// </summary>
        /// <returns>An OptionUnsafe representation of the structure</returns>
        [Pure]
        public async Task<OptionUnsafe<A>> ToOptionUnsafe() =>
            await toOptionUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(this).ConfigureAwait(false);

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
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for dispatching actions, use Some<A,B>(...) to return a value
        /// from the match operation.
        /// </summary>
        /// <param name="f">The Some(x) match operation</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SomeAsyncUnitContext<MOptionAsync<A>, OptionAsync<A>, A> Some(Action<A> f) =>
            new (this, f);

        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for returning a value from the match operation, to dispatch
        /// an action instead, use Some<A>(...)
        /// </summary>
        /// <typeparam name="B">Match operation return value type</typeparam>
        /// <param name="f">The Some(x) match operation</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SomeAsyncContext<MOptionAsync<A>, OptionAsync<A>, A, B> Some<B>(Func<A, B> f) =>
            new (this, f);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public async ValueTask<B> Match<B>(Func<A, B> Some, Func<B> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(fx.Value),
                _                  => None()
            };

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public async ValueTask<B> MatchAsync<B>(Func<A, ValueTask<B>> Some, Func<B> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(fx.Value).ConfigureAwait(false),
                _                  => None()
            };

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public async ValueTask<B> MatchAsync<B>(Func<A, B> Some, Func<ValueTask<B>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(fx.Value),
                _                  => await None().ConfigureAwait(false)
            };

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public async ValueTask<B> MatchAsync<B>(Func<A, ValueTask<B>> Some, Func<ValueTask<B>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(fx.Value),
                _                  => await None().ConfigureAwait(false)
            };

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public async ValueTask<B?> MatchUnsafe<B>(Func<A, B?> Some, Func<B?> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(fx.Value),
                _                  => None()
            };

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public async ValueTask<B?> MatchUnsafeAsync<B>(Func<A, ValueTask<B?>> Some, Func<B?> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(fx.Value).ConfigureAwait(false),
                _                  => None()
            };

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public async ValueTask<B?> MatchUnsafeAsync<B>(Func<A, B?> Some, Func<ValueTask<B?>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(fx.Value),
                _                  => await None().ConfigureAwait(false)
            };

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public async ValueTask<B?> MatchUnsafeAsync<B>(Func<A, ValueTask<B?>> Some, Func<ValueTask<B?>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(fx.Value).ConfigureAwait(false),
                _                  => await None().ConfigureAwait(false)
            };

        /// <summary>
        /// Match the two states of the Option
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        public async ValueTask<Unit> Match(Action<A> Some, Action None)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    Some(fx.Value);
                    return unit;
                default:
                    None();
                    return unit;
            }
        }

        /// <summary>
        /// Invokes the action if Option is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">async Action to invoke if Option is in the Some state</param>
        public async ValueTask<Unit> IfSome(Func<A, ValueTask> f)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    await f(fx.Value);
                    return unit;
                default:
                    return unit;
            }
        }

        /// <summary>
        /// Invokes the action if Option is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">Action to invoke if Option is in the Some state</param>
        public async ValueTask<Unit> IfSome(Action<A> f)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    f(fx.Value);
                    return unit;
                default:
                    return unit;
            }
        }

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if Option is in the Some state</param>
        public async ValueTask<Unit> IfSomeAsync(Func<A, ValueTask<Unit>> f)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    return await f(fx.Value);
                default:
                    return unit;
            }
        }

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if Option is in the Some state</param>
        public async ValueTask<Unit> IfSomeAsync(Func<A, ValueTask> f)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    await f(fx.Value);
                    return unit;
                default:
                    return unit;
            }
        }

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if Option is in the Some state</param>
        public async ValueTask<Unit> IfSome(Func<A, Unit> f)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    return f(fx.Value);
                default:
                    return unit;
            }
        }

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public async ValueTask<A> IfNone(Func<A> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => fx.Value,
                _ => None()
            };

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public async ValueTask<A> IfNoneAsync(Func<ValueTask<A>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => fx.Value,
                _ => await None().ConfigureAwait(false)
            };

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public async ValueTask<A> IfNone(A noneValue) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => fx.Value,
                _ => noneValue
            };

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow null the be returned from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public async ValueTask<A?> IfNoneUnsafe(Func<A?> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => fx.Value,
                _ => None()
            };

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow null the be returned from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public async ValueTask<A?> IfNoneUnsafeAsync(Func<ValueTask<A?>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => fx.Value,
                _ => await None().ConfigureAwait(false)
            };

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow noneValue to be null</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public async ValueTask<A?> IfNoneUnsafe(A? noneValue) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => fx.Value,
                _ => noneValue
            };

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
        public async ValueTask<S> Fold<S>(S state, Func<S, A, S> folder) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => folder(state, fx.Value),
                _                  => state
            };

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
        public async ValueTask<S> FoldAsync<S>(S state, Func<S, A, ValueTask<S>> folder) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await folder(state, fx.Value).ConfigureAwait(false),
                _                  => state
            };

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
        public async ValueTask<S> FoldBack<S>(S state, Func<S, A, S> folder) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => folder(state, fx.Value),
                _                  => state
            };

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
        public async ValueTask<S> FoldBackAsync<S>(S state, Func<S, A, ValueTask<S>> folder) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await folder(state, fx.Value).ConfigureAwait(false),
                _                  => state
            };

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
        public async ValueTask<S> BiFold<S>(S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(state, fx.Value),
                _                  => None(state, unit)
            };

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
        public async ValueTask<S> BiFold<S>(S state, Func<S, A, ValueTask<S>> Some, Func<S, Unit, S> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(state, fx.Value).ConfigureAwait(false),
                _                  => None(state, unit)
            };

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
        public async ValueTask<S> BiFoldAsync<S>(S state, Func<S, A, S> Some, Func<S, Unit, ValueTask<S>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(state, fx.Value),
                _                  => await None(state, unit).ConfigureAwait(false)
            };

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
        public async ValueTask<S> BiFoldAsync<S>(S state, Func<S, A, ValueTask<S>> Some, Func<S, Unit, ValueTask<S>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(state, fx.Value).ConfigureAwait(false),
                _                  => await None(state, unit)
            };

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
        public async ValueTask<S> BiFold<S>(S state, Func<S, A, S> Some, Func<S, S> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(state, fx.Value),
                _                  => None(state)
            };

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
        public async ValueTask<S> BiFoldAsync<S>(S state, Func<S, A, ValueTask<S>> Some, Func<S, S> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(state, fx.Value).ConfigureAwait(false),
                _                  => None(state)
            };

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
        public async ValueTask<S> BiFoldAsync<S>(S state, Func<S, A, S> Some, Func<S, ValueTask<S>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => Some(state, fx.Value),
                _                  => await None(state).ConfigureAwait(false)
            };

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
        public async ValueTask<S> BiFoldAsync<S>(S state, Func<S, A, ValueTask<S>> Some, Func<S, ValueTask<S>> None) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await Some(state, fx.Value).ConfigureAwait(false),
                _                  => await None(state).ConfigureAwait(false)
            };

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
        public async ValueTask<int> Count() =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} => 1,
                _               => 0
            };

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
        public async ValueTask<bool> ForAll(Func<A, bool> pred) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => pred(fx.Value),
                _                  => true
            };

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
        public async ValueTask<bool> ForAllAsync(Func<A, ValueTask<bool>> pred) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await pred(fx.Value).ConfigureAwait(false),
                _                  => true
            };

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
        public async ValueTask<bool> Exists(Func<A, bool> pred) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => pred(fx.Value),
                _                  => false
            };

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
        public async ValueTask<bool> ExistsAsync(Func<A, ValueTask<bool>> pred) =>
            await Effect.Run().ConfigureAwait(false) switch
            {
                {IsSucc : true} fx => await pred(fx.Value).ConfigureAwait(false),
                _                  => false
            };

        /// <summary>
        /// Invoke an action for the bound value (if in a Some state)
        /// </summary>
        /// <param name="Some">Action to invoke</param>
        public async ValueTask<Unit> Iter(Action<A> Some)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    Some(fx.Value);
                    return unit;
                
                default:
                    return unit;
            }
        }

        /// <summary>
        /// Invoke an action for the bound value (if in a Some state)
        /// </summary>
        /// <param name="Some">Action to invoke</param>
        public async ValueTask<Unit> Iter(Func<A, ValueTask<Unit>> Some)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    return await Some(fx.Value).ConfigureAwait(false);
                
                default:
                    return unit;
            }
        }

        /// <summary>
        /// Invoke an action depending on the state of the Option
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        public async ValueTask<Unit> BiIter(Action<A> Some, Action<Unit> None)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    Some(fx.Value);
                    return unit;
                
                default:
                    None(unit);
                    return unit;
            }
        }

        /// <summary>
        /// Invoke an action depending on the state of the Option
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        public async ValueTask<Unit> BiIter(Action<A> Some, Action None)
        {
            switch (await Effect.Run().ConfigureAwait(false))
            {
                case {IsSucc : true} fx:
                    Some(fx.Value);
                    return unit;
                
                default:
                    None();
                    return unit;
            }
        }

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> Filter(Func<A, bool> pred) =>
            new(Effect.Bind(x => pred(x) ? SuccessEff(x) : FailEff<A>(Errors.None)));

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> FilterAsync(Func<A, ValueTask<bool>> pred) =>
            new(Effect.BindAsync<A, A>(async x => await pred(x).ConfigureAwait(false) 
                                                    ? SuccessEff(x) 
                                                    : FailEff<A>(Errors.None)));

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> Where(Func<A, bool> pred) =>
            new(Effect.Bind(x => pred(x) ? SuccessEff(x) : FailEff<A>(Errors.None)));

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the Option is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionAsync<A> Where(Func<A, ValueTask<bool>> pred) =>
            new(Effect.BindAsync<A, A>(async x => await pred(x).ConfigureAwait(false) 
                                                    ? SuccessEff(x) 
                                                    : FailEff<A>(Errors.None)));

        /// <summary>
        /// Enumerate asynchronously
        /// </summary>
        [Pure]
        public async IAsyncEnumerator<A> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var r = await Effect.Run().ConfigureAwait(false);
            if (r.IsSucc) yield return r.Value;
        }
    }
}
