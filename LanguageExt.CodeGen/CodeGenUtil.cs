using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.CodeGen
{
    internal static class CodeGenUtil
    {
        public static ClassDeclarationSyntax AddLenses(ClassDeclarationSyntax partialClass, TypeSyntax returnType, System.Collections.Generic.List<FieldDeclarationSyntax> fields)
        {
            foreach (var field in fields)
            {
                partialClass = AddLens(partialClass, returnType, field);
            }
            return partialClass;
        }

        public static ClassDeclarationSyntax AddLens(ClassDeclarationSyntax partialClass, TypeSyntax returnType, FieldDeclarationSyntax field)
        {
            var lfield = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier("Lens"))
                                 .WithTypeArgumentList(
                                    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(new[] { returnType, field.Declaration.Type }))))
                             .WithVariables(
                                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    SyntaxFactory.VariableDeclarator(MakeFirstCharLower(field.Declaration.Variables[0].Identifier))
                                                 .WithInitializer(
                                                    SyntaxFactory.EqualsValueClause(
                                                        SyntaxFactory.InvocationExpression(
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.GenericName("Lens")
                                                                                    .WithTypeArgumentList(
                                                                                        SyntaxFactory.TypeArgumentList(
                                                                                            SyntaxFactory.SeparatedList<TypeSyntax>(new[] { returnType, field.Declaration.Type }))),
                                                                                    SyntaxFactory.IdentifierName("New")))
                                                                     .WithArgumentList(
                                                                        SyntaxFactory.ArgumentList(
                                                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[] {
                                                                                    SyntaxFactory.Argument(
                                                                                        SyntaxFactory.SimpleLambdaExpression(
                                                                                            SyntaxFactory.Parameter(
                                                                                                SyntaxFactory.Identifier("_x")),
                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                SyntaxFactory.IdentifierName("_x"),
                                                                                                SyntaxFactory.IdentifierName(field.Declaration.Variables[0].Identifier.ToString())))),
                                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                                    SyntaxFactory.Argument(
                                                                                        SyntaxFactory.SimpleLambdaExpression(
                                                                                            SyntaxFactory.Parameter(
                                                                                                SyntaxFactory.Identifier("_x")),
                                                                                                SyntaxFactory.SimpleLambdaExpression(
                                                                                                    SyntaxFactory.Parameter(
                                                                                                        SyntaxFactory.Identifier("_y")),
                                                                                                        SyntaxFactory.InvocationExpression(
                                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                                SyntaxFactory.IdentifierName("_y"),
                                                                                                                SyntaxFactory.IdentifierName("With")))
                                                                                                            .WithArgumentList(
                                                                                                                SyntaxFactory.ArgumentList(
                                                                                                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                                                                                        SyntaxFactory.Argument(
                                                                                                                            SyntaxFactory.IdentifierName("_x"))
                                                                                                                            .WithNameColon(
                                                                                                                                SyntaxFactory.NameColon(field.Declaration.Variables[0].Identifier.ToString()))))))))

                                                                                }))))))));

            lfield = lfield.WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)));

            return partialClass.AddMembers(lfield);
        }

        public static ClassDeclarationSyntax AddWith(ClassDeclarationSyntax partialClass, TypeSyntax returnType, List<FieldDeclarationSyntax> fields)
        {
            var withParms = fields.Where(f => f.Declaration.Variables.Count > 0)
                                  .Select(f => (Id: f.Declaration.Variables[0].Identifier, Type: SyntaxFactory.GenericName(
                                                                SyntaxFactory.Identifier("WithOpt"))
                                                                .WithTypeArgumentList(
                                                                    SyntaxFactory.TypeArgumentList(
                                                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(f.Declaration.Type)))))
                                  .Select(f =>
                                       SyntaxFactory.Parameter(MakeFirstCharUpper(f.Id))
                                                    .WithType(f.Type)
                                                    .WithDefault(SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(f.Type))))
                                  .ToArray();

            var withMethod = SyntaxFactory.MethodDeclaration(returnType, "With")
                                          .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(withParms)))
                                          .WithModifiers(SyntaxFactory.TokenList(
                                              SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
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
                                                                                                SyntaxFactory.ThisExpression(),
                                                                                                 SyntaxFactory.IdentifierName(wa.Identifier)))))))))),
                                                  null)));

            partialClass = partialClass.AddMembers(withMethod);
            return partialClass;
        }

        public static TypeSyntax TypeFromClass(ClassDeclarationSyntax decl) =>
            SyntaxFactory.IdentifierName(decl.Identifier);

        public static SyntaxToken MakeFirstCharUpper(SyntaxToken identifier)
        {
            var id = identifier.ToString();
            var id2 = $"{Char.ToUpper(id[0])}{id.Substring(1)}";
            return SyntaxFactory.Identifier(id2);
        }

        public static SyntaxToken MakeFirstCharLower(SyntaxToken identifier)
        {
            var id = identifier.ToString();
            var id2 = $"{Char.ToLower(id[0])}{id.Substring(1)}";
            return SyntaxFactory.Identifier(id2);
        }
    }
}
