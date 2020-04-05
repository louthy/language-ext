using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;

namespace LanguageExt
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute("LanguageExt.CodeGen.ReaderGenerator, LanguageExt.CodeGen")]
    [Conditional("CodeGeneration")]
    public class ReaderAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Env">Reader environment type</param>
        /// <param name="Constructor">Optional name for the function that will construct the new monad</param>
        /// <param name="Fail">Optional name for the function that will represent the fail state for the new monad</param>
        public ReaderAttribute(Type Env, string Constructor = "Return", string Fail = "Fail")
        {
        }
    }
}
