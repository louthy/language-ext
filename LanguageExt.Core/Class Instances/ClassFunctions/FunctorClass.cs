using System;
using System.Linq;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

public static class FunctorClass<MA, A>
{
    public static MB Map<MB, B>(MA ma, Func<A, B> f) => 
        MapBuild<MB, B>.Default(ma, f);
        
    static class MapBuild<MB, B>
    {
        public static readonly Option<Error> Error;
        public static readonly Func<MA, Func<A, B>, MB> Default;

        static MapBuild()
        {
            try
            {
                var (fullNameMA, nameMA, gensMA) = ClassFunctions.GetTypeInfo<MA>();
                var (_, nameMB, gensMB)          = ClassFunctions.GetTypeInfo<MB>();

                if (nameMA != nameMB)
                {
                    Default = (_, _) =>
                        throw new NotSupportedException(
                            $"Different outer types given for functor, they must match, like Option<A> and Option<B>");
                }

                var gens = gensMA.Concat(gensMB).ToArray();

                var f = ClassFunctions.MakeFunc2<MA, Func<A, B>, MB>(nameMA, "Map", gens, 
                                                                       ("Functor", ""), ("F", ""));

                if (f is null)
                {
                    Default = (_, _) =>
                        throw new NotSupportedException(
                            $"Functor{nameMA} or F{nameMA} instance not found for {fullNameMA} (FunctorClass.Map)");
                }
                else
                {
                    Default = f;
                }
            }
            catch (Exception e)
            {
                Error   = Some(Common.Error.New(e));
                Default = (_, _) => throw e;
            }
        }
    }
}
