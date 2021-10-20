using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public static class FunctorClass<MA, A>
    {
        public static MB Map<MB, B>(MA ma, Func<A, B> f) => MapBuild<MB, B>.Default(ma, f);
        
        static class MapBuild<MB, B>
        {
            public static readonly Option<Error> Error;
            public static readonly Func<MA, Func<A, B>, MB> Default;

            static MapBuild()
            {
                try
                {
                    var (fullNameMA, nameMA, gensMA) = ClassFunctions.GetTypeInfo<MA>();
                    var (fullNameMB, nameMB, gensMB) = ClassFunctions.GetTypeInfo<MB>();

                    if (nameMA != nameMB)
                    {
                        Default = (MA x, Func<A, B> f) =>
                            throw new NotSupportedException(
                                $"Different outer types given for functor, they must match, like Option<A> and Option<B>");
                    }

                    var gens = Enumerable.Concat(gensMA, gensMB).ToArray();

                    Default = ClassFunctions.MakeFunc2<MA, Func<A, B>, MB>(nameMA, "Map", gens, 
                        ("Functor", ""), ("F", ""));

                    if (Default == null)
                    {
                        Default = (MA x, Func<A, B> f) =>
                            throw new NotSupportedException(
                                $"Functor{nameMA} or F{nameMA} instance not found for {fullNameMA} (FunctorClass.Map)");
                    }
                }
                catch (Exception e)
                {
                    Error = Some(Common.Error.New(e));
                    Default = (MA x, Func<A, B> f) => throw e;
                }
            }
        }
    }
}
