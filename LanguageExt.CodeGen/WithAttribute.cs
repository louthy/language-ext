using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;
using LanguageExt.CodeGen;

namespace LanguageExt
{
    /// <summary>
    /// With attribute which tells the LanguageExt.CodeGen to provide a With function for all 
    /// public readonly fields in a type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute(typeof(RecordWithGenerator))]
    [Conditional("CodeGeneration")]
    public class WithAttribute : Attribute
    {
    }

    /// <summary>
    /// With attribute which tells the LanguageExt.CodeGen to provide a With function for all 
    /// public readonly fields in a type and a set of static lens fields that share the field 
    /// name with a lower-case letter (so Name and Surname become name and surname) for easy
    /// bidirectional immutable transformations. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute(typeof(RecordWithAndLensGenerator))]
    [Conditional("CodeGeneration")]
    public class WithLensAttribute : Attribute
    {
        public WithLensAttribute()
        {
        }
    }
}
