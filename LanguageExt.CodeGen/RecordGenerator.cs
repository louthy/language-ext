using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp;
using System.ComponentModel;

namespace LanguageExt.CodeGen
{
    public class RecordGenerator : ICodeGenerator
    {
        public RecordGenerator(AttributeData attributeData)
        {
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
            TransformationContext context,
            IProgress<Diagnostic> progress,
            CancellationToken cancellationToken)
        {
            if (context.ProcessingNode is TypeDeclarationSyntax applyTo && 
                (applyTo is ClassDeclarationSyntax || (applyTo is StructDeclarationSyntax)))
            {
                var (ok, record) = CodeGenUtil.MakeCaseType(
                                        context,
                                        progress,
                                        applyTo.Identifier,
                                        applyTo.Members,
                                        applyTo.TypeParameterList,
                                        applyTo.Modifiers,
                                        applyTo.ConstraintClauses,
                                        applyTo.Identifier,
                                        null,
                                        CodeGenUtil.GetState(context, progress, AllowedType.ClassOrStruct, "Record code-gen").Fields,
                                        BaseSpec.None,
                                        caseIsClass: applyTo is ClassDeclarationSyntax,
                                        caseIsPartial: true,
                                        includeWithAndLenses: true,
                                        includeMatch: false,
                                        -1);

                if (ok)
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>().Add(record));
                }
                else
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }
            }
            else 
            {
                CodeGenUtil.ReportError($"Type can't be made into a record.  It must be a partial class or a partial struct", "Union Code-Gen", context.ProcessingNode, progress);
                return Task.FromResult(List<MemberDeclarationSyntax>());
            }
        }
    }
}
