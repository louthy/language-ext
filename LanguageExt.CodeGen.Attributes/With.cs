using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;

namespace LanguageExt
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute("LanguageExt.CodeGen.RecordWithGenerator, LanguageExt.CodeGen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [Conditional("CodeGeneration")]
    public class WithAttribute : Attribute
    {
        public WithAttribute()
        {
        }
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
