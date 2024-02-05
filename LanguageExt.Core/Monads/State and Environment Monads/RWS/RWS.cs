using LanguageExt.TypeClasses;

namespace LanguageExt;

public delegate Fin<(W Output, S State, A Value)> RWS<MonoidW, in R, W, S, A>(R env, S state)
    where MonoidW : Monoid<W>;
