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
    public static class MonadClass<MA, A>
    {
        public static readonly Option<Error> Error;
        public static readonly Func<A, MA> Return;
        public static readonly Func<Error, MA> Fail;
        public static MB Bind<MB>(MA ma, Func<A, MB> f) => BindBuild<MB>.Default(ma, f);
        
        static MonadClass()
        {
            try
            {
                var (fullNameMA, nameMA, gensMA) = ClassFunctions.GetTypeInfo<MA>();

                Return = ClassFunctions.MakeFunc1<A, MA>(nameMA, "Return", gensMA, 
                    ("MonadReturn", ""), ("MReturn", ""), ("Monad", ""), ("M", ""));

                Fail = ClassFunctions.MakeFunc1<Error, MA>(nameMA, "Fail", gensMA, 
                    ("MonadFail", ""), ("MFail", ""), ("Monad", ""), ("M", ""));

                if (Return == null)
                {
                    Return = (A x) =>
                        throw new NotSupportedException(
                            $"MonadReturn{nameMA}, MReturn{nameMA}, Monad{nameMA} or M{nameMA} , instance not found for {fullNameMA} (MonadClass.Return)");
                }

                if (Fail == null)
                {
                    Fail = (Error x) =>
                        throw new NotSupportedException(
                            $"MonadFail{nameMA}, MFail{nameMA}, Monad{nameMA} or M{nameMA} , instance not found for {fullNameMA} (MonadClass.Fail)");
                }
            }
            catch (Exception e)
            {
                Error = Some(Common.Error.New(e));
                Fail = (Error x) => throw e;
                Return = (A x) => throw e;
            }
        }
        
        static class BindBuild<MB>
        {
            public static readonly Option<Error> Error;
            public static readonly Func<MA, Func<A, MB>, MB> Default;

            static BindBuild()
            {
                try
                {
                    var (fullNameMA, nameMA, gensMA) = ClassFunctions.GetTypeInfo<MA>();
                    var (fullNameMB, nameMB, gensMB) = ClassFunctions.GetTypeInfo<MB>();

                    if (nameMA != nameMB)
                    {
                        Default = (MA x, Func<A, MB> f) =>
                            throw new NotSupportedException(
                                $"Different outer types given for monad, they must match, like Option<A> and Option<B>");
                    }

                    var gens = Enumerable.Concat(gensMA, gensMB).ToArray();

                    Default = ClassFunctions.MakeFunc2<MA, Func<A, MB>, MB>(nameMA, "Bind", gens, 
                        ("MonadBind", ""), ("MBind", ""), ("Monad", ""), ("M", ""));

                    if (Default == null)
                    {
                        Default = (MA x, Func<A, MB> f) =>
                            throw new NotSupportedException(
                                $"MonadBind{nameMA} or MBind{nameMA} instance not found for {fullNameMA} (MonadClass.Bind)");
                    }
                }
                catch (Exception e)
                {
                    Error = Some(Common.Error.New(e));
                    Default = (MA x, Func<A, MB> f) => throw e;
                }
            }
        }
    }
}
