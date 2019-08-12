using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;
using LanguageExt.CodeGen;

namespace LanguageExt
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute(typeof(ReaderGenerator))]
    [Conditional("CodeGeneration")]
    public class ReaderAttribute : Attribute
    {
        readonly Type type;
        readonly string constructor;
        readonly string fail;

        public ReaderAttribute(Type Env, string Constructor = "Return", string Fail = "Fail")
        {
            this.type = Env;
            this.constructor = Constructor;
            this.fail = Fail;
        }
    }
}
