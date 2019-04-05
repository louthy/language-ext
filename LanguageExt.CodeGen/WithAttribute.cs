using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;
using LanguageExt.CodeGen;

namespace LanguageExt
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute(typeof(RecordWithGenerator))]
    [Conditional("CodeGeneration")]
    public class WithAttribute : Attribute
    {
    }

    //[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    //[CodeGenerationAttribute("LanguageExt.CodeGen.RecordWithAndLensGenerator, LanguageExt.CodeGen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    //[Conditional("CodeGeneration")]
    //public class WithLensAttribute : Attribute
    //{
    //    public WithLensAttribute()
    //    {
    //    }
    //}
}
