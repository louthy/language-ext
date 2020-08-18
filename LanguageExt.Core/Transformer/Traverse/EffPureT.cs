using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Interfaces;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class EffPureT
    {
        //
        // Collections
        //

        [Pure]
        public static EffPure<Arr<B>> Traverse<A, B>(this Arr<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<Arr<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<Arr<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new Arr<B>(rs.ToArray()));
            });


        [Pure]
        public static EffPure<HashSet<B>> Traverse<A, B>(this HashSet<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<HashSet<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<HashSet<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new HashSet<B>(rs));
            });

        
        [Pure]
        public static EffPure<IEnumerable<B>> Traverse<A, B>(this IEnumerable<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<IEnumerable<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<IEnumerable<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.AsEnumerable());
            });

        [Pure]
        public static EffPure<Lst<B>> Traverse<A, B>(this Lst<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<Lst<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<Lst<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.Freeze());
            });


        [Pure]
        public static EffPure<Que<B>> Traverse<A, B>(this Que<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<Que<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<Que<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toQueue(rs));
            });


        [Pure]
        public static EffPure<Seq<B>> Traverse<A, B>(this Seq<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<Seq<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<Seq<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(Seq.FromArray(rs.ToArray()));
            });

        [Pure]
        public static EffPure<Set<B>> Traverse<A, B>(this Set<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<Set<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<Set<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toSet(rs));
            });
        

        [Pure]
        public static EffPure<Stck<B>> Traverse<A, B>(this Stck<EffPure<A>> ma, Func<A, B> f) =>
            EffMaybe<Stck<B>>(() =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.RunIO();
                    if (r.IsFail) return FinFail<Stck<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toStack(rs));
            });
        
        
        //
        // Sync types
        // 
        
        public static EffPure<Either<L, B>> Traverse<L, A, B>(this Either<L, EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<Either<L, B>>(() => Go(ma, f));
            Fin<Either<L, B>> Go(Either<L, EffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<Either<L, B>>(ma.LeftValue);
                var rb = ma.RightValue.RunIO();
                if(rb.IsFail) return FinFail<Either<L, B>>(rb.Error);
                return FinSucc<Either<L, B>>(f(rb.Value));
            }
        }

        public static EffPure<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<EitherUnsafe<L, B>>(() => Go(ma, f));
            Fin<EitherUnsafe<L, B>> Go(EitherUnsafe<L, EffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<EitherUnsafe<L, B>>(ma.LeftValue);
                var rb = ma.RightValue.RunIO();
                if(rb.IsFail) return FinFail<EitherUnsafe<L, B>>(rb.Error);
                return FinSucc<EitherUnsafe<L, B>>(f(rb.Value));
            }
        }

        public static EffPure<Identity<B>> Traverse<A, B>(this Identity<EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<Identity<B>>(() => Go(ma, f));
            Fin<Identity<B>> Go(Identity<EffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                var rb = ma.Value.RunIO();
                if(rb.IsFail) return FinFail<Identity<B>>(rb.Error);
                return FinSucc<Identity<B>>(new Identity<B>(f(rb.Value)));
            }
        }

        public static EffPure<Option<B>> Traverse<A, B>(this Option<EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<Option<B>>(() => Go(ma, f));
            Fin<Option<B>> Go(Option<EffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<Option<B>>(None);
                var rb = ma.Value.RunIO();
                if(rb.IsFail) return FinFail<Option<B>>(rb.Error);
                return FinSucc<Option<B>>(Option<B>.Some(f(rb.Value)));
            }
        }
        
        public static EffPure<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<OptionUnsafe<B>>(() => Go(ma, f));
            Fin<OptionUnsafe<B>> Go(OptionUnsafe<EffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<OptionUnsafe<B>>(None);
                var rb = ma.Value.RunIO();
                if(rb.IsFail) return FinFail<OptionUnsafe<B>>(rb.Error);
                return FinSucc<OptionUnsafe<B>>(OptionUnsafe<B>.Some(f(rb.Value)));
            }
        }
        
        public static EffPure<Try<B>> Traverse<A, B>(this Try<EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<Try<B>>(() => Go(ma, f));
            Fin<Try<B>> Go(Try<EffPure<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsFaulted) return FinSucc<Try<B>>(TryFail<B>(ra.Exception));
                var rb = ra.Value.RunIO();
                if (rb.IsFail) return FinFail<Try<B>>(rb.Error);
                return FinSucc<Try<B>>(Try<B>(f(rb.Value)));
            }
        }
        
        public static EffPure<TryOption<B>> Traverse<A, B>(this TryOption<EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<TryOption<B>>(() => Go(ma, f));
            Fin<TryOption<B>> Go(TryOption<EffPure<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsBottom) return default;
                if (ra.IsNone) return FinSucc<TryOption<B>>(TryOptional<B>(None));
                if (ra.IsFaulted) return FinSucc<TryOption<B>>(TryOptionFail<B>(ra.Exception));
                var rb = ra.Value.Value.RunIO();
                if (rb.IsFail) return FinFail<TryOption<B>>(rb.Error);
                return FinSucc<TryOption<B>>(TryOption<B>(f(rb.Value)));
            }
        }
        
        public static EffPure<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, EffPure<A>> ma, Func<A, B> f)
        {
            return EffMaybe<Validation<Fail, B>>(() => Go(ma, f));
            Fin<Validation<Fail, B>> Go(Validation<Fail, EffPure<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var rb = ma.SuccessValue.RunIO();
                if(rb.IsFail) return FinFail<Validation<Fail, B>>(rb.Error);
                return FinSucc<Validation<Fail, B>>(f(rb.Value));
            }
        }
        
        public static EffPure<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, EffPure<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return EffMaybe<Validation<MonoidFail, Fail, B>>(() => Go(ma, f));
            Fin<Validation<MonoidFail, Fail, B>> Go(Validation<MonoidFail, Fail, EffPure<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = ma.SuccessValue.RunIO();
                if(rb.IsFail) return FinFail<Validation<MonoidFail, Fail, B>>(rb.Error);
                return FinSucc<Validation<MonoidFail, Fail, B>>(f(rb.Value));
            }
        }
    }
}
