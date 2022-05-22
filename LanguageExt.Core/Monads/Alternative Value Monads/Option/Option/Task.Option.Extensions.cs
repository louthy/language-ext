#nullable enable
using System;
using static LanguageExt.OptionalAsync;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.TypeClasses;
using System.Collections.Generic;

public static class TaskOptionAsyncExtensions
{
    public static OptionAsync<A> ToAsync<A>(this Task<Option<A>> ma) =>
        #nullable disable
        new OptionAsync<A>(ma.Map(a => (a.IsSome, a.Value)));
        #nullable enable

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="f">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> MapAsync<A, B>(this Task<Option<A>> self, Func<A, B> f) =>
        default(FOptionAsync<A, B>).Map(self.ToAsync(), f);

    /// <summary>
    /// Projection from one value to another 
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="f">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> MapAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> f) =>
        default(FOptionAsync<A, B>).MapAsync(self.ToAsync(), f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static OptionAsync<B> BindAsync<A, B>(this Task<Option<A>> self, Func<A, Option<B>> f) =>
        default(MOptionAsync<A>).Bind<MOptionAsync<B>, OptionAsync<B>, B>(self.ToAsync(), a => f(a).ToAsync());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static OptionAsync<B> BindAsync<A, B>(this Task<Option<A>> self, Func<A, Task<Option<B>>> f) =>
        default(MOptionAsync<A>).BindAsync<MOptionAsync<B>, OptionAsync<B>, B>(self.ToAsync(), async a => (await f(a).ConfigureAwait(false)).ToAsync());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static OptionAsync<B> BindAsync<A, B>(this Task<Option<A>> self, Func<A, OptionAsync<B>> f) =>
        default(MOptionAsync<A>).Bind<MOptionAsync<B>, OptionAsync<B>, B>(self.ToAsync(), f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    [Pure]
    public static OptionAsync<B> BindAsync<A, B>(this Task<Option<A>> self, Func<A, Task<OptionAsync<B>>> f) =>
        default(MOptionAsync<A>).BindAsync<MOptionAsync<B>, OptionAsync<B>, B>(self.ToAsync(), f);

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    public static Task<R> MatchUntypedAsync<A, R>(this Task<Option<A>> self, Func<object, R> Some, Func<R> None) =>
        matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(self.ToAsync(), Some, None);

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    public static Task<R> MatchUntypedAsync<A, R>(this Task<Option<A>> self, Func<object, Task<R>> Some, Func<R> None) =>
        matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(self.ToAsync(), Some, None);

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    public static Task<R> MatchUntypedAsync<A, R>(this Task<Option<A>> self, Func<object, R> Some, Func<Task<R>> None) =>
        matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(self.ToAsync(), Some, None);

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    public static Task<R> MatchUntypedAsync<A, R>(this Task<Option<A>> self, Func<object, Task<R>> Some, Func<Task<R>> None) =>
        matchUntypedAsync<MOptionAsync<A>, OptionAsync<A>, A, R>(self.ToAsync(), Some, None);

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    public static Task<Arr<A>> ToArrayAsync<A>(this Task<Option<A>> self) =>
        toArrayAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync());

    /// <summary>
    /// Convert the Option to an immutable list of zero or one items
    /// </summary>
    /// <returns>An immutable list of zero or one items</returns>
    [Pure]
    public static Task<Lst<A>> ToListAsync<A>(this Task<Option<A>> self) =>
        toListAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync());

    /// <summary>
    /// Convert the Option to an enumerable sequence of zero or one items
    /// </summary>
    /// <returns>An enumerable sequence of zero or one items</returns>
    [Pure]
    public static Task<Seq<A>> ToSeqAsync<A>(this Task<Option<A>> self) =>
        toSeqAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync());

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    public static Task<IEnumerable<A>> AsEnumerableAsync<A>(this Task<Option<A>> self) =>
        asEnumerableAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync());

    /// <summary>
    /// Convert the structure to an Either
    /// </summary>
    /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
    /// <returns>An Either representation of the structure</returns>
    [Pure]
    public static EitherAsync<L, A> ToEitherAsync<L, A>(this Task<Option<A>> self, L defaultLeftValue) =>
        toEitherAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(self.ToAsync(), defaultLeftValue);

    /// <summary>
    /// Convert the structure to an Either
    /// </summary>
    /// <param name="defaultLeftValue">Function to invoke to get a default value if the 
    /// structure is in a None state</param>
    /// <returns>An Either representation of the structure</returns>
    [Pure]
    public static EitherAsync<L, A> ToEitherAsync<L, A>(this Task<Option<A>> self, Func<L> Left) =>
        #nullable disable
        toEitherAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(new OptionAsync<A>(self.Map(x => (x.IsSome, x.Value))), Left);
        #nullable enable

    /// <summary>
    /// Convert the structure to an EitherUnsafe
    /// </summary>
    /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
    /// <returns>An EitherUnsafe representation of the structure</returns>
    [Pure]
    public static Task<EitherUnsafe<L, A>> ToEitherUnsafeAsync<L, A>(this Task<Option<A>> self, L? defaultLeftValue) =>
        #nullable disable
        toEitherUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(new OptionAsync<A>(self.Map(x => (x.IsSome, x.Value))), defaultLeftValue);
        #nullable enable

    /// <summary>
    /// Convert the structure to an EitherUnsafe
    /// </summary>
    /// <param name="defaultLeftValue">Function to invoke to get a default value if the 
    /// structure is in a None state</param>
    /// <returns>An EitherUnsafe representation of the structure</returns>
    [Pure]
    public static Task<EitherUnsafe<L, A>> ToEitherUnsafeAsync<L, A>(this Task<Option<A>> self, Func<L?> Left) =>
        #nullable disable
        toEitherUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, L, A>(self.ToAsync(), Left);
        #nullable enable

    /// <summary>
    /// Convert the structure to a OptionUnsafe
    /// </summary>
    /// <returns>An OptionUnsafe representation of the structure</returns>
    [Pure]
    public static Task<OptionUnsafe<A>> ToOptionUnsafeAsync<A>(this Task<Option<A>> self) =>
        toOptionUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync());

    /// <summary>
    /// Convert the structure to a TryOptionAsync
    /// </summary>
    /// <returns>A TryOptionAsync representation of the structure</returns>
    [Pure]
    public static TryOptionAsync<A> ToTryOptionAsync<A>(this Task<Option<A>> self) =>
        toTryOptionAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync());

    /// <summary>
    /// Convert the structure to a TryAsync
    /// </summary>
    /// <returns>A TryAsync representation of the structure</returns>
    [Pure]
    public static TryAsync<A> ToTryAsync<A>(this Task<Option<A>> self) =>
        toTryAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync());

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. Must not return null.</param>
    /// <param name="None">None match operation. Must not return null.</param>
    /// <returns>A non-null B</returns>
    [Pure]
    public static Task<B> Match<A, B>(this Task<Option<A>> self, Func<A, B> Some, Func<B> None) =>
        MOptionAsync<A>.Inst.Match(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. Must not return null.</param>
    /// <param name="None">None match operation. Must not return null.</param>
    /// <returns>A non-null B</returns>
    [Pure]
    public static Task<B> MatchAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> Some, Func<B> None) =>
        MOptionAsync<A>.Inst.MatchAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. Must not return null.</param>
    /// <param name="None">None match operation. Must not return null.</param>
    /// <returns>A non-null B</returns>
    [Pure]
    public static Task<B> MatchAsync<A, B>(this Task<Option<A>> self, Func<A, B> Some, Func<Task<B>> None) =>
        MOptionAsync<A>.Inst.MatchAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option and return a non-null R.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. Must not return null.</param>
    /// <param name="None">None match operation. Must not return null.</param>
    /// <returns>A non-null B</returns>
    [Pure]
    public static Task<B> MatchAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> Some, Func<Task<B>> None) =>
        MOptionAsync<A>.Inst.MatchAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option and return a B, which can be null.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. May return null.</param>
    /// <param name="None">None match operation. May return null.</param>
    /// <returns>B, or null</returns>
    [Pure]
    public static Task<B?> MatchUnsafe<A, B>(this Task<Option<A>> self, Func<A, B?> Some, Func<B?> None) =>
        MOptionAsync<A>.Inst.MatchUnsafe(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option and return a B, which can be null.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. May return null.</param>
    /// <param name="None">None match operation. May return null.</param>
    /// <returns>B, or null</returns>
    [Pure]
    public static Task<B?> MatchUnsafeAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B?>> Some, Func<B?> None) =>
        MOptionAsync<A>.Inst.MatchUnsafeAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option and return a B, which can be null.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. May return null.</param>
    /// <param name="None">None match operation. May return null.</param>
    /// <returns>B, or null</returns>
    [Pure]
    public static Task<B?> MatchUnsafeAsync<A, B>(this Task<Option<A>> self, Func<A, B?> Some, Func<Task<B?>> None) =>
        MOptionAsync<A>.Inst.MatchUnsafeAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option and return a B, which can be null.
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Some">Some match operation. May return null.</param>
    /// <param name="None">None match operation. May return null.</param>
    /// <returns>B, or null</returns>
    [Pure]
    public static Task<B?> MatchUnsafeAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B?>> Some, Func<Task<B?>> None) =>
        MOptionAsync<A>.Inst.MatchUnsafeAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Match the two states of the Option
    /// </summary>
    /// <param name="Some">Some match operation</param>
    /// <param name="None">None match operation</param>
    public static Task<Unit> Match<A>(this Task<Option<A>> self, Action<A> Some, Action None) =>
        MOptionAsync<A>.Inst.Match(self.ToAsync(), Some, None);

    /// <summary>
    /// Invokes the action if Option is in the Some state, otherwise nothing happens.
    /// </summary>
    /// <param name="f">Action to invoke if Option is in the Some state</param>
    public static Task<Unit> IfSomeAsync<A>(this Task<Option<A>> self, Action<A> f) =>
        ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), f);

    /// <summary>
    /// Invokes the f function if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    /// <param name="f">Function to invoke if Option is in the Some state</param>
    public static Task<Unit> IfSomeAsync<A>(this Task<Option<A>> self, Func<A, Task<Unit>> f) =>
        ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), f);

    /// <summary>
    /// Invokes the f function if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    /// <param name="f">Function to invoke if Option is in the Some state</param>
    public static Task<Unit> IfSomeAsync<A>(this Task<Option<A>> self, Func<A, Task> f) =>
        ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), f);

    /// <summary>
    /// Invokes the f function if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    /// <param name="f">Function to invoke if Option is in the Some state</param>
    public static Task<Unit> IfSomeAsync<A>(this Task<Option<A>> self, Func<A, Unit> f) =>
        ifSomeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), f);

    /// <summary>
    /// Returns the result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null return value from the None operation</remarks>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Tesult of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    public static Task<A> IfNoneAsync<A>(this Task<Option<A>> self, Func<A> None) =>
        ifNoneAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), None);

    /// <summary>
    /// Returns the result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null return value from the None operation</remarks>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Tesult of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    public static Task<A> IfNoneAsync<A>(this Task<Option<A>> self, Func<Task<A>> None) =>
        ifNoneAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), None);

    /// <summary>
    /// Returns the noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null noneValue</remarks>
    /// <param name="noneValue">Value to return if in a None state</param>
    /// <returns>noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned</returns>
    [Pure]
    public static Task<A> IfNoneAsync<A>(this Task<Option<A>> self, A noneValue) =>
        ifNoneAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), noneValue);

    /// <summary>
    /// Returns the result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will allow null the be returned from the None operation</remarks>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Tesult of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    public static Task<A?> IfNoneUnsafeAsync<A>(this Task<Option<A>> self, Func<A?> None) =>
        #nullable disable
        OptionalUnsafeAsync.ifNoneUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), None);
        #nullable enable

    /// <summary>
    /// Returns the result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will allow null the be returned from the None operation</remarks>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Tesult of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    public static Task<A?> IfNoneUnsafeAsync<A>(this Task<Option<A>> self, Func<Task<A?>> None) =>
        #nullable disable
        OptionalUnsafeAsync.ifNoneUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), None);
        #nullable enable

    /// <summary>
    /// Returns the noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will allow noneValue to be null</remarks>
    /// <param name="noneValue">Value to return if in a None state</param>
    /// <returns>noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned</returns>
    [Pure]
    public static Task<A?> IfNoneUnsafeAsync<A>(this Task<Option<A>> self, A? noneValue) =>
        #nullable disable
        OptionalUnsafeAsync.ifNoneUnsafeAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), noneValue);
        #nullable enable

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
    public static Task<S> FoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, S> folder) =>
        MOptionAsync<A>.Inst.Fold(self.ToAsync(), state, folder)(unit);

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
    public static Task<S> FoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, Task<S>> folder) =>
        MOptionAsync<A>.Inst.FoldAsync(self.ToAsync(), state, folder)(unit);

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
    public static Task<S> FoldBackAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, S> folder) =>
        MOptionAsync<A>.Inst.FoldBack(self.ToAsync(), state, folder)(unit);

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
    public static Task<S> FoldBackAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, Task<S>> folder) =>
        MOptionAsync<A>.Inst.FoldBackAsync(self.ToAsync(), state, folder)(unit);

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
        MOptionAsync<A>.Inst.BiFold(self.ToAsync(), state, Some, None);

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, Task<S>> Some, Func<S, Unit, S> None) =>
        MOptionAsync<A>.Inst.BiFoldAsync(self.ToAsync(), state, Some, None);

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> None) =>
        MOptionAsync<A>.Inst.BiFoldAsync(self.ToAsync(), state, Some, None);

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, Task<S>> Some, Func<S, Unit, Task<S>> None) =>
        MOptionAsync<A>.Inst.BiFoldAsync(self.ToAsync(), state, Some, None);

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, S> Some, Func<S, S> None) =>
        MOptionAsync<A>.Inst.BiFold(self.ToAsync(), state, Some, (s, _) => None(s));

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, Task<S>> Some, Func<S, S> None) =>
        MOptionAsync<A>.Inst.BiFoldAsync(self.ToAsync(), state, Some, (s, _) => None(s));

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, S> Some, Func<S, Task<S>> None) =>
        MOptionAsync<A>.Inst.BiFoldAsync(self.ToAsync(), state, Some, (s, _) => None(s));

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
    public static Task<S> BiFoldAsync<S, A>(this Task<Option<A>> self, S state, Func<S, A, Task<S>> Some, Func<S, Task<S>> None) =>
        MOptionAsync<A>.Inst.BiFoldAsync(self.ToAsync(), state, Some, (s, _) => None(s));

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, B> Some, Func<Unit, B> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> Some, Func<Unit, B> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> Some, Func<Unit, Task<B>> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, B> Some, Func<Unit, Task<B>> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, None);

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, B> Some, Func<B> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, _ => None());

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> Some, Func<B> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, _ => None());

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, Task<B>> Some, Func<Task<B>> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, async _ => await None().ConfigureAwait(false));

    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <typeparam name="B">Resulting functor value type</typeparam>
    /// <param name="Some">Projection function</param>
    /// <param name="None">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static OptionAsync<B> BiMapAsync<A, B>(this Task<Option<A>> self, Func<A, B> Some, Func<Task<B>> None) =>
        default(FOptionAsync<A, B>).BiMapAsync(self.ToAsync(), Some, async _ => await None().ConfigureAwait(false));

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
    public static Task<int> CountAsync<A>(this Task<Option<A>> self) =>
        MOptionAsync<A>.Inst.Count(self.ToAsync())(unit);

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
    public static Task<bool> ForAllAsync<A>(this Task<Option<A>> self, Func<A, bool> pred) =>
        forallAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), pred);

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
    public static Task<bool> BiForAllAsync<A>(this Task<Option<A>> self, Func<A, bool> Some, Func<Unit, bool> None) =>
        biForAllAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(self.ToAsync(), Some, None);

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
    public static Task<bool> BiForAllAsync<A>(this Task<Option<A>> self, Func<A, bool> Some, Func<bool> None) =>
        biForAllAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(self.ToAsync(), Some, _ => None());

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
    public static Task<bool> ExistsAsync<A>(this Task<Option<A>> self, Func<A, bool> pred) =>
        existsAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), pred);

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
    public static Task<bool> BiExistsAsync<A>(this Task<Option<A>> self, Func<A, bool> Some, Func<Unit, bool> None) =>
        biExistsAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(self.ToAsync(), Some, None);

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
    public static Task<bool> BiExistsAsync<A>(this Task<Option<A>> self, Func<A, bool> Some, Func<bool> None) =>
        biExistsAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(self.ToAsync(), Some, _ => None());

    /// <summary>
    /// Invoke an action for the bound value (if in a Some state)
    /// </summary>
    /// <param name="Some">Action to invoke</param>
    [Pure]
    public static Task<Unit> IterAsync<A>(this Task<Option<A>> self, Action<A> Some) =>
        iterAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), Some);

    /// <summary>
    /// Invoke an action for the bound value (if in a Some state)
    /// </summary>
    /// <param name="Some">Action to invoke</param>
    [Pure]
    public static Task<Unit> IterAsync<A>(this Task<Option<A>> self, Func<A, Task<Unit>> Some) =>
        iterAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), Some);

    /// <summary>
    /// Invoke an action depending on the state of the Option
    /// </summary>
    /// <param name="Some">Action to invoke if in a Some state</param>
    /// <param name="None">Action to invoke if in a None state</param>
    [Pure]
    public static Task<Unit> BiIterAsync<A>(this Task<Option<A>> self, Action<A> Some, Action<Unit> None) =>
        biIterAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(self.ToAsync(), Some, None);

    /// <summary>
    /// Invoke an action depending on the state of the Option
    /// </summary>
    /// <param name="Some">Action to invoke if in a Some state</param>
    /// <param name="None">Action to invoke if in a None state</param>
    [Pure]
    public static Task<Unit> BiIterAsync<A>(this Task<Option<A>> self, Action<A> Some, Action None) =>
        biIterAsync<MOptionAsync<A>, OptionAsync<A>, A, Unit>(self.ToAsync(), Some, _ => None());

    /// <summary>
    /// Apply a predicate to the bound value (if in a Some state)
    /// </summary>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Some(x) if the Option is in a Some state and the predicate
    /// returns True.  None otherwise.</returns>
    [Pure]
    public static OptionAsync<A> FilterAsync<A>(this Task<Option<A>> self, Func<A, bool> pred) =>
        filterAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), pred);

    /// <summary>
    /// Apply a predicate to the bound value (if in a Some state)
    /// </summary>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Some(x) if the Option is in a Some state and the predicate
    /// returns True.  None otherwise.</returns>
    [Pure]
    public static OptionAsync<A> FilterAsync<A>(this Task<Option<A>> self, Func<A, Task<bool>> pred) =>
        filterAsync<MOptionAsync<A>, OptionAsync<A>, A>(self.ToAsync(), pred);

    /// <summary>
    /// Add the bound values of x and y, uses an Add type-class to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Add of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y added to x</returns>
    [Pure]
    public static OptionAsync<A> AddAsync<NUM, A>(this Task<Option<A>> x, Task<Option<A>> y) where NUM : struct, Num<A> =>
        x.ToAsync().Add<NUM, A>(y.ToAsync());

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Subtract of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the subtract between x and y</returns>
    [Pure]
    public static OptionAsync<A> SubtractAsync<NUM, A>(this Task<Option<A>> x, Task<Option<A>> y) where NUM : struct, Num<A> =>
        x.ToAsync().Subtract<NUM, A>(y.ToAsync());

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product type-class 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Product of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the product of x and y</returns>
    [Pure]
    public static OptionAsync<A> ProductAsync<NUM, A>(this Task<Option<A>> x, Task<Option<A>> y) where NUM : struct, Num<A> =>
        x.ToAsync().Product<NUM, A>(y.ToAsync());

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
    /// operation for type A.  For example x.Divide<TDouble,double>(y)
    /// </summary>
    /// <typeparam name="NUM">Divide of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option x / y</returns>
    [Pure]
    public static OptionAsync<A> DivideAsync<NUM, A>(this Task<Option<A>> x, Task<Option<A>> y) where NUM : struct, Num<A> =>
        x.ToAsync().Divide<NUM, A>(y.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static OptionAsync<B> ApplyAsync<A, B>(this Option<Func<A, B>> fab, Task<Option<A>> fa) =>
        ApplOptionAsync<A, B>.Inst.Apply(fab.ToAsync(), fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static OptionAsync<B> ApplyAsync<A, B>(this Func<A, B> fab, Task<Option<A>> fa) =>
        ApplOptionAsync<A, B>.Inst.Apply(fab, fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static OptionAsync<C> ApplyAsync<A, B, C>(this Option<Func<A, B, C>> fabc, Task<Option<A>> fa, Option<B> fb) =>
        from x in fabc.ToAsync()
        from y in ApplOptionAsync<A, B, C>.Inst.Apply(curry(x), fa.ToAsync(), fb.ToAsync())
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static OptionAsync<C> ApplyAsync<A, B, C>(this Func<A, B, C> fabc, Task<Option<A>> fa, Option<B> fb) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(curry(fabc), fa.ToAsync(), fb.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> ApplyAsync<A, B, C>(this Option<Func<A, B, C>> fabc, Task<Option<A>> fa) =>
        from x in fabc.ToAsync()
        from y in ApplOptionAsync<A, B, C>.Inst.Apply(curry(x), fa.ToAsync())
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> ApplyAsync<A, B, C>(this Func<A, B, C> fabc, Task<Option<A>> fa) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(curry(fabc), fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> ApplyAsync<A, B, C>(this Option<Func<A, Func<B, C>>> fabc, Task<Option<A>> fa) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(fabc.ToAsync(), fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static OptionAsync<Func<B, C>> ApplyAsync<A, B, C>(this Func<A, Func<B, C>> fabc, Task<Option<A>> fa) =>
        ApplOptionAsync<A, B, C>.Inst.Apply(fabc, fa.ToAsync());

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type OptionAsync<B></returns>
    [Pure]
    public static OptionAsync<B> ActionAsync<A, B>(this Task<Option<A>> fa, Option<B> fb) =>
        ApplOptionAsync<A, B>.Inst.Action(fa.ToAsync(), fb.ToAsync());

}
