using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.CodeGen
{
    /// <summary>
    /// Provides a With function and lens fields for record types
    /// </summary>
    public class RecordWithAndLensGenerator : ICodeGenerator
    {
        /// <summary>
        /// Provides a With function and lens fields for record types
        /// </summary>
        public RecordWithAndLensGenerator(AttributeData attributeData)
        {
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var (partialClass, returnType, fields) = CodeGenUtil.GetState(context, progress, AllowedType.ClassOrStruct, "WithLens code-gen");

            partialClass = CodeGenUtil.AddWith(context, partialClass, returnType, fields);
            partialClass = CodeGenUtil.AddLenses(partialClass, returnType, fields);

            return Task.FromResult(SyntaxFactory.List<MemberDeclarationSyntax>().Add(partialClass));
        }
    }
}
