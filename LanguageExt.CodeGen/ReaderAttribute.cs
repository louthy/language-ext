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

        public ReaderAttribute(Type type) => 
            this.type = type;
    }
}
