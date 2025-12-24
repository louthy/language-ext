using LanguageExt.Traits;

namespace LanguageExt;

public static partial class StateExtensions
{
    extension<S, A>(K<State<S>, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static State<S, A> operator +(K<State<S>, A> ma) =>
            (State<S, A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static State<S, A> operator >> (K<State<S>, A> ma, Lower lower) =>
            +ma;
    }
}
