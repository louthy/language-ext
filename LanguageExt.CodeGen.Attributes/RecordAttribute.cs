using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;

namespace LanguageExt
{
    /// <summary>
    /// Union attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [CodeGenerationAttribute("LanguageExt.CodeGen.RecordGenerator, LanguageExt.CodeGen.Generators")]
    [Conditional("CodeGeneration")]
    public class RecordAttribute : Attribute
    {
    }
}
