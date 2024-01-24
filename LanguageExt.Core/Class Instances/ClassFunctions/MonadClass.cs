using System;
using System.Linq;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances;

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

            var @return = ClassFunctions.MakeFunc1<A, MA>(
                nameMA,
                "Return",
                gensMA,
                ("MonadReturn", ""),
                ("MReturn", ""),
                ("Monad", ""),
                ("M", ""));

            var fail = ClassFunctions.MakeFunc1<Error, MA>(
                nameMA,
                "Fail",
                gensMA,
                ("MonadFail", ""),
                ("MFail", ""),
                ("Monad", ""),
                ("M", ""));

            if (@return is null)
            {
                Return = _ =>
                    throw new NotSupportedException(
                        $"MonadReturn{nameMA}, MReturn{nameMA}, Monad{nameMA} or M{nameMA} , instance not found for {fullNameMA} (MonadClass.Return)");
            }
            else
            {
                Return = @return;
            }

            if (fail is null)
            {
                Fail = _ =>
                    throw new NotSupportedException(
                        $"MonadFail{nameMA}, MFail{nameMA}, Monad{nameMA} or M{nameMA} , instance not found for {fullNameMA} (MonadClass.Fail)");
            }
            else
            {
                Fail = fail;
            }
        }
        catch (Exception e)
        {
            Error  = Some(Common.Error.New(e));
            Fail   = _ => throw e;
            Return = _ => throw e;
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
                var (_, nameMB, gensMB)          = ClassFunctions.GetTypeInfo<MB>();

                if (nameMA != nameMB)
                {
                    Default = (_, _) =>
                        throw new NotSupportedException(
                            "Different outer types given for monad, they must match, like Option<A> and Option<B>");
                }

                var gens = gensMA.Concat(gensMB).ToArray();

                var @default = ClassFunctions.MakeFunc2<MA, Func<A, MB>, MB>(nameMA, "Bind", gens, 
                                                                        ("MonadBind", ""), ("MBind", ""), ("Monad", ""), ("M", ""));

                if (@default is null)
                {
                    Default = (_, _) =>
                        throw new NotSupportedException(
                            $"MonadBind{nameMA} or MBind{nameMA} instance not found for {fullNameMA} (MonadClass.Bind)");
                }
                else
                {
                    Default = @default;
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
