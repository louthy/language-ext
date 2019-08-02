using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.CodeGen
{
    /// <summary>
    /// Reader Generator
    /// </summary>
    public class ReaderGenerator : ICodeGenerator
    {
        readonly string envType;

        /// <summary>
        /// Provides a With function for record types
        /// </summary>
        public ReaderGenerator(AttributeData attributeData) =>
            envType = attributeData.ConstructorArguments[0].Value.ToString();

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
            TransformationContext context, 
            IProgress<Diagnostic> progress, 
            CancellationToken cancellationToken)
        {
            var results = SyntaxFactory.List<MemberDeclarationSyntax>();

            if (context.ProcessingNode is StructDeclarationSyntax applyToStruct)
            {
                // Apply a suffix to the name of a copy of the struct.
                var partialStruct = SyntaxFactory.StructDeclaration($"{applyToStruct.Identifier}")
                                                 .WithModifiers(applyToStruct.Modifiers)
                                                 .WithTypeParameterList(applyToStruct.TypeParameterList);

                var compField = SyntaxFactory.FieldDeclaration(
                                    SyntaxFactory.VariableDeclaration(
                                        SyntaxFactory.ParseTypeName($"LanguageExt.Reader<{envType}, {applyToStruct.TypeParameterList.Parameters.Last()}>"),
                                        SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>(
                                            SyntaxFactory.NodeOrTokenList(
                                                SyntaxFactory.VariableDeclarator("__comp")))));

                partialStruct = partialStruct.AddMembers(compField);
                return Task.FromResult<SyntaxList<MemberDeclarationSyntax>>(results.Add(partialStruct));
            }
            else
            {
                return Task.FromResult<SyntaxList<MemberDeclarationSyntax>>(results);
            }
        }
    }
}
