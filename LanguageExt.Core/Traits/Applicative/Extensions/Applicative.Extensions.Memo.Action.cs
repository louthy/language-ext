using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    extension<F, A>(K<F, A> ma) where F : Applicative<F>
    {
        [Pure]
        public K<F, B> Action<B>(Memo<F, B> mb) =>
            F.Action(ma, mb);
        
        [Pure]
        public K<F, A> BackAction<B>(Memo<F, B> mb) =>
            F.BackAction(ma, mb);
    }
    
    extension<F, A>(Memo<F, A> ma) where F : Applicative<F>
    {
        [Pure]
        public K<F, B> Action<B>(Memo<F, B> mb) =>
            F.Action(ma, mb);
        
        [Pure]
        public K<F, A> BackAction<B>(Memo<F, B> mb) =>
            F.BackAction(ma, mb);
    }
}
