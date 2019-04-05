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

        public A Log<A>(A value)
        {
            Console.WriteLine(value);
            return value;
        }

        public A Log<A>(A value, string msg)
        {
            Console.WriteLine($"{msg} : {value}");
            return value;
        }

        public void LogLine(string msg)
        {
            Console.WriteLine(msg);
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
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
                                             .Where(m => m.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                                             .Where(m => !m.Modifiers.Any(SyntaxKind.StaticKeyword))
                                             .ToList();

            LogLine("withParams");

            var withParms = fields.Where(f => f.Declaration.Variables.Count > 0)
                                  .Select(f => (Id: f.Declaration.Variables[0].Identifier, Type: SyntaxFactory.GenericName(
                                                                SyntaxFactory.Identifier("Option"))
                                                                .WithTypeArgumentList(
                                                                    SyntaxFactory.TypeArgumentList(
                                                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(f.Declaration.Type)))))
                                  .Select(f =>
                                       SyntaxFactory.Parameter(MakeFirstCharUpper(f.Id))
                                                    .WithType(f.Type)
                                                    .WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(f.Type))))
                                  .ToArray();

            var withThisArg = SyntaxFactory.Parameter(SyntaxFactory.Identifier("self"))
                                           .WithType(returnType)
                                           .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)));

            var lst = (new[] { withThisArg }).ToList();
            lst.AddRange(withParms);
            var arr = lst.ToArray();

            var withArgs = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(arr));

            var withMethod = SyntaxFactory.MethodDeclaration(returnType, "With")
                                          .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                                          .WithParameterList(withArgs)
                                          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                          .WithExpressionBody(
                                              SyntaxFactory.ArrowExpressionClause(
                                                  SyntaxFactory.ObjectCreationExpression(
                                                      returnType,
                                                      SyntaxFactory.ArgumentList(
                                                          SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                              withParms.Select(wa =>
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName(wa.Identifier),
                                                                            SyntaxFactory.IdentifierName("IfNone")))
                                                                            .WithArgumentList(
                                                                                SyntaxFactory.ArgumentList(
                                                                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                                                        SyntaxFactory.Argument(
                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                SyntaxFactory.IdentifierName("self"),
                                                                                                SyntaxFactory.IdentifierName(wa.Identifier)))))))))),
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
