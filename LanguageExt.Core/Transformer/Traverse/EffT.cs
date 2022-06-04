#nullable enable
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
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class EffT
    {
        //
        // Collections
        //

        [Pure]
        public static Eff<RT, Arr<B>> Traverse<RT, A, B>(this Arr<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, Arr<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<Arr<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new Arr<B>(rs.ToArray()));
            });


        [Pure]
        public static Eff<RT, HashSet<B>> Traverse<RT, A, B>(this HashSet<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, HashSet<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<HashSet<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new HashSet<B>(rs));
            });

        
        

        [Pure]
        public static Eff<RT, IEnumerable<B>> Traverse<RT, A, B>(this IEnumerable<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, IEnumerable<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<IEnumerable<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.AsEnumerable());
            });

        [Pure]
        public static Eff<RT, Lst<B>> Traverse<RT, A, B>(this Lst<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, Lst<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<Lst<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.Freeze());
            });


        
        [Pure]
        public static Eff<RT, Que<B>> Traverse<RT, A, B>(this Que<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, Que<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<Que<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toQueue(rs));
            });

        
        
        
        [Pure]
        public static Eff<RT, Seq<B>> Traverse<RT, A, B>(this Seq<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, Seq<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<Seq<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(Seq.FromArray(rs.ToArray()));
            });

         

        [Pure]
        public static Eff<RT, Set<B>> Traverse<RT, A, B>(this Set<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, Set<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<Set<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toSet(rs));
            });
        
        

        [Pure]
        public static Eff<RT, Stck<B>> Traverse<RT, A, B>(this Stck<Eff<RT, A>> ma, Func<A, B> f) where RT : struct =>
            EffMaybe<RT, Stck<B>>(env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = m.Run(env);
                    if (r.IsFail) return FinFail<Stck<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toStack(rs));
            });
        
        //
        // Sync types
        // 
        
        public static Eff<RT, Either<L, B>> Traverse<RT, L, A, B>(this Either<L, Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, Either<L, B>>(env => Go(env, ma, f));
            Fin<Either<L, B>> Go(RT env, Either<L, Eff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<Either<L, B>>(ma.LeftValue);
                var rb = ma.RightValue.Run(env);
                if(rb.IsFail) return FinFail<Either<L, B>>(rb.Error);
                return FinSucc<Either<L, B>>(f(rb.Value));
            }
        }

        public static Eff<RT, EitherUnsafe<L, B>> Traverse<RT, L, A, B>(this EitherUnsafe<L, Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, EitherUnsafe<L, B>>(env => Go(env, ma, f));
            Fin<EitherUnsafe<L, B>> Go(RT env, EitherUnsafe<L, Eff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<EitherUnsafe<L, B>>(ma.LeftValue);
                var rb = ma.RightValue.Run(env);
                if(rb.IsFail) return FinFail<EitherUnsafe<L, B>>(rb.Error);
                return FinSucc<EitherUnsafe<L, B>>(f(rb.Value));
            }
        }

        public static Eff<RT, Identity<B>> Traverse<RT, A, B>(this Identity<Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, Identity<B>>(env => Go(env, ma, f));
            Fin<Identity<B>> Go(RT env, Identity<Eff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                var rb = ma.Value.Run(env);
                if(rb.IsFail) return FinFail<Identity<B>>(rb.Error);
                return FinSucc<Identity<B>>(new Identity<B>(f(rb.Value)));
            }
        }

        public static Eff<RT, Fin<B>> Traverse<RT, A, B>(this Fin<Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, Fin<B>>(env => Go(env, ma, f));
            Fin<Fin<B>> Go(RT env, Fin<Eff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return FinSucc<Fin<B>>(ma.Cast<B>());
                var rb = ma.Value.Run(env);
                if(rb.IsFail) return FinFail<Fin<B>>(rb.Error);
                return FinSucc<Fin<B>>(Fin<B>.Succ(f(rb.Value)));
            }
        }
        
        public static Eff<RT, Option<B>> Traverse<RT, A, B>(this Option<Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, Option<B>>(env => Go(env, ma, f));
            Fin<Option<B>> Go(RT env, Option<Eff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<Option<B>>(None);
                var rb = ma.Value.Run(env);
                if(rb.IsFail) return FinFail<Option<B>>(rb.Error);
                return FinSucc<Option<B>>(Option<B>.Some(f(rb.Value)));
            }
        }
        
        public static Eff<RT, OptionUnsafe<B>> Traverse<RT, A, B>(this OptionUnsafe<Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, OptionUnsafe<B>>(env => Go(env, ma, f));
            Fin<OptionUnsafe<B>> Go(RT env, OptionUnsafe<Eff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<OptionUnsafe<B>>(None);
                var rb = ma.Value.Run(env);
                if(rb.IsFail) return FinFail<OptionUnsafe<B>>(rb.Error);
                return FinSucc<OptionUnsafe<B>>(OptionUnsafe<B>.Some(f(rb.Value)));
            }
        }
        
        public static Eff<RT, Try<B>> Traverse<RT, A, B>(this Try<Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, Try<B>>(env => Go(env, ma, f));
            Fin<Try<B>> Go(RT env, Try<Eff<RT, A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsFaulted) return FinSucc<Try<B>>(TryFail<B>(ra.Exception));
                var rb = ra.Value.Run(env);
                if (rb.IsFail) return FinFail<Try<B>>(rb.Error);
                return FinSucc<Try<B>>(Try<B>(f(rb.Value)));
            }
        }
        
        public static Eff<RT, TryOption<B>> Traverse<RT, A, B>(this TryOption<Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, TryOption<B>>(env => Go(env, ma, f));
            Fin<TryOption<B>> Go(RT env, TryOption<Eff<RT, A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsBottom) return default;
                if (ra.IsNone) return FinSucc<TryOption<B>>(TryOptional<B>(None));
                if (ra.IsFaulted) return FinSucc<TryOption<B>>(TryOptionFail<B>(ra.Exception));
                var rb = ra.Value.Value.Run(env);
                if (rb.IsFail) return FinFail<TryOption<B>>(rb.Error);
                return FinSucc<TryOption<B>>(TryOption<B>(f(rb.Value)));
            }
        }
        
        public static Eff<RT, Validation<Fail, B>> Traverse<RT, Fail, A, B>(this Validation<Fail, Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
        {
            return EffMaybe<RT, Validation<Fail, B>>(env => Go(env, ma, f));
            Fin<Validation<Fail, B>> Go(RT env, Validation<Fail, Eff<RT, A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var rb = ma.SuccessValue.Run(env);
                if(rb.IsFail) return FinFail<Validation<Fail, B>>(rb.Error);
                return FinSucc<Validation<Fail, B>>(f(rb.Value));
            }
        }
        
        public static Eff<RT, Validation<MonoidFail, Fail, B>> Traverse<RT, MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return EffMaybe<RT, Validation<MonoidFail, Fail, B>>(env => Go(env, ma, f));
            Fin<Validation<MonoidFail, Fail, B>> Go(RT env, Validation<MonoidFail, Fail, Eff<RT, A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = ma.SuccessValue.Run(env);
                if(rb.IsFail) return FinFail<Validation<MonoidFail, Fail, B>>(rb.Error);
                return FinSucc<Validation<MonoidFail, Fail, B>>(f(rb.Value));
            }
        }
        
        public static Eff<RT, Eff<B>> Traverse<RT, A, B>(this Eff<Eff<RT, A>> ma, Func<A, B> f)
            where RT : struct         {
            return EffMaybe<RT, Eff<B>>(env => Go(env, ma, f));
            Fin<Eff<B>> Go(RT env, Eff<Eff<RT, A>> ma, Func<A, B> f)
            {
                var ra = ma.Run();
                if (ra.IsFail) return FinSucc<Eff<B>>(FailEff<B>(ra.Error));
                var rb = ra.Value.Run(env);
                if (rb.IsFail) return FinFail<Eff<B>>(rb.Error);
                return FinSucc<Eff<B>>(SuccessEff<B>(f(rb.Value)));
            }
        }
    }
}
