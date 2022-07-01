#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public static OptionAsync<A> flatten<A>(OptionAsync<OptionAsync<A>> ma) =>
            ma.Bind(identity);

        /// <summary>
        /// Subtract the Ts
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static OptionAsync<T> subtract<NUM, T>(OptionAsync<T> lhs, OptionAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Subtract<NUM, T>(rhs);

        /// <summary>
        /// Find the product of the Ts
        [Pure]
        public static OptionAsync<T> product<NUM, T>(OptionAsync<T> lhs, OptionAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Product<NUM, T>(rhs);

        /// <summary>
        /// Divide the Ts
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static OptionAsync<T> divide<NUM, T>(OptionAsync<T> lhs, OptionAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Divide<NUM, T>(rhs);

        /// <summary>
        /// Add the Ts
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static OptionAsync<T> add<NUM, T>(OptionAsync<T> lhs, OptionAsync<T> rhs) where NUM : struct, Num<T> =>
            lhs.Add<NUM, T>(rhs);

        /// <summary>
        /// Check if OptionAsync is in a Some state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">OptionAsync</param>
        /// <returns>True if value is in a Some state</returns>
        [Pure]
        public static ValueTask<bool> isSome<T>(OptionAsync<T> value) =>
            value.IsSome;

        /// <summary>
        /// Check if OptionAsync is in a None state
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">OptionAsync</param>
        /// <returns>True if value is in a None state</returns>
        [Pure]
        public static ValueTask<bool> isNone<T>(OptionAsync<T> value) =>
            value.IsNone;

        /// <summary>
        /// Create a Some of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made OptionAsyncal</param>
        /// <returns>`OptionAsync<T>` in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        [Pure]
        public static OptionAsync<T> SomeAsync<T>(T value) =>
            isnull(value)
                ? raise<OptionAsync<T>>(new ValueIsNullException())
                : default(MOptionAsync<T>).ReturnAsync(_ => value.AsTask());

        /// <summary>
        /// Create a lazy Some of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made OptionAsyncal</param>
        /// <returns>OptionAsync<T> in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        [Pure]
        public static OptionAsync<T> SomeAsync<T>(Task<T> taskValue) =>
            OptionAsync<T>.SomeAsync(taskValue);

        /// <summary>
        /// Create a lazy Some of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made OptionAsyncal</param>
        /// <returns>OptionAsync<T> in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        [Pure]
        public static OptionAsync<T> SomeAsync<T>(ValueTask<T> taskValue) =>
            OptionAsync<T>.SomeAsync(taskValue);

        /// <summary>
        /// Create a lazy Some of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made OptionAsyncal</param>
        /// <returns>OptionAsync<T> in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        /*[Pure]
        public static OptionAsync<Task<A>> SomeValueAsync<A>(Task<A> taskValue) =>
            OptionAsync<Task<A>>.SomeAsync(taskValue.AsTask());*/
        
        /// <summary>
        /// Create a lazy Some of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made OptionAsyncal</param>
        /// <returns>OptionAsync<T> in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        [Pure]
        public static OptionAsync<T> SomeAsync<T>(Func<Unit, ValueTask<T>> f) =>
            new (Aff(() => f(default)));
        
        /// <summary>
        /// Create a lazy Some of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made OptionAsyncal</param>
        /// <returns>OptionAsync<T> in a Some state or throws ValueIsNullException
        /// if isnull(value).</returns>
        [Pure]
        public static OptionAsync<T> SomeAsync<T>(Func<ValueTask<T>> f) =>
            new (Aff(f));

        /// <summary>
        /// Create a Some of T from a Nullable<T> (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made OptionAsyncal</param>
        /// <returns>OptionAsync<T> in a Some state or throws ValueIsNullException
        /// if isnull(value)</returns>
        [Pure]
        public static OptionAsync<T> SomeAsync<T>(T? value) where T : struct =>
            value.HasValue
                ? default(MOptionAsync<T>).ReturnAsync(_ => value.Value.AsTask())
                : raise<OptionAsync<T>>(new ValueIsNullException());

        /// <summary>
        /// Create an OptionAsync
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to be made OptionAsyncal, or null</param>
        /// <returns>If the value is null it will be None else Some(value)</returns>
        [Pure]
        public static OptionAsync<T> OptionalAsync<T>(Task<T> value) =>
            OptionAsync<T>.OptionalAsync(value);

        /// <summary>
        /// Create a lazy OptionAsync of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="f">A function that returns the value to construct the OptionAsync with</param>
        /// <returns>A lazy OptionAsync<T></returns>
        [Pure]
        public static OptionAsync<T> OptionalAsync<T>(Func<ValueTask<T>> f) =>
            new (Aff(f));

        /// <summary>
        /// Create a lazy OptionAsync of T (OptionAsync<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="f">A function that returns the value to construct the OptionAsync with</param>
        /// <returns>A lazy OptionAsync<T></returns>
        [Pure]
        public static OptionAsync<T> OptionalAsync<T>(Func<Unit, ValueTask<T>> f) =>
            new (Aff(() => f(default)));

        /// <summary>
        /// Create an OptionAsync
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to be made OptionAsyncal, or null</param>
        /// <returns>If the value is null it will be None else Some(value)</returns>
        [Pure]
        public static OptionAsync<T> OptionalAsync<T>(T? value) where T : struct =>
            value.HasValue
                ? default(MOptionAsync<T>).ReturnAsync(_ => value.Value.AsTask())
                : OptionAsync<T>.None;

        /// <summary>
        /// Invokes the action if OptionAsync is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">Action to invoke if OptionAsync is in the Some state</param>
        public static ValueTask<Unit> ifSome<T>(OptionAsync<T> OptionAsync, Action<T> Some) => 
            OptionAsync.IfSome(Some);

        /// <summary>
        /// Returns the result of invoking the None() operation if the OptionAsyncal 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the OptionAsyncal 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public static ValueTask<T> ifNone<T>(OptionAsync<T> ma, Func<T> None) =>
            ma.IfNone(None);

        /// <summary>
        /// Returns the noneValue if the OptionAsyncal is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the OptionAsyncal is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public static ValueTask<T> ifNone<T>(OptionAsync<T> ma, T noneValue) =>
            ma.IfNone(noneValue);

        /// <summary>
        /// Returns the result of invoking the None() operation if the OptionAsyncal 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow null the be returned from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the OptionAsyncal 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public static ValueTask<T?> ifNoneUnsafe<T>(OptionAsync<T> ma, Func<T?> None) =>
            ma.IfNoneUnsafe(None);

        /// <summary>
        /// Returns the noneValue if the OptionAsyncal is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow noneValue to be null</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the OptionAsyncal is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public static ValueTask<T?> ifNoneUnsafe<T>(OptionAsync<T> ma, T noneValue) =>
            ma.IfNoneUnsafe(noneValue);

        /// <summary>
        /// Match the two states of the OptionAsync and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public static ValueTask<R> match<T, R>(OptionAsync<T> ma, Func<T, R> Some, Func<R> None) =>
            ma.Match(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public static ValueTask<R> matchAsync<T, R>(OptionAsync<T> ma, Func<T, ValueTask<R>> Some, Func<R> None) =>
            ma.MatchAsync(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public static ValueTask<R> matchAsync<T, R>(OptionAsync<T> ma, Func<T, R> Some, Func<ValueTask<R>> None) =>
            ma.MatchAsync(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null B</returns>
        [Pure]
        public static ValueTask<R> matchAsync<T, R>(OptionAsync<T> ma, Func<T, ValueTask<R>> Some, Func<ValueTask<R>> None) =>
            ma.MatchAsync(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public static ValueTask<R?> matchUnsafe<T, R>(OptionAsync<T> ma, Func<T, R?> Some, Func<R?> None) =>
            ma.MatchUnsafe(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public static ValueTask<R?> matchUnsafeAsync<T, R>(OptionAsync<T> ma, Func<T, ValueTask<R?>> Some, Func<R?> None) =>
            ma.MatchUnsafeAsync(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public static ValueTask<R?> matchUnsafeAsync<T, R>(OptionAsync<T> ma, Func<T, R> Some, Func<ValueTask<R?>> None) =>
            ma.MatchUnsafeAsync(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public static ValueTask<R?> matchUnsafeAsync<T, R>(OptionAsync<T> ma, Func<T, ValueTask<R?>> Some, Func<ValueTask<R?>> None) =>
            ma.MatchUnsafeAsync(Some, None);

        /// <summary>
        /// Match the two states of the OptionAsync
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        public static ValueTask<Unit> match<T>(OptionAsync<T> ma, Action<T> Some, Action None) =>
            ma.Match(Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
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
        /// <param name="folder">Folder function, applied if OptionAsync is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> fold<S, A>(OptionAsync<A> ma, S state, Func<S, A, S> folder) =>
            ma.Fold(state, folder);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
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
        /// <param name="folder">Folder function, applied if OptionAsync is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> fold<S, A>(OptionAsync<A> ma, S state, Func<S, A, ValueTask<S>> folder) =>
            ma.FoldAsync(state, folder);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, S> Some, Func<S, S> None) =>
            ma.BiFold(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, ValueTask<S>> Some, Func<S, S> None) =>
            ma.BiFoldAsync(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, S> Some, Func<S, ValueTask<S>> None) =>
            ma.BiFoldAsync(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, ValueTask<S>> Some, Func<S, ValueTask<S>> None) =>
            ma.BiFoldAsync(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            ma.BiFold(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, ValueTask<S>> Some, Func<S, Unit, S> None) =>
            ma.BiFold(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, ValueTask<S>> None) =>
            ma.BiFoldAsync(state, Some, None);

        /// <summary>
        /// <para>
        /// OptionAsync types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionAsync is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionAsync is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public static ValueTask<S> bifold<S, A>(OptionAsync<A> ma, S state, Func<S, A, ValueTask<S>> Some, Func<S, Unit, ValueTask<S>> None) =>
            ma.BiFoldAsync(state, Some, None);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionAsync is in a None state
        /// then True is returned (because the predicate applies for-all values).
        /// If the OptionAsync is in a Some state the value is the result of running 
        /// applying the bound value to the predicate supplied.        
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>If the OptionAsync is in a None state then True is returned (because 
        /// the predicate applies for-all values).  If the OptionAsync is in a Some state
        /// the value is the result of running applying the bound value to the 
        /// predicate supplied.</returns>
        [Pure]
        public static ValueTask<bool> forall<A>(OptionAsync<A> ma, Func<A, bool> pred) =>
            ma.ForAll(pred);

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
        public static ValueTask<int> count<A>(OptionAsync<A> ma) =>
            ma.Count();

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionAsync is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionAsync is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>If the OptionAsync is in a None state then True is returned if 
        /// invoking None returns True. If the OptionAsync is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public static ValueTask<bool> exists<A>(OptionAsync<A> ma, Func<A, bool> pred) =>
            ma.Exists(pred);

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public static OptionAsync<B> map<A, B>(OptionAsync<A> ma, Func<A, B> f) =>
            ma.Map(f);


        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the OptionAsync is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public static OptionAsync<T> filter<T>(OptionAsync<T> ma, Func<T, bool> pred) =>
            ma.Filter(pred);

        /// <summary>
        /// Monadic bind operation
        /// </summary>
        [Pure]
        public static OptionAsync<R> bind<T, R>(OptionAsync<T> ma, Func<T, OptionAsync<R>> binder) =>
            ma.Bind(binder);

        /// <summary>
        /// Match the two states of the list of OptionAsyncs
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        [Pure]
        public static async ValueTask<IEnumerable<R>> match<T, R>(IEnumerable<OptionAsync<T>> list,
            Func<T, IEnumerable<R>> Some,
            Func<IEnumerable<R>> None
            )
        {
            IEnumerable<ValueTask<IEnumerable<R>>> Yield()
            {
                foreach (var item in list)
                {
                    yield return item.Match(Some, None);
                }
            }

            var results = new List<R>();
            foreach (var item in Yield())
            {
                foreach(var inner in await item.ConfigureAwait(false))
                {
                    results.Add(inner);
                }
            }
            return results.AsEnumerable();
        }

        /// <summary>
        /// Match the two states of the list of OptionAsyncs
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        [Pure]
        public static ValueTask<IEnumerable<R>> match<T, R>(IEnumerable<OptionAsync<T>> list,
            Func<T, IEnumerable<R>> Some,
            IEnumerable<R> None) =>
            match(list, Some, () => None);

        /// <summary>
        /// Extracts from a list of 'OptionAsync' all the 'Some' elements.
        /// All the 'Some' elements are extracted in order.
        /// </summary>
        [Pure]
        public static ValueTask<IEnumerable<T>> somes<T>(IEnumerable<OptionAsync<T>> list) =>
            list.Somes();

        /// <summary>
        /// Convert the OptionAsync to an immutable list of zero or one items
        /// </summary>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public static ValueTask<Lst<T>> toList<T>(OptionAsync<T> OptionAsync) =>
            OptionAsync.ToList();

        /// <summary>
        /// Convert the OptionAsync to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static ValueTask<Arr<T>> toArray<T>(OptionAsync<T> OptionAsync) =>
            OptionAsync.ToArray();
    }
}
