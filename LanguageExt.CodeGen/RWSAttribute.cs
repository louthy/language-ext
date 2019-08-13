using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;
using LanguageExt.CodeGen;

namespace LanguageExt
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute(typeof(RWSGenerator))]
    [Conditional("CodeGeneration")]
    public class RWSAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="WriterMonoid">struct type derived from Monoid<W></param>
        /// <param name="Env">Reader environment type</param>
        /// <param name="Constructor">Optional name for the function that will construct the new monad</param>
        /// <param name="Fail">Optional name for the function that will represent the fail state for the new monad</param>
        public RWSAttribute(Type WriterMonoid, Type Env, string Constructor = "Return", string Fail = "Fail")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="WriterMonoid">struct type derived from Monoid<W></param>
        /// <param name="Env">Reader environment type</param>
        /// <param name="State">State type</param>
        /// <param name="Constructor">Optional name for the function that will construct the new monad</param>
        /// <param name="Fail">Optional name for the function that will represent the fail state for the new monad</param>
        public RWSAttribute(Type WriterMonoid, Type Env, Type State, string Constructor = "Return", string Fail = "Fail")
        {
        }
    }
}
