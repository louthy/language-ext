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
        private readonly string suffix;

        public RecordWithGenerator(AttributeData attributeData)
        {
            //Requires.NotNull(attributeData, nameof(attributeData));
            //this.suffix = (string)attributeData.ConstructorArguments[0].Value;
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            File.WriteAllText("c:\\codegen.txt", "running");

            var results = SyntaxFactory.List<MemberDeclarationSyntax>();

            // Our generator is applied to any class that our attribute is applied to.
            var applyToClass = (ClassDeclarationSyntax)context.ProcessingNode;

            // Apply a suffix to the name of a copy of the class.
            var extensionClass = SyntaxFactory.ClassDeclaration($"{applyToClass.Identifier}__Extensions")
                                              .WithModifiers(
                                                   SyntaxFactory.TokenList(
                                                       SyntaxFactory.Token(SyntaxKind.PublicKeyword), 
                                                       SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

            var returnType = TypeFromClass(applyToClass);

            var fields = applyToClass.Members.Where(m => m is FieldDeclarationSyntax)
                                             .Select(m => m as FieldDeclarationSyntax)
                                             .Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword))
                                             .Where(m => !m.Modifiers.Any(SyntaxKind.StaticKeyword))
                                             .ToList();

            var withParms = fields.Select(f =>
                                       SyntaxFactory.Parameter(MakeFirstCharUpper(f.Declaration.Variables.First().Identifier))
                                                    .WithType(SyntaxFactory.ParseTypeName($"LanguageExt.Option<{f.Declaration.Type}>"))
                                                    .WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.DefaultKeyword)))
                                          ).ToArray();

            var withThisArg = SyntaxFactory.Parameter(SyntaxFactory.Identifier("self"))
                                           .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)));

            var lst = (new[] { withThisArg }).ToList();
            lst.AddRange(withParms);
            var arr = lst.ToArray();

            var withArgs = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(arr));

            var withMethod = SyntaxFactory.MethodDeclaration(returnType, "With")
                                          .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                                          .WithParameterList(withArgs)
                                          .WithExpressionBody(
                                              SyntaxFactory.ArrowExpressionClause(
                                                  SyntaxFactory.ObjectCreationExpression(
                                                      returnType,
                                                      SyntaxFactory.ArgumentList(
                                                          SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                              withParms.Select(wa =>
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.BinaryExpression(
                                                                        SyntaxKind.LogicalOrExpression,
                                                                        SyntaxFactory.IdentifierName(wa.Identifier),
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName("self"),
                                                                            SyntaxFactory.IdentifierName(wa.Identifier))
                                                                        ))))),
                                                      null)));

            //var ctorArgs = SyntaxFactory.ParameterList(
            //                   SyntaxFactory.SeparatedList(
            //                       fields.Select(f =>
            //                           SyntaxFactory.Parameter(f.Declaration.Variables.First().Identifier)
            //                                        .WithType(f.Declaration.Type)).ToArray()));

            //var ctorMethod = SyntaxFactory.ConstructorDeclaration(applyToClass.Identifier)
            //                              .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            //                              .WithParameterList(ctorArgs);

            extensionClass = extensionClass.AddMembers(withMethod);

            return Task.FromResult<SyntaxList<MemberDeclarationSyntax>>(results.Add(extensionClass));
        }

        static TypeSyntax TypeFromClass(ClassDeclarationSyntax decl) =>
            SyntaxFactory.IdentifierName(decl.Identifier);

        static SyntaxToken MakeFirstCharUpper(SyntaxToken identifier)
        {
            var id = identifier.ToString();
            var id2 = $"{Char.ToUpper(id[0])}{id.Substring(1)}";
            return SyntaxFactory.Identifier(id2);
        }
    }
}
