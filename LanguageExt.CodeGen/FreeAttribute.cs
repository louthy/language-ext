using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CodeGeneration.Roslyn;
using LanguageExt.CodeGen;

namespace LanguageExt
{
    /// <summary>
    /// Union attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [CodeGenerationAttribute(typeof(FreeGenerator))]
    [Conditional("CodeGeneration")]
    public class FreeAttribute : Attribute
    {
    }
}
