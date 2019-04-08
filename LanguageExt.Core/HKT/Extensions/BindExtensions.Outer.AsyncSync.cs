using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class BindExtensions
    {
        // Async : Arr

        public static OptionAsync<Arr<B>> BindT<A, B>(this OptionAsync<Arr<A>> ma, Func<A, OptionAsync<Arr<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb) = await f(ia).Map(b => (succ: true, res: b)).IfNone(() => (false, Arr<B>.Empty));
                        if (!success) return OptionAsync<Arr<B>>.None;
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return results.ToArr();
                });

        public static Task<Arr<B>> BindT<A, B>(this Task<Arr<A>> ma, Func<A, Task<Arr<B>>> f) =>
            ma.MapAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var imb = await f(ia);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return results.ToArr();
                });

        public static EitherAsync<L, Arr<B>> BindT<L, A , B>(this EitherAsync<L, Arr<A>> ma, Func<A, EitherAsync<L, Arr<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, left) = await f(ia).Map(b => (succ: true, fail: b, left: default(L))).IfLeft(l => (false, Arr<B>.Empty, l));
                        if (!success) return EitherAsync<L, Arr<B>>.Left(left);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return results.ToArr();
                });

        public static TryAsync<Arr<B>> BindT<A, B>(this TryAsync<Arr<A>> ma, Func<A, TryAsync<Arr<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Succ: b => (true, b, default(Exception)),
                                                                  Fail: ex => (false, Arr<B>.Empty, ex));

                        if (!success) return TryAsync<Arr<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryAsync(results.ToArr());
                });

        public static TryOptionAsync<Arr<B>> BindT<A, B>(this TryOptionAsync<Arr<A>> ma, Func<A, TryOptionAsync<Arr<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Some: b => (true, b, default(Exception)),
                                                                  None: () => (true, Arr<B>.Empty, default(Exception)),
                                                                  Fail: ex => (false, Arr<B>.Empty, ex));

                        if (!success && e == null) return TryOptionAsync(Option<Arr<B>>.None);
                        if (!success && e != null) return TryOptionAsync<Arr<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryOptionAsync(results.ToArr());
                });


        // Async : HashSet

        public static OptionAsync<HashSet<B>> BindT<A, B>(this OptionAsync<HashSet<A>> ma, Func<A, OptionAsync<HashSet<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb) = await f(ia).Map(b => (succ: true, res: b)).IfNone(() => (false, HashSet<B>.Empty));
                        if (!success) return OptionAsync<HashSet<B>>.None;
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new HashSet<B>(results);
                });

        public static Task<HashSet<B>> BindT<A, B>(this Task<HashSet<A>> ma, Func<A, Task<HashSet<B>>> f) =>
            ma.MapAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var imb = await f(ia);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new HashSet<B>(results);
                });

        public static EitherAsync<L, HashSet<B>> BindT<L, A, B>(this EitherAsync<L, HashSet<A>> ma, Func<A, EitherAsync<L, HashSet<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, left) = await f(ia).Map(b => (succ: true, res: b, left:default(L))).IfLeft(l => (false, HashSet<B>.Empty, l));
                        if (!success) return EitherAsync<L, HashSet<B>>.Left(left);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new HashSet<B>(results);
                });

        public static TryAsync<HashSet<B>> BindT<A, B>(this TryAsync<HashSet<A>> ma, Func<A, TryAsync<HashSet<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Succ: b => (true, b, default(Exception)),
                                                                  Fail: ex => (false, HashSet<B>.Empty, ex));

                        if (!success) return TryAsync<HashSet<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryAsync(new HashSet<B>(results));
                });

        public static TryOptionAsync<HashSet<B>> BindT<A, B>(this TryOptionAsync<HashSet<A>> ma, Func<A, TryOptionAsync<HashSet<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Some: b => (true, b, default(Exception)),
                                                                  None: () => (true, HashSet<B>.Empty, default(Exception)),
                                                                  Fail: ex => (false, HashSet<B>.Empty, ex));

                        if (!success && e == null) return TryOptionAsync(Option<HashSet<B>>.None);
                        if (!success && e != null) return TryOptionAsync<HashSet<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryOptionAsync(new HashSet<B>(results));
                });

        // Async : Lst

        public static OptionAsync<Lst<B>> BindT<A, B>(this OptionAsync<Lst<A>> ma, Func<A, OptionAsync<Lst<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb) = await f(ia).Map(b => (succ: true, res: b)).IfNone(() => (false, Lst<B>.Empty));
                        if (!success) return OptionAsync<Lst<B>>.None;
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new Lst<B>(results);
                });

        public static Task<Lst<B>> BindT<A, B>(this Task<Lst<A>> ma, Func<A, Task<Lst<B>>> f) =>
            ma.MapAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var imb = await f(ia);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new Lst<B>(results);
                });

        public static EitherAsync<L, Lst<B>> BindT<L, A, B>(this EitherAsync<L, Lst<A>> ma, Func<A, EitherAsync<L, Lst<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, left) = await f(ia).Map(b => (succ: true, res: b, left: default(L))).IfLeft(l => (false, Lst<B>.Empty, l));
                        if (!success) return EitherAsync<L, Lst<B>>.Left(left);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new Lst<B>(results);
                });

        public static TryAsync<Lst<B>> BindT<A, B>(this TryAsync<Lst<A>> ma, Func<A, TryAsync<Lst<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Succ: b => (true, b, default(Exception)),
                                                                  Fail: ex => (false, Lst<B>.Empty, ex));

                        if (!success) return TryAsync<Lst<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryAsync(new Lst<B>(results));
                });

        public static TryOptionAsync<Lst<B>> BindT<A, B>(this TryOptionAsync<Lst<A>> ma, Func<A, TryOptionAsync<Lst<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Some: b => (true, b, default(Exception)),
                                                                  None: () => (true, Lst<B>.Empty, default(Exception)),
                                                                  Fail: ex => (false, Lst<B>.Empty, ex));

                        if (!success && e == null) return TryOptionAsync(Option<Lst<B>>.None);
                        if (!success && e != null) return TryOptionAsync<Lst<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryOptionAsync(new Lst<B>(results));
                });


        // Async : Option

        public static OptionAsync<Option<B>> BindT<A, B>(this OptionAsync<Option<A>> ma, Func<A, OptionAsync<Option<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => OptionAsync<Option<B>>.Some(Option<B>.None)));

        public static Task<Option<B>> BindT<A, B>(this Task<Option<A>> ma, Func<A, Task<Option<B>>> f) =>
            ma.MapAsync(
                async ima =>
                    await ima.MatchAsync(
                        Some: async x => await f(x),
                        None: () => Option<B>.None));

        public static EitherAsync<L, Option<B>> BindT<L, A, B>(this EitherAsync<L, Option<A>> ma, Func<A, EitherAsync<L, Option<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => EitherAsync<L, Option<B>>.Right(Option<B>.None)));

        public static TryAsync<Option<B>> BindT<A, B>(this TryAsync<Option<A>> ma, Func<A, TryAsync<Option<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => TryAsync(Option<B>.None)));

        public static TryOptionAsync<Option<B>> BindT<A, B>(this TryOptionAsync<Option<A>> ma, Func<A, TryOptionAsync<Option<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => TryOptionAsync<Option<B>>(Option<B>.None)));


        // Async : OptionUnsafe

        public static OptionAsync<OptionUnsafe<B>> BindT<A, B>(this OptionAsync<OptionUnsafe<A>> ma, Func<A, OptionAsync<OptionUnsafe<B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Some: x => f(x),
                     None: () => OptionAsync<OptionUnsafe<B>>.Some(OptionUnsafe<B>.None)));

        public static Task<OptionUnsafe<B>> BindT<A, B>(this Task<OptionUnsafe<A>> ma, Func<A, Task<OptionUnsafe<B>>> f) =>
            ma.MapAsync(
                async ima =>
                    await ima.MatchAsync(
                        Some: async x => await f(x),
                        None: () => OptionUnsafe<B>.None));

        public static EitherAsync<L, OptionUnsafe<B>> BindT<L, A, B>(this EitherAsync<L, OptionUnsafe<A>> ma, Func<A, EitherAsync<L, OptionUnsafe<B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Some: x => f(x),
                     None: () => EitherAsync<L, OptionUnsafe<B>>.Right(OptionUnsafe<B>.None)));

        public static TryAsync<OptionUnsafe<B>> BindT<A, B>(this TryAsync<OptionUnsafe<A>> ma, Func<A, TryAsync<OptionUnsafe<B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Some: x => f(x),
                     None: () => TryAsync(OptionUnsafe<B>.None)));

        public static TryOptionAsync<OptionUnsafe<B>> BindT<A, B>(this TryOptionAsync<OptionUnsafe<A>> ma, Func<A, TryOptionAsync<OptionUnsafe<B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Some: x => f(x),
                     None: () => TryOptionAsync(OptionUnsafe<B>.None)));

        // Async : Either

        public static OptionAsync<Either<L, B>> BindT<L, A, B>(this OptionAsync<Either<L, A>> ma, Func<A, OptionAsync<Either<L, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Right: x => f(x),
                     Left: e => OptionAsync<Either<L, B>>.Some(Either<L, B>.Left(e))));

        public static Task<Either<L, B>> BindT<L, A, B>(this Task<Either<L, A>> ma, Func<A, Task<Either<L, B>>> f) =>
            ma.MapAsync(
                async ima =>
                    await ima.ToAsync().MatchAsync(
                        RightAsync: async x => await f(x),
                        Left: e => Either<L, B>.Left(e)));

        public static EitherAsync<L, Either<L, B>> BindT<L, A, B>(this EitherAsync<L, Either<L, A>> ma, Func<A, EitherAsync<L, Either<L, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Right: x => f(x),
                     Left: e => EitherAsync<L, Either<L, B>>.Right(Either<L, B>.Left(e))));

        public static TryAsync<Either<L, B>> BindT<L, A, B>(this TryAsync<Either<L, A>> ma, Func<A, TryAsync<Either<L, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Right: x => f(x),
                     Left: e => TryAsync(Either<L, B>.Left(e))));

        public static TryOptionAsync<Either<L, B>> BindT<L, A, B>(this TryOptionAsync<Either<L, A>> ma, Func<A, TryOptionAsync<Either<L, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Right: x => f(x),
                     Left: e => TryOptionAsync(Either<L, B>.Left(e))));

        // Async : EitherUnsafe

        public static OptionAsync<EitherUnsafe<L, B>> BindT<L, A, B>(this OptionAsync<EitherUnsafe<L, A>> ma, Func<A, OptionAsync<EitherUnsafe<L, B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Right: x => f(x),
                     Left: e => OptionAsync<EitherUnsafe<L, B>>.Some(EitherUnsafe<L, B>.Left(e))));

        public static Task<EitherUnsafe<L, B>> BindT<L, A, B>(this Task<EitherUnsafe<L, A>> ma, Func<A, Task<EitherUnsafe<L, B>>> f) =>
            ma.MapAsync(
                async ima =>
                    await ima.MatchUnsafe(
                        Right: x => f(x),
                        Left: e => EitherUnsafe<L, B>.Left(e).AsTask()));

        public static EitherAsync<L, EitherUnsafe<L, B>> BindT<L, A, B>(this EitherAsync<L, EitherUnsafe<L, A>> ma, Func<A, EitherAsync<L, EitherUnsafe<L, B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Right: x => f(x),
                     Left: e => EitherAsync<L, EitherUnsafe<L, B>>.Right(EitherUnsafe<L, B>.Left(e))));

        public static TryAsync<EitherUnsafe<L, B>> BindT<L, A, B>(this TryAsync<EitherUnsafe<L, A>> ma, Func<A, TryAsync<EitherUnsafe<L, B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Right: x => f(x),
                     Left: e => TryAsync(EitherUnsafe<L, B>.Left(e))));

        public static TryOptionAsync<EitherUnsafe<L, B>> BindT<L, A, B>(this TryOptionAsync<EitherUnsafe<L, A>> ma, Func<A, TryOptionAsync<EitherUnsafe<L, B>>> f) =>
            ma.Bind(
                ima => ima.MatchUnsafe(
                     Right: x => f(x),
                     Left: e => TryOptionAsync(EitherUnsafe<L, B>.Left(e))));

        // Async : Try

        public static OptionAsync<Try<B>> BindT<A, B>(this OptionAsync<Try<A>> ma, Func<A, OptionAsync<Try<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => OptionAsync<Try<B>>.Some(Try<B>(e))));

        public static Task<Try<B>> BindT<A, B>(this Task<Try<A>> ma, Func<A, Task<Try<B>>> f) =>
            ma.MapAsync(
                async ima =>
                    await ima.MatchAsync(
                        Succ: async x => await f(x),
                        Fail: e => Try<B>(e)));

        public static EitherAsync<L, Try<B>> BindT<L, A, B>(this EitherAsync<L, Try<A>> ma, Func<A, EitherAsync<L, Try<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => EitherAsync<L, Try<B>>.Right(Try<B>(e))));

        public static TryAsync<Try<B>> BindT<A, B>(this TryAsync<Try<A>> ma, Func<A, TryAsync<Try<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => TryAsync(Try<B>(e))));

        public static TryOptionAsync<Try<B>> BindT<A, B>(this TryOptionAsync<Try<A>> ma, Func<A, TryOptionAsync<Try<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => TryOptionAsync<Try<B>>(Try<B>(e))));


        // Async : Try

        public static OptionAsync<TryOption<B>> BindT<A, B>(this OptionAsync<TryOption<A>> ma, Func<A, OptionAsync<TryOption<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => OptionAsync<TryOption<B>>.Some(TryOption(Option<B>.None)),
                     Fail: e => OptionAsync<TryOption<B>>.Some(TryOption<B>(e))));

        public static Task<TryOption<B>> BindT<A, B>(this Task<TryOption<A>> ma, Func<A, Task<TryOption<B>>> f) =>
            ma.MapAsync(
                async ima =>
                    await ima.MatchAsync(
                        Some: async x => await f(x),
                        None: () => TryOption(Option<B>.None),
                        Fail: e => TryOption<B>(e)));

        public static EitherAsync<L, TryOption<B>> BindT<L, A, B>(this EitherAsync<L, TryOption<A>> ma, Func<A, EitherAsync<L, TryOption<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => TryOption(Option<B>.None),
                     Fail: e => EitherAsync<L, TryOption<B>>.Right(TryOption<B>(e))));

        public static TryAsync<TryOption<B>> BindT<A, B>(this TryAsync<TryOption<A>> ma, Func<A, TryAsync<TryOption<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => TryAsync(TryOption(Option<B>.None)),
                     Fail: e => TryAsync(TryOption<B>(e))));

        public static TryOptionAsync<TryOption<B>> BindT<A, B>(this TryOptionAsync<TryOption<A>> ma, Func<A, TryOptionAsync<TryOption<B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Some: x => f(x),
                     None: () => TryOptionAsync(TryOption(Option<B>.None)),
                     Fail: e => TryOptionAsync(TryOption<B>(e))));


        // Async : IEnumerable

        public static OptionAsync<IEnumerable<B>> BindT<A, B>(this OptionAsync<IEnumerable<A>> ma, Func<A, OptionAsync<IEnumerable<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb) = await f(ia).Map(b => (succ: true, res: b)).IfNone(() => (false, Enumerable.Empty<B>()));
                        if (!success) return OptionAsync<IEnumerable<B>>.None;
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return OptionAsync<IEnumerable<B>>.Some(results.AsEnumerable());
                });

        public static Task<IEnumerable<B>> BindT<A, B>(this Task<IEnumerable<A>> ma, Func<A, Task<IEnumerable<B>>> f) =>
            ma.MapAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var imb = await f(ia);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return results.AsEnumerable();
                });

        public static EitherAsync<L, IEnumerable<B>> BindT<L, A, B>(this EitherAsync<L, IEnumerable<A>> ma, Func<A, EitherAsync<L, IEnumerable<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, left) = await f(ia).Map(b => (succ: true, res: b, left: default(L))).IfLeft(l => (false, Enumerable.Empty<B>(), l));
                        if (!success) return EitherAsync<L, IEnumerable<B>>.Left(left);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return EitherAsync<L, IEnumerable<B>>.Right(results.AsEnumerable());
                });

        public static TryAsync<IEnumerable<B>> BindT<A, B>(this TryAsync<IEnumerable<A>> ma, Func<A, TryAsync<IEnumerable<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Succ: b => (true, b, default(Exception)),
                                                                  Fail: ex => (false, Enumerable.Empty<B>(), ex));

                        if (!success) return TryAsync<IEnumerable<B>>(e);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return TryAsync(results.AsEnumerable());
                });

        public static TryOptionAsync<IEnumerable<B>> BindT<A, B>(this TryOptionAsync<IEnumerable<A>> ma, Func<A, TryOptionAsync<IEnumerable<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Some: b => (true, b, default(Exception)),
                                                                  None: () => (true, Enumerable.Empty<B>(), default(Exception)),
                                                                  Fail: ex => (false, Enumerable.Empty<B>(), ex));

                        if (!success && e == null) return TryOptionAsync(Option<IEnumerable<B>>.None);
                        if (!success && e != null) return TryOptionAsync<IEnumerable<B>>(e);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return TryOptionAsync(results.AsEnumerable());
                });


        // Async : Seq

        public static OptionAsync<Seq<B>> BindT<A, B>(this OptionAsync<Seq<A>> ma, Func<A, OptionAsync<Seq<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb) = await f(ia).Map(b => (succ: true, res: b)).IfNone(() => (false, Seq<B>()));
                        if (!success) return OptionAsync<Seq<B>>.None;
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return OptionAsync<Seq<B>>.Some(Seq(results));
                });

        public static Task<Seq<B>> BindT<A, B>(this Task<Seq<A>> ma, Func<A, Task<Seq<B>>> f) =>
            ma.MapAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var imb = await f(ia);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return Seq(results);
                });

        public static EitherAsync<L, Seq<B>> BindT<L, A, B>(this EitherAsync<L, Seq<A>> ma, Func<A, EitherAsync<L, Seq<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, left) = await f(ia).Map(b => (succ: true, res: b, left: default(L))).IfLeft(l => (false, Seq<B>(), l));
                        if (!success) return EitherAsync<L, Seq<B>>.Left(left);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return EitherAsync<L, Seq<B>>.Right(Seq(results));
                });

        public static TryAsync<Seq<B>> BindT<A, B>(this TryAsync<Seq<A>> ma, Func<A, TryAsync<Seq<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Succ: b => (succ: true, res: b, left: default(Exception)),
                                                                  Fail: ex => (succ: false, res: Enumerable.Empty<B>(), left: ex));

                        if (!success) return TryAsync<Seq<B>>(e);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return TryAsync(Seq(results));
                });

        public static TryOptionAsync<Seq<B>> BindT<A, B>(this TryOptionAsync<Seq<A>> ma, Func<A, TryOptionAsync<Seq<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = Enumerable.Empty<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Some: b => (true, b, default(Exception)),
                                                                  None: () => (true, Enumerable.Empty<B>(), default(Exception)),
                                                                  Fail: ex => (false, Enumerable.Empty<B>(), ex));

                        if (!success && e == null) return TryOptionAsync(Option<Seq<B>>.None);
                        if (!success && e != null) return TryOptionAsync<Seq<B>>(e);
                        results = EnumerableOptimal.ConcatFast(results, imb);
                    }
                    return TryOptionAsync(Seq(results));
                });

        // Async : Set

        public static OptionAsync<Set<B>> BindT<A, B>(this OptionAsync<Set<A>> ma, Func<A, OptionAsync<Set<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb) = await f(ia).Map(b => (succ: true, res: b)).IfNone(() => (false, Set<B>.Empty));
                        if (!success) return OptionAsync<Set<B>>.None;
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new Set<B>(results);
                });

        public static Task<Set<B>> BindT<A, B>(this Task<Set<A>> ma, Func<A, Task<Set<B>>> f) =>
            ma.MapAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var imb = await f(ia);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new Set<B>(results);
                });

        public static EitherAsync<L, Set<B>> BindT<L, A, B>(this EitherAsync<L, Set<A>> ma, Func<A, EitherAsync<L, Set<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, left) = await f(ia).Map(b => (succ: true, res: b, left: default(L))).IfLeft(l => (false, Set<B>.Empty, l));
                        if (!success) return EitherAsync<L, Set<B>>.Left(left);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return new Set<B>(results);
                });

        public static TryAsync<Set<B>> BindT<A, B>(this TryAsync<Set<A>> ma, Func<A, TryAsync<Set<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Succ: b => (true, b, default(Exception)),
                                                                  Fail: ex => (false, Set<B>.Empty, ex));

                        if (!success) return TryAsync<Set<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryAsync(new Set<B>(results));
                });

        public static TryOptionAsync<Set<B>> BindT<A, B>(this TryOptionAsync<Set<A>> ma, Func<A, TryOptionAsync<Set<B>>> f) =>
            ma.BindAsync(
                async ima =>
                {
                    var results = new List<B>();
                    foreach (var ia in ima)
                    {
                        var (success, imb, e) = await f(ia).Match(Some: b => (true, b, default(Exception)),
                                                                  None: () => (true, Set<B>.Empty, default(Exception)),
                                                                  Fail: ex => (false, Set<B>.Empty, ex));

                        if (!success && e == null) return TryOptionAsync(Option<Set<B>>.None);
                        if (!success && e != null) return TryOptionAsync<Set<B>>(e);
                        foreach (var ib in imb)
                        {
                            results.Add(ib);
                        }
                    }
                    return TryOptionAsync(new Set<B>(results));
                });


        // Async : Validation

        public static OptionAsync<Validation<L, B>> BindT<L, A, B>(this OptionAsync<Validation<L, A>> ma, Func<A, OptionAsync<Validation<L, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => OptionAsync<Validation<L, B>>.Some(Validation<L, B>.Fail(e))));

        public static Task<Validation<L, B>> BindT<L, A, B>(this Task<Validation<L, A>> ma, Func<A, Task<Validation<L, B>>> f) =>
            ma.MapAsync(
                async ima =>
                    await ima.MatchAsync(
                        SuccAsync: async x => await f(x),
                        Fail: e => Validation<L, B>.Fail(e)));

        public static EitherAsync<L, Validation<FAIL, B>> BindT<L, FAIL, A, B>(this EitherAsync<L, Validation<FAIL, A>> ma, Func<A, EitherAsync<L, Validation<FAIL, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => EitherAsync<L, Validation<FAIL, B>>.Right(Validation<FAIL, B>.Fail(e))));

        public static TryAsync<Validation<L, B>> BindT<L, A, B>(this TryAsync<Validation<L, A>> ma, Func<A, TryAsync<Validation<L, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => TryAsync(Validation<L, B>.Fail(e))));

        public static TryOptionAsync<Validation<L, B>> BindT<L, A, B>(this TryOptionAsync<Validation<L, A>> ma, Func<A, TryOptionAsync<Validation<L, B>>> f) =>
            ma.Bind(
                ima => ima.Match(
                     Succ: x => f(x),
                     Fail: e => TryOptionAsync(Validation<L, B>.Fail(e))));
    }
}
