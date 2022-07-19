using System;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.DataTypes.Serialisation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// EitherAsync L R
    /// Holds one of two values 'Left' or 'Right'.  Usually 'Left' is considered 'wrong' or 'in error', and
    /// 'Right' is, well, right.  So when the Either is in a Left state, it cancels computations like bind
    /// or map, etc.  So you can see Left as an 'early out, with a message'.  Unlike Option that has None
    /// as its alternative value (i.e. it has an 'early out, but no message').
    /// </summary>
    /// <remarks>
    /// NOTE: If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
    /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
    /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
    /// of filtering Either.
    /// 
    /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
    /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
    /// doesn't needlessly break expressions. 
    /// </remarks>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    [Serializable]
    //[AsyncMethodBuilder(typeof(EitherAsyncBuilder<,>))]
    public readonly struct EitherAsync<L, R> :
        IAsyncEnumerable<R>,
        IEitherAsync
    {
        public readonly static EitherAsync<L, R> Bottom = new EitherAsync<L, R>();
        internal readonly Task<EitherData<L, R>> data;

        internal EitherAsync(Task<EitherData<L, R>> data) =>
            this.data = data ?? EitherData<L, R>.Bottom.AsTask();

        internal Task<EitherData<L, R>> Data =>
            data ?? EitherData<L, R>.Bottom.AsTask();

        /// <summary>
        /// State of the Either
        /// You can also use:
        ///     IsRight
        ///     IsLeft
        ///     IsBottom
        /// </summary>
        public Task<EitherStatus> State =>
            Data.Select(x => x.State);

        /// <summary>
        /// Is the Either in a Right state?
        /// </summary>
        [Pure]
        public Task<bool> IsRight =>
            Data.Map(x => x.State == EitherStatus.IsRight);

        /// <summary>
        /// Is the Either in a Left state?
        /// </summary>
        [Pure]
        public Task<bool> IsLeft =>
            Data.Map(x => x.State == EitherStatus.IsLeft);

        /// <summary>
        /// Is the Either in a Bottom state?
        /// When the Either is filtered, both Right and Left are meaningless.
        /// 
        /// If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
        /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
        /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
        /// of filtering Either.
        /// 
        /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
        /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
        /// doesn't needlessly break expressions. 
        /// </summary>
        [Pure]
        public Task<bool> IsBottom =>
            Data.Map(x => x.State == EitherStatus.IsBottom);

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public ValueTask<object> Case =>
            GetCase();

        [Pure]
        async ValueTask<object> GetCase()
        {
            var data = await Data.ConfigureAwait(false);
            return data.State switch
            {
                EitherStatus.IsRight => data.Right,
                EitherStatus.IsLeft  => data.Left,
                _                    => null
            };
        }

        /// <summary>
        /// Custom awaiter that turns an EitherAsync into an Either
        /// </summary>
        public TaskAwaiter<Either<L, R>> GetAwaiter() =>
            Data.Map(d => d.State switch
            {
                EitherStatus.IsRight => Either<L, R>.Right(d.Right),
                EitherStatus.IsLeft  => Either<L, R>.Left(d.Left),
                _                    => Either<L, R>.Bottom
            }).GetAwaiter();

        /// <summary>
        /// Implicit conversion operator from R to Either R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static implicit operator EitherAsync<L, R>(R value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : Right(value);

        /// <summary>
        /// Implicit conversion operator from R to Either R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static implicit operator EitherAsync<L, R>(Task<R> value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : RightAsync(value);

        /// <summary>
        /// Implicit conversion operator from L to Either R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static implicit operator EitherAsync<L, R>(L value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : Left(value);

        /// <summary>
        /// Implicit conversion operator from L to Either R L
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static implicit operator EitherAsync<L, R>(Task<L> value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : LeftAsync(value);
        
        /// <summary>
        /// Equality operator
        /// </summary>
        public override bool Equals(object _) =>
            throw new NotSupportedException(
                "The standard Equals override is not supported for EitherAsync because it's an asynchronous type and " +
                "the return value is synchronous.  Use the typed version of Equals or the == operator to get a bool " +
                " Task that can be awaited");
        
        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public async Task<bool> Equals<EqL, EqR>(EitherAsync<L, R> rhs) 
            where EqL : struct, EqAsync<L>
            where EqR : struct, EqAsync<R>
        {
            var a = await Data.ConfigureAwait(false);
            var b = await rhs.Data.ConfigureAwait(false);
            return a.State == b.State &&
                   await default(EqL).EqualsAsync(a.Left, b.Left).ConfigureAwait(false) &&
                   await default(EqR).EqualsAsync(a.Right, b.Right).ConfigureAwait(false);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public Task<bool> Equals(EitherAsync<L, R> rhs) =>
            Equals<EqDefaultAsync<L>, EqDefaultAsync<R>>(rhs);

        /// <summary>
        /// Equality operator
        /// </summary>
        [Pure]
        public static Task<bool> operator ==(EitherAsync<L, R> lhs, EitherAsync<L, R> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        [Pure]
        public static Task<bool> operator !=(EitherAsync<L, R> lhs, EitherAsync<L, R> rhs) =>
            lhs.Equals(rhs).Map(not);

        /// <summary>
        /// Ordering
        /// </summary>
        [Pure]
        public async Task<int> CompareTo<OrdL, OrdR>(EitherAsync<L, R> rhs) 
            where OrdL : struct, Ord<L>
            where OrdR : struct, Ord<R>
        {
            var a = await Data.ConfigureAwait(false);
            var b = await rhs.Data.ConfigureAwait(false);
            var c = default(OrdInt).Compare((int)a.State, (int)b.State);
            if (c != 0) return c;
            c = default(OrdL).Compare(a.Left, b.Left);
            if (c != 0) return c;
            return default(OrdR).Compare(a.Right, b.Right);
        }

        /// <summary>
        /// Ordering
        /// </summary>
        [Pure]
        public Task<int> CompareTo(EitherAsync<L, R> rhs) =>
            CompareTo<OrdDefault<L>, OrdDefault<R>>(rhs);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator < (EitherAsync<L, R> lhs, EitherAsync<L, R> rhs) =>
            lhs.CompareTo(rhs).Map(x => x < 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator <= (EitherAsync<L, R> lhs, EitherAsync<L, R> rhs) =>
            lhs.CompareTo(rhs).Map(x => x <= 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator > (EitherAsync<L, R> lhs, EitherAsync<L, R> rhs) =>
            lhs.CompareTo(rhs).Map(x => x > 0);
        
        /// <summary>
        /// Ordering operator
        /// </summary>
        [Pure]
        public static Task<bool> operator >= (EitherAsync<L, R> lhs, EitherAsync<L, R> rhs) =>
            lhs.CompareTo(rhs).Map(x => x >= 0);
        

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> Match<Ret>(Func<R, Ret> Right, Func<L, Ret> Left, Func<Ret> Bottom = null) =>
            Check.NullReturn(await MatchUnsafe(Right, Left, Bottom).ConfigureAwait(false));

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="LeftAsync">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> MatchAsync<Ret>(Func<R, Ret> Right, Func<L, Task<Ret>> LeftAsync, Func<Ret> Bottom = null) =>
            Check.NullReturn(await MatchUnsafeAsync(Right, LeftAsync, Bottom).ConfigureAwait(false));

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="RightAsync">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> MatchAsync<Ret>(Func<R, Task<Ret>> RightAsync, Func<L, Ret> Left, Func<Ret> Bottom = null) =>
            Check.NullReturn(await MatchUnsafeAsync(RightAsync, Left, Bottom).ConfigureAwait(false));

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="RightAsync">Function to invoke if in a Right state</param>
        /// <param name="LeftAsync">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> MatchAsync<Ret>(Func<R, Task<Ret>> RightAsync, Func<L, Task<Ret>> LeftAsync, Func<Ret> Bottom = null) =>
            Check.NullReturn(await MatchUnsafeAsync(RightAsync, LeftAsync, Bottom).ConfigureAwait(false));

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> MatchUnsafe<Ret>(Func<R, Ret> Right, Func<L, Ret> Left, Func<Ret> Bottom = null) =>
            await IsBottom.ConfigureAwait(false)
                ? Bottom == null
                    ? throw new BottomException()
                    : Bottom()
                : await IsLeft.ConfigureAwait(false)
                    ? Left == null
                        ? throw new ArgumentNullException(nameof(Left))
                        : Left((await Data.ConfigureAwait(false)).Left)
                    : Right == null
                        ? throw new ArgumentNullException(nameof(Right))
                        : Right((await Data.ConfigureAwait(false)).Right);

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="RightAsync">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> MatchUnsafeAsync<Ret>(Func<R, Task<Ret>> RightAsync, Func<L, Ret> Left, Func<Ret> Bottom = null) =>
            await IsBottom.ConfigureAwait(false)
                ? Bottom == null
                    ? throw new BottomException()
                    : Bottom()
                : await IsLeft.ConfigureAwait(false)
                    ? Left == null
                        ? throw new ArgumentNullException(nameof(Left))
                        : Left((await Data.ConfigureAwait(false)).Left)
                    : RightAsync == null
                        ? throw new ArgumentNullException(nameof(RightAsync))
                        : await RightAsync((await Data.ConfigureAwait(false)).Right);

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="RightAsync">Function to invoke if in a Right state</param>
        /// <param name="LeftAsync">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> MatchUnsafeAsync<Ret>(Func<R, Task<Ret>> RightAsync, Func<L, Task<Ret>> LeftAsync, Func<Ret> Bottom = null) =>
            await IsBottom.ConfigureAwait(false)
                ? Bottom == null
                    ? throw new BottomException()
                    : Bottom()
                : await IsLeft.ConfigureAwait(false)
                    ? LeftAsync == null
                        ? throw new ArgumentNullException(nameof(LeftAsync))
                        : await LeftAsync((await Data.ConfigureAwait(false)).Left)
                    : RightAsync == null
                        ? throw new ArgumentNullException(nameof(RightAsync))
                        : await RightAsync((await Data.ConfigureAwait(false)).Right);

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="LeftAsync">Function to invoke if in a Left state</param>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public async Task<Ret> MatchUnsafeAsync<Ret>(Func<R, Ret> Right, Func<L, Task<Ret>> LeftAsync, Func<Ret> Bottom = null) =>
            await IsBottom.ConfigureAwait(false)
                ? Bottom == null
                    ? throw new BottomException()
                    : Bottom()
                : await IsLeft.ConfigureAwait(false)
                    ? LeftAsync == null
                        ? throw new ArgumentNullException(nameof(LeftAsync))
                        : await LeftAsync((await Data.ConfigureAwait(false)).Left)
                    : Right == null
                        ? throw new ArgumentNullException(nameof(Right))
                        : Right((await Data.ConfigureAwait(false)).Right);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either
        /// </summary>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        public async Task<Unit> Match(Action<R> Right, Action<L> Left, Action Bottom = null)
        {
            if (await IsRight.ConfigureAwait(false))
            {
                Right?.Invoke((await Data.ConfigureAwait(false)).Right);
            }
            else if (await IsLeft.ConfigureAwait(false))
            {
                Left?.Invoke((await Data.ConfigureAwait(false)).Left);
            }
            else if (await IsBottom.ConfigureAwait(false))
            {
                if (Bottom != null)
                {
                    Bottom();
                }
                else
                {
                    throw new BottomException();
                }
            }
            return unit;
        }

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either
        /// </summary>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="LeftAsync">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        public async Task<Unit> MatchAsync(Action<R> Right, Func<L, Task> LeftAsync, Action Bottom = null)
        {
            if (await IsRight.ConfigureAwait(false))
            {
                Right((await Data.ConfigureAwait(false)).Right);
            }
            else if (await IsLeft.ConfigureAwait(false))
            {
                await LeftAsync((await Data.ConfigureAwait(false)).Left).ConfigureAwait(false);
            }
            else if (await IsBottom.ConfigureAwait(false))
            {
                if (Bottom != null)
                {
                    Bottom();
                }
                else
                {
                    throw new BottomException();
                }
            }
            return unit;
        }

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either
        /// </summary>
        /// <param name="RightAsync">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        public async Task<Unit> MatchAsync(Func<R, Task> RightAsync, Action<L> Left, Action Bottom = null)
        {
            if (await IsRight.ConfigureAwait(false))
            {
                await RightAsync((await Data.ConfigureAwait(false)).Right).ConfigureAwait(false);
            }
            else if (await IsLeft)
            {
                Left?.Invoke((await Data).Left);
            }
            else if (await IsBottom.ConfigureAwait(false))
            {
                if (Bottom != null)
                {
                    Bottom();
                }
                else
                {
                    throw new BottomException();
                }
            }
            return unit;
        }

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either
        /// </summary>
        /// <param name="RightAsync">Action to invoke if in a Right state</param>
        /// <param name="LeftAsync">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        /// <exception cref="BottomException">Thrown if matching on an Either in a bottom state</exception>
        public async Task<Unit> MatchAsync(Func<R, Task> RightAsync, Func<L, Task> LeftAsync, Action Bottom = null)
        {
            if (await IsRight.ConfigureAwait(false))
            {
                await RightAsync((await Data.ConfigureAwait(false)).Right).ConfigureAwait(false);
            }
            else if (await IsLeft)
            {
                await LeftAsync((await Data.ConfigureAwait(false)).Left).ConfigureAwait(false);
            }
            else if (await IsBottom.ConfigureAwait(false))
            {
                if (Bottom != null)
                {
                    Bottom();
                }
                else
                {
                    throw new BottomException();
                }
            }
            return unit;
        }

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public Task<R> IfLeft(Func<R> Left) =>
            Match(
                Right: r => r,
                Left: l => Left());

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="LeftAsync">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public Task<R> IfLeftAsync(Func<Task<R>> LeftAsync) =>
            MatchAsync(
                Right: r => r,
                LeftAsync: l => LeftAsync());

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public Task<R> IfLeft(Func<L, R> Left) =>
            Match(
                Right: r => r,
                Left: l => Left(l));

        /// <summary>
        /// Executes the leftMap function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="LeftAsync">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public Task<R> IfLeftAsync(Func<L, Task<R>> LeftAsync) =>
            MatchAsync(
                Right: r => r,
                LeftAsync: l => LeftAsync(l));

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Right">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public Task<R> IfLeft(R Right) =>
            Match(
                Right: r => r,
                Left: l => Right);

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="RightAsync">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        [Pure]
        public Task<R> IfLeftAsync(Task<R> RightAsync) =>
            MatchAsync(
                Right: r => r,
                LeftAsync: l => RightAsync);

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Action to invoke if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public Task<Unit> IfLeft(Action<L> Left) =>
            Match(
                Right: r => { },
                Left: l => Left(l));

        /// <summary>
        /// Executes the Left action if the Either is in a Left state.
        /// </summary>
        /// <param name="LeftAsync">async Action to invoke if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public Task<Unit> IfLeftAsync(Func<L, Task> LeftAsync) =>
            MatchAsync(
                Right: r => unit,
                LeftAsync: async l => { await LeftAsync(l).ConfigureAwait(false); return unit; });

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="Right">Action to invoke if in the Right state</param>
        /// <returns>Unit</returns>
        public Task<Unit> IfRight(Action<R> Right) =>
            Match(
                Right: r => Right(r),
                Left: l => { });

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// </summary>
        /// <param name="RightAsync">async Action to invoke if in the Right state</param>
        /// <returns>Unit</returns>
        public Task<Unit> IfRightAsync(Func<R, Task> RightAsync) =>
            MatchAsync(
                RightAsync: async r => { await RightAsync(r).ConfigureAwait(false); return unit; },
                Left: l => unit);

        /// <summary>
        /// Returns the leftValue if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public Task<L> IfRight(L Left) =>
            Match(
                Right: r => Left,
                Left: l => l);

        /// <summary>
        /// Returns the result of Right() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public Task<L> IfRight(Func<L> Right) =>
            Match(
                Right: r => Right(),
                Left: l => l);

        /// <summary>
        /// Returns the result of Right() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="RightAsync">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public Task<L> IfRightAsync(Func<Task<L>> RightAsync) =>
            MatchAsync(
                RightAsync: r => RightAsync(),
                Left: l => l);

        /// <summary>
        /// Returns the result of rightMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public Task<L> IfRight(Func<R, L> Right) =>
            Match(
                Right: r => Right(r),
                Left: l => l);

        /// <summary>
        /// Returns the result of rightMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="RightAsync">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public Task<L> IfRightAsync(Func<R, Task<L>> RightAsync) =>
            MatchAsync(
                RightAsync: r => RightAsync(r),
                Left: l => l);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherAsyncUnitContext<L, R> Right(Action<R> right) =>
            new EitherAsyncUnitContext<L, R>(this, right);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        [Pure]
        public EitherAsyncContext<L, R, Ret> Right<Ret>(Func<R, Ret> right) =>
            new EitherAsyncContext<L, R, Ret>(this, right);

        /// <summary>
        /// Return a string representation of the Either
        /// </summary>
        /// <returns>String representation of the Either</returns>
        [Pure]
        public override string ToString() =>
            "EitherAsync";

        /// <summary>
        /// Return a string representation of the Either
        /// </summary>
        /// <returns>String representation of the Either</returns>
        [Pure]
        public Task<string> ToStringAsync() =>
            Match(
                Right:  r  => isnull(r) ? "Right(null)" : $"Right({r})",
                Left:   l  => isnull(l) ? "Left(null)" : $"Right({l})",
                Bottom: () => "Bottom");

        /// <summary>
        /// Returns a hash code of the wrapped value of the Either
        /// </summary>
        /// <returns>Hash code</returns>
        [Pure]
        public override int GetHashCode() =>
            throw new NotSupportedException("Call GetHashCodeAsync instead");

        /// <summary>
        /// Returns a hash code of the wrapped value of the Either
        /// </summary>
        /// <returns>Hash code</returns>
        [Pure]
        public Task<int> GetHashCodeAsync() =>
            Match(
                Right: r => r?.GetHashCode() ?? 0,
                Left: l => l?.GetHashCode() ?? 0,
                Bottom: () => 0);

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public Task<Lst<R>> RightToList() =>
            Match(
                Right: r => List(r),
                Left: l => Lst<R>.Empty,
                Bottom: () => Lst<R>.Empty);

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public Task<Arr<R>> RightToArray() =>
            Match(
                Right: r => Array(r),
                Left: l => Arr<R>.Empty,
                Bottom: () => Arr<R>.Empty);

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public Task<Lst<L>> LeftToList() =>
            Match(
                Right: r => Lst<L>.Empty,
                Left: l => List(l),
                Bottom: () => Lst<L>.Empty);

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public Task<Arr<L>> LeftToArray() =>
            Match(
                Right: r => Arr<L>.Empty,
                Left: l => Array(l),
                Bottom: () => Arr<L>.Empty);

        /// <summary>
        /// Convert either to sequence of 0 or 1 right values
        /// </summary>
        [Pure]
        public Task<Seq<R>> ToSeq() =>
            RightAsEnumerable();

        /// <summary>
        /// Convert either to sequence of 0 or 1 right values
        /// </summary>
        [Pure]
        public Task<Seq<R>> RightToSeq() =>
            RightAsEnumerable();

        /// <summary>
        /// Convert either to sequence of 0 or 1 right values
        /// </summary>
        [Pure]
        public Task<Seq<L>> LeftToSeq() =>
            LeftAsEnumerable();

        /// <summary>
        /// Project the Either into a IEnumerable R
        /// </summary>
        /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
        [Pure]
        public Task<Seq<R>> RightAsEnumerable() =>
            Match(
                Right: Seq1,
                Left: l => Seq<R>.Empty,
                Bottom: () => Seq<R>.Empty);

        /// <summary>
        /// Project the Either into a IEnumerable L
        /// </summary>
        /// <returns>If the Either is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
        [Pure]
        public Task<Seq<L>> LeftAsEnumerable() =>
            Match(
                Right: r => Seq<L>.Empty,
                Left: Seq1,
                Bottom: () => Seq<L>.Empty);

        [Pure]
        public Task<Validation<L, R>> ToValidation() =>
            Match(
                Right: Success<L, R>,
                Left: Fail<L, R>,
                Bottom: () => Validation<L, R>.Fail(Seq<L>.Empty));

        /// <summary>
        /// Convert the Either to an Option
        /// </summary>
        /// <returns>Some(Right) or None</returns>
        [Pure]
        public OptionAsync<R> ToOption() =>
            Match(
                Right: r => Option<R>.Some(r),
                Left: l => Option<R>.None,
                Bottom: () => Option<R>.None).ToAsync();

        /// <summary>
        /// Convert to an Aff
        /// </summary>
        /// <param name="Left">Map the left value to the Eff Error</param>
        /// <returns>Aff monad</returns>
        [Pure]
        public Aff<R> ToAff(Func<L, Common.Error> Left)
        {
            var self = this;
            return AffMaybe<R>(Go);

            ValueTask<Fin<R>> Go() =>
                self.Match(
                    Right: Fin<R>.Succ,
                    Left: l => Fin<R>.Fail(Left(l)),
                    Bottom: () => default ).ToValue();
        }
        
        /// <summary>
        /// Convert the Either to an EitherUnsafe
        /// </summary>
        /// <returns>EitherUnsafe</returns>
        [Pure]
        public Task<EitherUnsafe<L, R>> ToEitherUnsafe() =>
            Match(
                Right: r => EitherUnsafe<L, R>.Right(r),
                Left: l => EitherUnsafe<L, R>.Left(l),
                Bottom: () => EitherUnsafe<L, R>.Bottom);

        /// <summary>
        /// Convert the EitherAsync to an Either
        /// </summary>
        /// <returns>Either</returns>
        [Pure]
        public Task<Either<L, R>> ToEither() =>
            Match(
                Right: r => Either<L, R>.Right(r),
                Left: l => Either<L, R>.Left(l),
                Bottom: () => Either<L, R>.Bottom);

        /// <summary>
        /// Convert the Either to an TryOption
        /// </summary>
        /// <returns>Some(Right) or None</returns>
        [Pure]
        public TryOptionAsync<R> ToTryOption() =>
            Match(
                Right: r => TryOption(Option<R>.Some(r)),
                Left: l => TryOption(Option<R>.None),
                Bottom: () => TryOption(Option<R>.None)).ToAsync();

        /// <summary>
        /// Override of the Or operator to be a Left coalescing operator
        /// </summary>
        [Pure]
        public static EitherAsync<L, R> operator |(EitherAsync<L, R> lhs, EitherAsync<L, R> rhs) =>
            default(MEitherAsync<L, R>).Plus(lhs, rhs);

        /// <summary>
        /// Find out the underlying Right type
        /// </summary>
        [Pure]
        public Type GetUnderlyingRightType() =>
            typeof(R);

        /// <summary>
        /// Find out the underlying Left type
        /// </summary>
        [Pure]
        public Type GetUnderlyingLeftType() =>
            typeof(L);

        [Pure]
        public static EitherAsync<L, R> Right(R value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : new EitherAsync<L, R>(new EitherData<L, R>(EitherStatus.IsRight, value, default(L)).AsTask());

        [Pure]
        public static EitherAsync<L, R> RightAsync(Task<R> value) =>
            isnull(value)
                ? throw new ArgumentNullException(nameof(value))
                : new EitherAsync<L, R>(value.Map(r => new EitherData<L, R>(EitherStatus.IsRight, r, default(L))));

        [Pure]
        public static EitherAsync<L, R> Left(L value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : new EitherAsync<L, R>(new EitherData<L, R>(EitherStatus.IsLeft, default(R), value).AsTask());

        [Pure]
        public static EitherAsync<L, R> LeftAsync(Task<L> value) =>
            isnull(value)
                ? throw new ArgumentNullException(nameof(value))
                : new EitherAsync<L, R>(value.Map(l => new EitherData<L, R>(EitherStatus.IsLeft, default(R), l)));

        [Pure]
        internal async Task<R> RightValue() =>
            Check.NullReturn(
                (await IsRight.ConfigureAwait(false))
                    ? (await Data.ConfigureAwait(false)).Right
                    : raise<R>(new EitherIsNotRightException()));

        [Pure]
        internal async Task<L> LeftValue() =>
            Check.NullReturn(
                (await IsLeft.ConfigureAwait(false))
                    ? (await Data.ConfigureAwait(false)).Left
                    : raise<L>(new EitherIsNotLeftException()));

        [Pure]
        public Type GetUnderlyingType() => 
            typeof(R);

        /// <summary>
        /// Counts the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to count</param>
        /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
        [Pure]
        public Task<int> Count() =>
            Match(
                Right: _ => 1,
                Left: _ => 0,
                Bottom: () => 0);

        /// <summary>
        /// Flips the left and right tagged values
        /// </summary>
        /// <returns>Either with the types swapped</returns>
        [Pure]
        public EitherAsync<R, L> Swap()
        {
            return new EitherAsync<R, L>(Go(Data));
            
            async Task<EitherData<R, L>> Go(Task<EitherData<L, R>> self)
            {
                var data = await self.ConfigureAwait(false);

                return data.State switch
                {
                    EitherStatus.IsRight => EitherData.Left<R, L>(data.Right),
                    EitherStatus.IsLeft  => EitherData.Right<R, L>(data.Left),
                    _                    => EitherData<R, L>.Bottom
                };
            }
        }


        /// <summary>
        /// Iterate the Either
        /// action is invoked if in the Right state
        /// </summary>
        public Task<Unit> Iter(Action<R> Right) =>
            IfRight(Right);

        /// <summary>
        /// Iterate the Either
        /// action is invoked if in the Right state
        /// </summary>
        public Task<Unit> BiIter(Action<R> Right, Action<L> Left) =>
            Match(
                Right: Right,
                Left: Left,
                Bottom: () => { });

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="Right">Predicate</param>
        /// <returns>True if the Either is in a Left state.  
        /// True if the Either is in a Right state and the predicate returns True.  
        /// False otherwise.</returns>
        [Pure]
        public Task<bool> ForAll(Func<R, bool> Right) =>
            Match(
                Right: r => Right(r),
                Left: l => true,
                Bottom: () => true);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="Right">Predicate</param>
        /// <returns>True if the Either is in a Left state.  
        /// True if the Either is in a Right state and the predicate returns True.  
        /// False otherwise.</returns>
        [Pure]
        public Task<bool> ForAllAsync(Func<R, Task<bool>> Right) =>
            MatchAsync(
                RightAsync: r => Right(r),
                Left: l => true,
                Bottom: () => true);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="Right">Predicate</param>
        /// <param name="Left">Predicate</param>
        /// <returns>True if either Predicate returns true</returns>
        [Pure]
        public Task<bool> BiForAll(Func<R, bool> Right, Func<L, bool> Left) =>
            Match(
                Right: r => Right(r),
                Left: l => Left(l),
                Bottom: () => true);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="RightAsync">Predicate</param>
        /// <param name="Left">Predicate</param>
        /// <returns>True if either Predicate returns true</returns>
        [Pure]
        public Task<bool> BiForAllAsync(Func<R, Task<bool>> RightAsync, Func<L, bool> Left) =>
            MatchAsync(
                RightAsync: r => RightAsync(r),
                Left: l => Left(l),
                Bottom: () => true);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="Right">Predicate</param>
        /// <param name="LeftAsync">Predicate</param>
        /// <returns>True if either Predicate returns true</returns>
        [Pure]
        public Task<bool> BiForAllAsync(Func<R, bool> Right, Func<L, Task<bool>> LeftAsync) =>
            MatchAsync(
                Right: r => Right(r),
                LeftAsync: l => LeftAsync(l),
                Bottom: () => true);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to forall</param>
        /// <param name="RightAsync">Predicate</param>
        /// <param name="LeftAsync">Predicate</param>
        /// <returns>True if either Predicate returns true</returns>
        [Pure]
        public Task<bool> BiForAllAsync(Func<R, Task<bool>> RightAsync, Func<L, Task<bool>> LeftAsync) =>
            MatchAsync(
                RightAsync: r => RightAsync(r),
                LeftAsync: l => LeftAsync(l),
                Bottom: () => true);

        /// <summary>
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Right">Folder function, applied if structure is in a Right state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> Fold<S>(S state, Func<S, R, S> Right) =>
            Match(
                Right: r => Right(state, r),
                Left: _ => state,
                Bottom: () => state);

        /// <summary>
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="RightAsync">Folder function, applied if structure is in a Right state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> FoldAsync<S>(S state, Func<S, R, Task<S>> RightAsync) =>
            MatchAsync(
                RightAsync: r => RightAsync(state, r),
                Left: _ => state,
                Bottom: () => state);

        /// <summary>
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Right">Folder function, applied if Either is in a Right state</param>
        /// <param name="Left">Folder function, applied if Either is in a Left state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFold<S>(S state, Func<S, R, S> Right, Func<S, L, S> Left) =>
            Match(
                Right: r => Right(state, r),
                Left: l => Left(state, l),
                Bottom: () => state);

        /// <summary>
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="RightAsync">Folder function, applied if Either is in a Right state</param>
        /// <param name="Left">Folder function, applied if Either is in a Left state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, R, Task<S>> RightAsync, Func<S, L, S> Left) =>
            MatchAsync(
                RightAsync: r => RightAsync(state, r),
                Left: l => Left(state, l),
                Bottom: () => state);

        /// <summary>
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="RightAsync">Folder function, applied if Either is in a Right state</param>
        /// <param name="LeftAsync">Folder function, applied if Either is in a Left state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, R, Task<S>> RightAsync, Func<S, L, Task<S>> LeftAsync) =>
            MatchAsync(
                RightAsync: r => RightAsync(state, r),
                LeftAsync: l => LeftAsync(state, l),
                Bottom: () => state);

        /// <summary>
        /// <para>
        /// Either types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Right">Folder function, applied if Either is in a Right state</param>
        /// <param name="LeftAsync">Folder function, applied if Either is in a Left state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public Task<S> BiFoldAsync<S>(S state, Func<S, R, S> Right, Func<S, L, Task<S>> LeftAsync) =>
            MatchAsync(
                Right: r => Right(state, r),
                LeftAsync: l => LeftAsync(state, l),
                Bottom: () => state);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to check existence of</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
        [Pure]
        public Task<bool> Exists(Func<R, bool> pred) =>
            Match(
                Right: r => pred(r),
                Left: _ => false,
                Bottom: () => false);

        /// <summary>
        /// Invokes a predicate on the value of the Either if it's in the Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to check existence of</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
        [Pure]
        public Task<bool> ExistsAsync(Func<R, Task<bool>> pred) =>
            MatchAsync(
                RightAsync: r => pred(r),
                Left: _ => false,
                Bottom: () => false);

        /// <summary>
        /// Invokes a predicate on the value of the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to check existence of</param>
        /// <param name="Right">Right predicate</param>
        /// <param name="Left">Left predicate</param>
        /// <returns>True if the predicate returns True.  False otherwise or if the Either is in a bottom state.</returns>
        [Pure]
        public Task<bool> BiExists(Func<R, bool> Right, Func<L, bool> Left) =>
            Match(
                Right: r => Right(r),
                Left: l => Left(l),
                Bottom: () => false);

        /// <summary>
        /// Invokes a predicate on the value of the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to check existence of</param>
        /// <param name="RightAsync">Right predicate</param>
        /// <param name="Left">Left predicate</param>
        /// <returns>True if the predicate returns True.  False otherwise or if the Either is in a bottom state.</returns>
        [Pure]
        public Task<bool> BiExistsAsync(Func<R, Task<bool>> RightAsync, Func<L, bool> Left) =>
            MatchAsync(
                RightAsync: r => RightAsync(r),
                Left: l => Left(l),
                Bottom: () => false);

        /// <summary>
        /// Invokes a predicate on the value of the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to check existence of</param>
        /// <param name="RightAsync">Right predicate</param>
        /// <param name="LeftAsync">Left predicate</param>
        /// <returns>True if the predicate returns True.  False otherwise or if the Either is in a bottom state.</returns>
        [Pure]
        public Task<bool> BiExistsAsync(Func<R, Task<bool>> RightAsync, Func<L, Task<bool>> LeftAsync) =>
            MatchAsync(
                RightAsync: r => RightAsync(r),
                LeftAsync: l => LeftAsync(l),
                Bottom: () => false);

        /// <summary>
        /// Invokes a predicate on the value of the Either
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to check existence of</param>
        /// <param name="Right">Right predicate</param>
        /// <param name="LeftAsync">Left predicate</param>
        /// <returns>True if the predicate returns True.  False otherwise or if the Either is in a bottom state.</returns>
        [Pure]
        public Task<bool> BiExistsAsync(Func<R, bool> Right, Func<L, Task<bool>> LeftAsync) =>
            MatchAsync(
                Right: r => Right(r),
                LeftAsync: l => LeftAsync(l),
                Bottom: () => false);

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public EitherAsync<L, R> Do(Action<R> f) =>
            Map(x => { f(x); return x; });

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="f">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L, Ret> Map<Ret>(Func<R, Ret> f) =>
            BiMap(f, identity);

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="mapper">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L, Ret> MapAsync<Ret>(Func<R, Task<Ret>> f) =>
            BiMapAsync(f, x => x);

        /// <summary>
        /// Maps the value in the Either if it's in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="f">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<Ret, R> MapLeft<Ret>(Func<L, Ret> f) =>
            BiMap(identity, f);

        /// <summary>
        /// Maps the value in the Either if it's in a Left state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret">Mapped Either type</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="f">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<Ret, R> MapLeftAsync<Ret>(Func<L, Task<Ret>> f) =>
            BiMapAsync(x => x, f);

        /// <summary>
        /// Bi-maps the value in the Either into a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="Right">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L, Ret> BiMap<Ret>(Func<R, Ret> Right, Func<L, Ret> Left)
        {
            async Task<Ret> Do(EitherAsync<L, R> self, Func<R, Ret> right, Func<L, Ret> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? right(await self.RightValue().ConfigureAwait(false))
                    : left(await self.LeftValue().ConfigureAwait(false));

            return EitherAsync<L, Ret>.RightAsync(Do(this, Right, Left));
        }

        /// <summary>
        /// Bi-maps the value in the Either into a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="RightAsync">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L, Ret> BiMapAsync<Ret>(Func<R, Task<Ret>> RightAsync, Func<L, Ret> Left)
        {
            async Task<Ret> Do(EitherAsync<L, R> self, Func<R, Task<Ret>> right, Func<L, Ret> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? await right(await self.RightValue().ConfigureAwait(false)).ConfigureAwait(false)
                    : left(await self.LeftValue().ConfigureAwait(false));

            return EitherAsync<L, Ret>.RightAsync(Do(this, RightAsync, Left));
        }

        /// <summary>
        /// Bi-maps the value in the Either into a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="Right">Right map function</param>
        /// <param name="LeftAsync">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L, Ret> BiMapAsync<Ret>(Func<R, Ret> Right, Func<L, Task<Ret>> LeftAsync)
        {
            async Task<Ret> Do(EitherAsync<L, R> self, Func<R, Ret> right, Func<L, Task<Ret>> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? right(await self.RightValue().ConfigureAwait(false))
                    : await left(await self.LeftValue().ConfigureAwait(false));

            return EitherAsync<L, Ret>.RightAsync(Do(this, Right, LeftAsync));
        }

        /// <summary>
        /// Bi-maps the value in the Either into a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="RightAsync">Right map function</param>
        /// <param name="LeftAsync">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L, Ret> BiMapAsync<Ret>(Func<R, Task<Ret>> RightAsync, Func<L, Task<Ret>> LeftAsync)
        {
            async Task<Ret> Do(EitherAsync<L, R> self, Func<R, Task<Ret>> right, Func<L, Task<Ret>> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? await right(await self.RightValue().ConfigureAwait(false)).ConfigureAwait(false)
                    : await left(await self.LeftValue().ConfigureAwait(false)).ConfigureAwait(false);

            return EitherAsync<L, Ret>.RightAsync(Do(this, RightAsync, LeftAsync));
        }

        /// <summary>
        /// Bi-maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="Right">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L2, R2> BiMap<L2, R2>(Func<R, R2> Right, Func<L, L2> Left)
        {
            async Task<EitherData<L2, R2>> Do(EitherAsync<L, R> self, Func<R, R2> right, Func<L, L2> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? new EitherData<L2, R2>(EitherStatus.IsRight, right(await self.RightValue().ConfigureAwait(false)), default(L2))
                    : (await self.IsLeft.ConfigureAwait(false))
                        ? new EitherData<L2, R2>(EitherStatus.IsLeft, default(R2), left(await self.LeftValue().ConfigureAwait(false)))
                        : EitherData<L2, R2>.Bottom;

            return new EitherAsync<L2, R2>(Do(this, Right, Left));
        }

        /// <summary>
        /// Bi-maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="RightAsync">Right map function</param>
        /// <param name="Left">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L2, R2> BiMapAsync<L2, R2>(Func<R, Task<R2>> RightAsync, Func<L, L2> Left)
        {
            async Task<EitherData<L2, R2>> Do(EitherAsync<L, R> self, Func<R, Task<R2>> right, Func<L, L2> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? new EitherData<L2, R2>(EitherStatus.IsRight, await right(await self.RightValue().ConfigureAwait(false)).ConfigureAwait(false), default(L2))
                    : (await self.IsLeft.ConfigureAwait(false))
                        ? new EitherData<L2, R2>(EitherStatus.IsLeft, default(R2), left(await self.LeftValue().ConfigureAwait(false)))
                        : EitherData<L2, R2>.Bottom;

            return new EitherAsync<L2, R2>(Do(this, RightAsync, Left));
        }

        /// <summary>
        /// Bi-maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="Right">Right map function</param>
        /// <param name="LeftAsync">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L2, R2> BiMapAsync<L2, R2>(Func<R, R2> Right, Func<L, Task<L2>> LeftAsync)
        {
            async Task<EitherData<L2, R2>> Do(EitherAsync<L, R> self, Func<R, R2> right, Func<L, Task<L2>> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? new EitherData<L2, R2>(EitherStatus.IsRight, right(await self.RightValue().ConfigureAwait(false)), default(L2))
                    : (await self.IsLeft.ConfigureAwait(false))
                        ? new EitherData<L2, R2>(EitherStatus.IsLeft, default(R2), await left(await self.LeftValue().ConfigureAwait(false)).ConfigureAwait(false))
                        : EitherData<L2, R2>.Bottom;

            return new EitherAsync<L2, R2>(Do(this, Right, LeftAsync));
        }

        /// <summary>
        /// Bi-maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="LRet">Left return</typeparam>
        /// <typeparam name="RRet">Right return</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="RightAsync">Right map function</param>
        /// <param name="LeftAsync">Left map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L2, R2> BiMapAsync<L2, R2>(Func<R, Task<R2>> RightAsync, Func<L, Task<L2>> LeftAsync)
        {
            async Task<EitherData<L2, R2>> Do(EitherAsync<L, R> self, Func<R, Task<R2>> right, Func<L, Task<L2>> left) =>
                (await self.IsRight.ConfigureAwait(false))
                    ? new EitherData<L2, R2>(EitherStatus.IsRight, await right(await self.RightValue().ConfigureAwait(false)).ConfigureAwait(false), default(L2))
                    : (await self.IsLeft.ConfigureAwait(false))
                        ? new EitherData<L2, R2>(EitherStatus.IsLeft, default(R2), await left(await self.LeftValue()).ConfigureAwait(false))
                        : EitherData<L2, R2>.Bottom;

            return new EitherAsync<L2, R2>(Do(this, RightAsync, LeftAsync));
        }

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret"></typeparam>
        /// <param name="self"></param>
        /// <param name="f"></param>
        /// <returns>Bound Either</returns>
        [Pure]
        public EitherAsync<L, Ret> Bind<Ret>(Func<R, EitherAsync<L, Ret>> f)
        {
            async Task<EitherData<L, Ret>> Do(EitherAsync<L, R> self, Func<R, EitherAsync<L, Ret>> ff)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    var mb = ff(await self.RightValue().ConfigureAwait(false));
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return (await self.IsLeft.ConfigureAwait(false))
                        ? new EitherData<L, Ret>(EitherStatus.IsLeft, default(Ret), await self.LeftValue().ConfigureAwait(false))
                        : EitherData<L, Ret>.Bottom;
                }
            }

            return new EitherAsync<L, Ret>(Do(this, f));
        }

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <typeparam name="Ret"></typeparam>
        /// <param name="self"></param>
        /// <param name="f"></param>
        /// <returns>Bound Either</returns>
        [Pure]
        public EitherAsync<L, Ret> BindAsync<Ret>(Func<R, Task<EitherAsync<L, Ret>>> f)
        {
            async Task<EitherData<L, Ret>> Do(EitherAsync<L, R> self, Func<R, Task<EitherAsync<L, Ret>>> ff)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    var mb = await ff(await self.RightValue()).ConfigureAwait(false);
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return (await self.IsLeft.ConfigureAwait(false))
                        ? new EitherData<L, Ret>(EitherStatus.IsLeft, default(Ret), await self.LeftValue().ConfigureAwait(false))
                        : EitherData<L, Ret>.Bottom;
                }
            }

            return new EitherAsync<L, Ret>(Do(this, f));
        }

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public EitherAsync<L, B> BiBind<B>(Func<R, EitherAsync<L, B>> Right, Func<L, EitherAsync<L, B>> Left)
        {
            async Task<EitherData<L, B>> Do(EitherAsync<L, R> self, Func<R, EitherAsync<L, B>> right, Func<L, EitherAsync<L, B>> left)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    var mb = right(await self.RightValue().ConfigureAwait(false));
                    return await mb.Data.ConfigureAwait(false);
                }
                if (await self.IsLeft.ConfigureAwait(false))
                {
                    var mb = left(await self.LeftValue().ConfigureAwait(false));
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return EitherData<L, B>.Bottom;
                }
            }
            return new EitherAsync<L, B>(Do(this, Right, Left));
        }

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public EitherAsync<L, B> BiBindAsync<B>(Func<R, Task<EitherAsync<L, B>>> RightAsync, Func<L, EitherAsync<L, B>> Left)
        {
            async Task<EitherData<L, B>> Do(EitherAsync<L, R> self, Func<R, Task<EitherAsync<L, B>>> right, Func<L, EitherAsync<L, B>> left)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    var mb = await right(await self.RightValue()).ConfigureAwait(false);
                    return await mb.Data.ConfigureAwait(false);
                }
                if (await self.IsLeft.ConfigureAwait(false))
                {
                    var mb = left(await self.LeftValue().ConfigureAwait(false));
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return EitherData<L, B>.Bottom;
                }
            }
            return new EitherAsync<L, B>(Do(this, RightAsync, Left));
        }

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public EitherAsync<L, B> BiBindAsync<B>(Func<R, EitherAsync<L, B>> Right, Func<L, Task<EitherAsync<L, B>>> LeftAsync)
        {
            async Task<EitherData<L, B>> Do(EitherAsync<L, R> self, Func<R, EitherAsync<L, B>> right, Func<L, Task<EitherAsync<L, B>>> left)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    var mb = right(await self.RightValue().ConfigureAwait(false));
                    return await mb.Data.ConfigureAwait(false);
                }
                if (await self.IsLeft.ConfigureAwait(false))
                {
                    var mb = await left(await self.LeftValue()).ConfigureAwait(false);
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return EitherData<L, B>.Bottom;
                }
            }
            return new EitherAsync<L, B>(Do(this, Right, LeftAsync));
        }

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public EitherAsync<L, B> BiBindAsync<B>(Func<R, Task<EitherAsync<L, B>>> RightAsync, Func<L, Task<EitherAsync<L, B>>> LeftAsync)
        {
            async Task<EitherData<L, B>> Do(EitherAsync<L, R> self, Func<R, Task<EitherAsync<L, B>>> right, Func<L, Task<EitherAsync<L, B>>> left)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    var mb = await right(await self.RightValue().ConfigureAwait(false)).ConfigureAwait(false);
                    return await mb.Data.ConfigureAwait(false);
                }
                if (await self.IsLeft.ConfigureAwait(false))
                {
                    var mb = await left(await self.LeftValue()).ConfigureAwait(false);
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return EitherData<L, B>.Bottom;
                }
            }
            return new EitherAsync<L, B>(Do(this, RightAsync, LeftAsync));
        }

        /// <summary>
        /// Bind left.  Binds the left path of the monad only
        /// </summary>
        public EitherAsync<B, R> BindLeft<B>(Func<L, EitherAsync<B, R>> Left)
        {
            async Task<EitherData<B, R>> Do(EitherAsync<L, R> self)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    return new EitherData<B, R>(EitherStatus.IsRight, await self.RightValue().ConfigureAwait(false), default(B));
                }
                if (await self.IsLeft.ConfigureAwait(false))
                {
                    var mb = Left(await self.LeftValue().ConfigureAwait(false));
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return EitherData<B, R>.Bottom;
                }
            }
            return new EitherAsync<B, R>(Do(this));
        }

        /// <summary>
        /// Bind left.  Binds the left path of the monad only
        /// </summary>
        public EitherAsync<B, R> BindLeftAsync<B>(Func<L, Task<EitherAsync<B, R>>> LeftAsync)
        {
            async Task<EitherData<B, R>> Do(EitherAsync<L, R> self)
            {
                if (await self.IsRight.ConfigureAwait(false))
                {
                    return new EitherData<B, R>(EitherStatus.IsRight, await self.RightValue().ConfigureAwait(false), default(B));
                }
                if (await self.IsLeft.ConfigureAwait(false))
                {
                    var mb = await LeftAsync(await self.LeftValue().ConfigureAwait(false)).ConfigureAwait(false);
                    return await mb.Data.ConfigureAwait(false);
                }
                else
                {
                    return EitherData<B, R>.Bottom;
                }
            }
            return new EitherAsync<B, R>(Do(this));
        }

        /// <summary>
        /// Filter the Either
        /// </summary>
        /// <remarks>
        /// This may give unpredictable results for a filtered value.  The Either won't
        /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
        /// should be checked for.
        /// </remarks>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to filter</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>If the Either is in the Left state it is returned as-is.  
        /// If in the Right state the predicate is applied to the Right value.
        /// If the predicate returns True the Either is returned as-is.
        /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
        [Pure]
        public EitherAsync<L, R> Filter(Func<R, bool> pred)
        {
            var self = this;
            return Bind(r => pred(r) ? self : Bottom);
        }

        /// <summary>
        /// Filter the Either
        /// </summary>
        /// <remarks>
        /// This may give unpredictable results for a filtered value.  The Either won't
        /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
        /// should be checked for.
        /// </remarks>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to filter</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>If the Either is in the Left state it is returned as-is.  
        /// If in the Right state the predicate is applied to the Right value.
        /// If the predicate returns True the Either is returned as-is.
        /// If the predicate returns False the Either is returned in a 'Bottom' state.</returns>
        [Pure]
        public EitherAsync<L, R> FilterAsync(Func<R, Task<bool>> pred)
        {
            var self = this;
            return BindAsync(async r => (await pred(r).ConfigureAwait(false)) ? self : Bottom);
        }

        /// <summary>
        /// Filter the Either
        /// </summary>
        /// <remarks>
        /// This may give unpredictable results for a filtered value.  The Either won't
        /// return true for IsLeft or IsRight.  IsBottom is True if the value is filtered and that
        /// should be checked for.
        /// </remarks>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="self">Either to filter</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>If the Either is in the Left state it is returned as-is.  
        /// If in the Right state the predicate is applied to the Right value.
        /// If the predicate returns True the Either is returned as-is.
        /// If the predicate returns False the Either is returned in a 'Bottom' state.  IsLeft will return True, but the value 
        /// of Left = default(L)</returns>
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EitherAsync<L, R> Where(Func<R, bool> pred)
        {
            var self = this;
            return Bind(r => pred(r) ? self : Bottom);
        }

        /// <summary>
        /// Maps the value in the Either if it's in a Right state
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="TR">Right</typeparam>
        /// <typeparam name="UR">Mapped Either type</typeparam>
        /// <param name="self">Either to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped Either</returns>
        [Pure]
        public EitherAsync<L, U> Select<U>(Func<R, U> map) =>
            Bind(x => EitherAsync<L, U>.Right(map(x)));

        /// <summary>
        /// Monadic bind function
        /// </summary>
        /// <returns>Bound Either</returns>
        [Pure]
        public EitherAsync<L, V> SelectMany<U, V>(Func<R, EitherAsync<L, U>> bind, Func<R, U, V> project) =>
            Bind(a => bind(a).Bind(b => EitherAsync<L, V>.Right(project(a, b))));


        /// <summary>
        /// Enumerate asynchronously
        /// </summary>
        [Pure]
        public async IAsyncEnumerator<R> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var data = await Data.ConfigureAwait(false);
            if (data.State == EitherStatus.IsRight)
            {
                yield return data.Right;
            }
        }
    }

    /// <summary>
    /// Context for the fluent EitherAsync matching
    /// </summary>
    public struct EitherAsyncContext<L, R, Ret>
    {
        readonly EitherAsync<L, R> either;
        readonly Func<R, Ret> rightHandler;

        internal EitherAsyncContext(EitherAsync<L, R> either, Func<R, Ret> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        /// <summary>
        /// Left match
        /// </summary>
        /// <param name="left"></param>
        /// <returns>Result of the match</returns>
        [Pure]
        public Task<Ret> Left(Func<L, Ret> left) =>
            either.Match(rightHandler, left);
    }

    /// <summary>
    /// Context for the fluent EitherAsync matching
    /// </summary>
    public struct EitherAsyncUnitContext<L, R>
    {
        readonly EitherAsync<L, R> either;
        readonly Action<R> rightHandler;

        internal EitherAsyncUnitContext(EitherAsync<L, R> either, Action<R> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        public Task<Unit> Left(Action<L> leftHandler) =>
            either.Match(rightHandler, leftHandler);
    }
}
