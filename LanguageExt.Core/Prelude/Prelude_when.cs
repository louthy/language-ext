using System;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Aff<RT, Unit> when<RT>(bool flag, Aff<RT, Unit> alternative) where RT : struct, HasCancel<RT> =>
            flag
                ? alternative
                : SuccessEff(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Aff<RT, Unit> unless<RT>(bool flag, Aff<RT, Unit> alternative) where RT : struct, HasCancel<RT> =>
            when(!flag, alternative);

        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Eff<RT, Unit> when<RT>(bool flag, Eff<RT, Unit> alternative) where RT : struct, HasCancel<RT> =>
            flag
                ? alternative
                : SuccessEff(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Eff<RT, Unit> unless<RT>(bool flag, Eff<RT, Unit> alternative) where RT : struct, HasCancel<RT> =>
            when(!flag, alternative);
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Aff<Unit> when(bool flag, Aff<Unit> alternative) =>
            flag
                ? alternative
                : SuccessEff(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Aff<Unit> unless(bool flag, Aff<Unit> alternative) =>
            when(!flag, alternative);

        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Eff<Unit> when(bool flag, Eff<Unit> alternative) =>
            flag
                ? alternative
                : SuccessEff(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Eff<Unit> unless(bool flag, Eff<Unit> alternative) =>
            when(!flag, alternative);        
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Try<Unit> when(bool flag, Try<Unit> alternative) =>
            flag
                ? alternative
                : TrySucc(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Try<Unit> unless(bool flag, Try<Unit> alternative) =>
            when(!flag, alternative);
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static TryAsync<Unit> when(bool flag, TryAsync<Unit> alternative) =>
            flag
                ? alternative
                : TryAsyncSucc(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static TryAsync<Unit> unless(bool flag, TryAsync<Unit> alternative) =>
            when(!flag, alternative);
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static TryOption<Unit> when(bool flag, TryOption<Unit> alternative) =>
            flag
                ? alternative
                : TryOptionSucc(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static TryOption<Unit> unless(bool flag, TryOption<Unit> alternative) =>
            when(!flag, alternative);
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static TryOptionAsync<Unit> when(bool flag, TryOptionAsync<Unit> alternative) =>
            flag
                ? alternative
                : TryOptionAsyncSucc(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static TryOptionAsync<Unit> unless(bool flag, TryOptionAsync<Unit> alternative) =>
            when(!flag, alternative);
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Option<Unit> when(bool flag, Option<Unit> alternative) =>
            flag
                ? alternative
                : Some(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Option<Unit> unless(bool flag, Option<Unit> alternative) =>
            when(!flag, alternative);    
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static OptionUnsafe<Unit> when(bool flag, OptionUnsafe<Unit> alternative) =>
            flag
                ? alternative
                : SomeUnsafe(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static OptionUnsafe<Unit> unless(bool flag, OptionUnsafe<Unit> alternative) =>
            when(!flag, alternative);
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static OptionAsync<Unit> when(bool flag, OptionAsync<Unit> alternative) =>
            flag
                ? alternative
                : SomeAsync(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static OptionAsync<Unit> unless(bool flag, OptionAsync<Unit> alternative) =>
            when(!flag, alternative);          
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Either<L, Unit> when<L>(bool flag, Either<L, Unit> alternative) =>
            flag
                ? alternative
                : Right<L, Unit>(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Either<L, Unit> unless<L>(bool flag, Either<L, Unit> alternative) =>
            when(!flag, alternative);      
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static EitherUnsafe<L, Unit> when<L>(bool flag, EitherUnsafe<L, Unit> alternative) =>
            flag
                ? alternative
                : RightUnsafe<L, Unit>(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static EitherUnsafe<L, Unit> unless<L>(bool flag, EitherUnsafe<L, Unit> alternative) =>
            when(!flag, alternative);      
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static EitherAsync<L, Unit> when<L>(bool flag, EitherAsync<L, Unit> alternative) =>
            flag
                ? alternative
                : RightAsync<L, Unit>(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static EitherAsync<L, Unit> unless<L>(bool flag, EitherAsync<L, Unit> alternative) =>
            when(!flag, alternative);
                
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Fin<Unit> when(bool flag, Fin<Unit> alternative) =>
            flag
                ? alternative
                : FinSucc(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Fin<Unit> unless(bool flag, Fin<Unit> alternative) =>
            when(!flag, alternative);

        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Task<Unit> when(bool flag, Task<Unit> alternative) =>
            flag
                ? alternative
                : unit.AsTask();

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Task<Unit> unless(bool flag, Task<Unit> alternative) =>
            when(!flag, alternative); 

        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static ValueTask<Unit> when(bool flag, ValueTask<Unit> alternative) =>
            flag
                ? alternative
                : unit.AsValueTask();

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static ValueTask<Unit> unless(bool flag, ValueTask<Unit> alternative) =>
            when(!flag, alternative); 
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Validation<L, Unit> when<L>(bool flag, Validation<L, Unit> alternative) =>
            flag
                ? alternative
                : Success<L, Unit>(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Validation<L, Unit> unless<L>(bool flag, Validation<L, Unit> alternative) =>
            when(!flag, alternative);      
        
        /// <summary>
        /// Run the alternative when the flag is true, return Pure Unit when false
        /// </summary>
        public static Validation<MonoidL, L, Unit> when<MonoidL, L>(bool flag, Validation<MonoidL, L, Unit> alternative) 
            where MonoidL : struct, Monoid<L>, Eq<L> =>
            flag
                ? alternative
                : Success<MonoidL, L, Unit>(unit);

        /// <summary>
        /// Run the alternative when the flag is false, return Pure Unit when true
        /// </summary>
        public static Validation<MonoidL, L, Unit> unless<MonoidL, L>(bool flag, Validation<MonoidL, L, Unit> alternative) 
            where MonoidL : struct, Monoid<L>, Eq<L> =>
                when(!flag, alternative);       
    }
}
