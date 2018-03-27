using System;
using System.Collections.Generic;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.TypeClasses;
using LanguageExt.DataTypes.Serialisation;

public static partial class TaskEitherAsyncExtensions
{
    public static EitherAsync<L, R> ToAsync<L, R>(this Task<Either<L, R>> ma) =>
        new EitherAsync<L, R>(
            ma.Map(a => 
                a.Match(r => new EitherData<L, R>(EitherStatus.IsRight, r,default(L)), 
                        l => new EitherData<L, R>(EitherStatus.IsRight, default(R),l),
                        () => new EitherData<L, R>(EitherStatus.IsBottom, default(R), default(L)))));
}
