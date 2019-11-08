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
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute(typeof(UnionGenerator))]
    [Conditional("CodeGeneration")]
    public class UnionAttribute : Attribute
    {
    }
}
