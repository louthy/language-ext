using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using Validation;

namespace LanguageExt.CodeGen
{
    public class RecordWithGenerator : ICodeGenerator
    {
        public RecordWithGenerator(AttributeData attributeData)
        {
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var results = SyntaxFactory.List<MemberDeclarationSyntax>();

            // Our generator is applied to any class that our attribute is applied to.
            var applyToClass = (ClassDeclarationSyntax)context.ProcessingNode;

            // Apply a suffix to the name of a copy of the class.
            var partialClass = SyntaxFactory.ClassDeclaration($"{applyToClass.Identifier}")
                                            .WithModifiers(
                                                 SyntaxFactory.TokenList(
                                                     SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                                     SyntaxFactory.Token(SyntaxKind.PartialKeyword)));

            var returnType = CodeGenUtil.TypeFromClass(applyToClass);

            var fields = applyToClass.Members.Where(m => m is FieldDeclarationSyntax)
                                             .Select(m => m as FieldDeclarationSyntax)
                                             .Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword))
                                             .Where(m => m.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                                             .Where(m => !m.Modifiers.Any(SyntaxKind.StaticKeyword))
                                             .ToList();

            partialClass = CodeGenUtil.AddWith(partialClass, returnType, fields);

            return Task.FromResult<SyntaxList<MemberDeclarationSyntax>>(results.Add(partialClass));
        }

    }
}
