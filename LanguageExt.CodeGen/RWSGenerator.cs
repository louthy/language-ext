using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Collections.Generic;

namespace LanguageExt.CodeGen
{
    /// <summary>
    /// RWS<MonoidW, R, W, S, A> Generator
    /// </summary>
    public class RWSGenerator : ICodeGenerator
    {
        readonly string monoidWType;
        readonly string rType;
        readonly object rTypeConst;
        readonly string wType;
        readonly string sType;
        readonly object sTypeConst;
        readonly string ctorName;
        readonly string failName;
        readonly bool fixedState;
 
        /// <summary>
        /// Provides a With function for record types
        /// </summary>
        public RWSGenerator(AttributeData attributeData)
        {
            var monoidWTypeConst = (INamedTypeSymbol)attributeData.ConstructorArguments[0].Value;
            monoidWType = monoidWTypeConst.ToString();

            wType = monoidWTypeConst.AllInterfaces.Where(interf => interf.Name == "Monoid").Take(1).Select(interf => interf.TypeArguments[0].ToString()).FirstOrDefault();

            rTypeConst = (INamedTypeSymbol)attributeData.ConstructorArguments[1].Value;
            rType = rTypeConst.ToString();

            if (attributeData.ConstructorArguments.Length == 4)
            {
                ctorName = attributeData.ConstructorArguments[2].Value?.ToString() ?? "Return";
                failName = attributeData.ConstructorArguments[3].Value?.ToString() ?? "Fail";
            }
            else
            {
                fixedState = true;

                sTypeConst = (INamedTypeSymbol)attributeData.ConstructorArguments[2].Value;
                sType = sTypeConst.ToString();

                ctorName = attributeData.ConstructorArguments[3].Value?.ToString() ?? "Return";
                failName = attributeData.ConstructorArguments[4].Value?.ToString() ?? "Fail";
            }
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
            TransformationContext context,
            IProgress<Diagnostic> progress,
            CancellationToken cancellationToken)
        {

            var results = SyntaxFactory.List<MemberDeclarationSyntax>();

            if (context.ProcessingNode is StructDeclarationSyntax applyToStruct && 
                ((fixedState == false && applyToStruct.TypeParameterList.Parameters.Count >= 2) ||
                 (fixedState == true && applyToStruct.TypeParameterList.Parameters.Count >= 1)))
            {
                var members = CodeGenUtil.MemberNames(applyToStruct.Members);

                // Apply a suffix to the name of a copy of the struct.
                var partialStruct = SyntaxFactory.StructDeclaration($"{applyToStruct.Identifier}")
                                                 .WithModifiers(applyToStruct.Modifiers)
                                                 .WithTypeParameterList(applyToStruct.TypeParameterList);

                var structName = applyToStruct.Identifier;

                var genA = applyToStruct.TypeParameterList.Parameters.Last().ToString();
                var genB = CodeGenUtil.NextGenName(genA);
                var genC = CodeGenUtil.NextGenName(genB);
                var genS = fixedState ? sType : applyToStruct.TypeParameterList.Parameters[applyToStruct.TypeParameterList.Parameters.Count - 2].ToString();
                var genW = wType;

                var structA = SyntaxFactory.ParseTypeName($"{applyToStruct.Identifier}<{applyToStruct.TypeParameterList.Parameters}>");
                var structB = CodeGenUtil.MakeGenericStruct(applyToStruct, genB);
                var structC = CodeGenUtil.MakeGenericStruct(applyToStruct, genC);
                var structR = CodeGenUtil.MakeGenericStruct(applyToStruct, rType);
                var structW = CodeGenUtil.MakeGenericStruct(applyToStruct, wType);
                var structS = CodeGenUtil.MakeGenericStruct(applyToStruct, genS);
                var structAB = CodeGenUtil.MakeGenericStruct(applyToStruct, $"({genA}, {genB})");
                var structUnit = CodeGenUtil.MakeGenericStruct(applyToStruct, "LanguageExt.Unit");
                var structPass = CodeGenUtil.MakeGenericStruct(applyToStruct, $"({genA}, System.Func<{wType}, {wType}>)");

                var noAGen = TypeParameterList(applyToStruct.TypeParameterList.Parameters.RemoveAt(applyToStruct.TypeParameterList.Parameters.Count - 1));

                var noAGenNeeded = fixedState
                    ? noAGen
                    : applyToStruct.TypeParameterList;

                var compType = SyntaxFactory.ParseTypeName($"LanguageExt.RWS<{monoidWType}, {rType}, {wType}, {genS}, {genA}>");
                var resultTypeA = SyntaxFactory.ParseTypeName($"LanguageExt.RWSResult<{monoidWType}, {rType}, {wType}, {genS}, {genA}>");
                var resultTypeR = SyntaxFactory.ParseTypeName($"LanguageExt.RWSResult<{monoidWType}, {rType}, {wType}, {genS}, {rType}>");
                var resultTypeS = SyntaxFactory.ParseTypeName($"LanguageExt.RWSResult<{monoidWType}, {rType}, {wType}, {genS}, {genS}>");
                var resultTypeUnit = SyntaxFactory.ParseTypeName($"LanguageExt.RWSResult<{monoidWType}, {rType}, {wType}, {genS}, LanguageExt.Unit>");

                var rTypeSyntax = ParseTypeName(rType);
                var wTypeSyntax = ParseTypeName(wType);
                TypeSyntax sTypeSyntax = null;
                try
                {
                    sTypeSyntax = ParseTypeName(genS);
                }
                catch (Exception e)
                {
                    CodeGenUtil.ReportError($"Type failed to parse: {genS} {e.Message}", "RWS Code-Gen", context.ProcessingNode, progress);
                    throw;
                }

                const string compName = "computation";

                // Internal field used to store the reader monad
                var compField = FieldDeclaration(
                                VariableDeclaration(compType)
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier(compName)))))
                            .WithModifiers(TokenList(new[] { Token(SyntaxKind.ReadOnlyKeyword), Token(SyntaxKind.InternalKeyword) }));

                // Constructor
                var ctor = ConstructorDeclaration(structName)
                            .WithModifiers(TokenList(new[] { Token(SyntaxKind.InternalKeyword) }))
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList<ParameterSyntax>(
                                        Parameter(Identifier("comp"))
                                        .WithType(compType))))
                            .WithExpressionBody(
                                ArrowExpressionClause(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName(compName),
                                        IdentifierName("comp"))))
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken));

                var returnMethod = MethodDeclaration(
                                        structA,
                                        Identifier(ctorName))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                        Token(SyntaxKind.PublicKeyword),
                                        Token(SyntaxKind.StaticKeyword)}))
                                            .WithParameterList(
                                                ParameterList(
                                                    SingletonSeparatedList<ParameterSyntax>(
                                                        Parameter(
                                                            Identifier("value"))
                                                        .WithType(
                                                            IdentifierName(genA)))))
                                            .WithExpressionBody(
                                                ArrowExpressionClause(
                                                    ObjectCreationExpression(structA)
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    ParenthesizedLambdaExpression(
                                                                        ParameterList(
                                                                            SeparatedList<ParameterSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Parameter(
                                                                                        Identifier("env")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Parameter(
                                                                                        Identifier("state"))})),

                                                                        InvocationExpression(
                                                                            MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                resultTypeA,
                                                                                IdentifierName("New")))
                                                                        .WithArgumentList(
                                                                            ArgumentList(
                                                                                SeparatedList<ArgumentSyntax>(
                                                                                    new SyntaxNodeOrToken[]{
                                                                                        Argument(
                                                                                            IdentifierName("state")),
                                                                                        Token(SyntaxKind.CommaToken),
                                                                                        Argument(
                                                                                            IdentifierName("value"))}))))
                                                                    ))))))
                                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                var failMethod1 = MethodDeclaration(structA, Identifier(failName))
                                      .WithModifiers(TokenList( new[]{ Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)}))
                                      .WithExpressionBody(
                                          ArrowExpressionClause(
                                              ObjectCreationExpression(structA)
                                              .WithArgumentList(
                                                  ArgumentList(
                                                      SingletonSeparatedList<ArgumentSyntax>(
                                                          Argument(
                                                              ParenthesizedLambdaExpression(
                                                                  ParameterList(
                                                                      SeparatedList<ParameterSyntax>(
                                                                          new SyntaxNodeOrToken[]{
                                                                              Parameter(
                                                                                  Identifier("env")),
                                                                              Token(SyntaxKind.CommaToken),
                                                                              Parameter(
                                                                                  Identifier("state"))})),
                                                                  InvocationExpression(
                                                                      MemberAccessExpression(
                                                                          SyntaxKind.SimpleMemberAccessExpression,
                                                                          resultTypeA,
                                                                          IdentifierName("New")))
                                                                  .WithArgumentList(
                                                                      ArgumentList(
                                                                          SeparatedList<ArgumentSyntax>(
                                                                              new SyntaxNodeOrToken[]{
                                                                                  Argument(IdentifierName("state")),
                                                                                  Token(SyntaxKind.CommaToken),
                                                                                  Argument(
                                                                                      MemberAccessExpression(
                                                                                          SyntaxKind.SimpleMemberAccessExpression,
                                                                                          MemberAccessExpression(
                                                                                              SyntaxKind.SimpleMemberAccessExpression,
                                                                                              MemberAccessExpression(
                                                                                                  SyntaxKind.SimpleMemberAccessExpression,
                                                                                                  IdentifierName("LanguageExt"),
                                                                                                  IdentifierName("Common")),
                                                                                              IdentifierName("Errors")),
                                                                                          IdentifierName("Bottom")))}))))))))))
                                                              .WithSemicolonToken(
                                                                  Token(SyntaxKind.SemicolonToken));

                var failMethod2 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("message"))
                                                    .WithType(
                                                        PredefinedType(
                                                            Token(SyntaxKind.StringKeyword))))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(structA)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                ParenthesizedLambdaExpression(
                                                                    ParameterList(
                                                                        SeparatedList<ParameterSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Parameter(
                                                                                    Identifier("env")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Parameter(
                                                                                    Identifier("state"))})),
                                                                    InvocationExpression(
                                                                      MemberAccessExpression(
                                                                          SyntaxKind.SimpleMemberAccessExpression,
                                                                          resultTypeA,
                                                                          IdentifierName("New")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("state")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        InvocationExpression(
                                                                                            MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                MemberAccessExpression(
                                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                                    MemberAccessExpression(
                                                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                                                        IdentifierName("LanguageExt"),
                                                                                                        IdentifierName("Common")),
                                                                                                    IdentifierName("Error")),
                                                                                                IdentifierName("New")))
                                                                                        .WithArgumentList(
                                                                                            ArgumentList(
                                                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                                                    Argument(
                                                                                                        IdentifierName("message"))))))}))))))))))
                                                                .WithSemicolonToken(
                                                                    Token(SyntaxKind.SemicolonToken));

                var failMethod3 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("exception"))
                                                    .WithType(CodeGenUtil.ExceptionType))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(structA)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                ParenthesizedLambdaExpression(
                                                                    ParameterList(
                                                                        SeparatedList<ParameterSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Parameter(
                                                                                    Identifier("env")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Parameter(
                                                                                    Identifier("state"))})),
                                                                    InvocationExpression(
                                                                      MemberAccessExpression(
                                                                          SyntaxKind.SimpleMemberAccessExpression,
                                                                          resultTypeA,
                                                                          IdentifierName("New")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("state")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        InvocationExpression(
                                                                                            MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                MemberAccessExpression(
                                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                                    MemberAccessExpression(
                                                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                                                        IdentifierName("LanguageExt"),
                                                                                                        IdentifierName("Common")),
                                                                                                    IdentifierName("Error")),
                                                                                                IdentifierName("New")))
                                                                                        .WithArgumentList(
                                                                                            ArgumentList(
                                                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                                                    Argument(
                                                                                                        IdentifierName("exception"))))))}))))))))))
                                                                .WithSemicolonToken(
                                                                    Token(SyntaxKind.SemicolonToken));

                var failMethod4 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("error"))
                                                    .WithType(
                                                        QualifiedName(
                                                            QualifiedName(
                                                                IdentifierName("LanguageExt"),
                                                                IdentifierName("Common")),
                                                            IdentifierName("Error"))))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(structA)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                ParenthesizedLambdaExpression(
                                                                    ParameterList(
                                                                        SeparatedList<ParameterSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Parameter(
                                                                                    Identifier("env")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Parameter(
                                                                                    Identifier("state"))})),
                                                                    InvocationExpression(
                                                                      MemberAccessExpression(
                                                                          SyntaxKind.SimpleMemberAccessExpression,
                                                                          resultTypeA,
                                                                          IdentifierName("New")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("state")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        IdentifierName("error"))}))))))))))
                                                                .WithSemicolonToken(
                                                                    Token(SyntaxKind.SemicolonToken));

                var failMethod5 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithParameterList(
                                            ParameterList(
                                                SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("message"))
                                                        .WithType(
                                                            PredefinedType(
                                                                Token(SyntaxKind.StringKeyword))),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("exception"))
                                                        .WithType(CodeGenUtil.ExceptionType)})))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(structA)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                ParenthesizedLambdaExpression(
                                                                    ParameterList(
                                                                        SeparatedList<ParameterSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Parameter(
                                                                                    Identifier("env")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Parameter(
                                                                                    Identifier("state"))})),
                                                                    InvocationExpression(
                                                                      MemberAccessExpression(
                                                                          SyntaxKind.SimpleMemberAccessExpression,
                                                                          resultTypeA,
                                                                          IdentifierName("New")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("state")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        InvocationExpression(
                                                                                            MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                MemberAccessExpression(
                                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                                    MemberAccessExpression(
                                                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                                                        IdentifierName("LanguageExt"),
                                                                                                        IdentifierName("Common")),
                                                                                                    IdentifierName("Error")),
                                                                                                IdentifierName("New")))
                                                                                        .WithArgumentList(
                                                                                            ArgumentList(
                                                                                                SeparatedList<ArgumentSyntax>(
                                                                                                    new SyntaxNodeOrToken[]{
                                                                                                        Argument(
                                                                                                            IdentifierName("message")),
                                                                                                        Token(SyntaxKind.CommaToken),
                                                                                                        Argument(
                                                                                                            IdentifierName("exception"))}))))}))))))))))
                                                                .WithSemicolonToken(
                                                                    Token(SyntaxKind.SemicolonToken));

                var selectMethod = MethodDeclaration(
                                            structB,
                                            Identifier("Select"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithTypeParameterList(
                                            TypeParameterList(
                                                SingletonSeparatedList<TypeParameterSyntax>(
                                                    TypeParameter(
                                                        Identifier(genB)))))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(genA, genB)
                                                        ))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                    IdentifierName("Map"))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("f")))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                var mapMethod = MethodDeclaration(
                                        structB,
                                        Identifier("Map"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithTypeParameterList(
                                        TypeParameterList(
                                            SingletonSeparatedList<TypeParameterSyntax>(
                                                TypeParameter(
                                                    Identifier(genB)))))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("f"))
                                                .WithType(
                                                    CodeGenUtil.FuncType(genA, genB)))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                IdentifierName("Bind"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            SimpleLambdaExpression(
                                                                Parameter(
                                                                    Identifier("a")),
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        structB,
                                                                        IdentifierName(this.ctorName)))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                            Argument(
                                                                                InvocationExpression(
                                                                                    IdentifierName("f"))
                                                                                .WithArgumentList(
                                                                                    ArgumentList(
                                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                                            Argument(
                                                                                                IdentifierName("a"))))))))))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var bindMethod =
                    MethodDeclaration(
                            structB,
                            Identifier("Bind"))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))
                        .WithTypeParameterList(
                            TypeParameterList(
                                SingletonSeparatedList<TypeParameterSyntax>(
                                    TypeParameter(
                                        Identifier(genB)))))
                        .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList<ParameterSyntax>(
                                    Parameter(
                                        Identifier("f"))
                                    .WithType(CodeGenUtil.FuncType(genA, structB)))))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                ObjectCreationExpression(structB)
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName(compName),
                                                        IdentifierName("Bind")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                SimpleLambdaExpression(
                                                                    Parameter(
                                                                        Identifier("a")),
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        InvocationExpression(
                                                                            IdentifierName("f"))
                                                                        .WithArgumentList(
                                                                            ArgumentList(
                                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                                    Argument(
                                                                                        IdentifierName("a"))))),
                                                                        IdentifierName(compName)))))))))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));

                var selectManyMethod1 =
                    MethodDeclaration(
                            structB,
                            Identifier("SelectMany"))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))
                        .WithTypeParameterList(
                            TypeParameterList(
                                SingletonSeparatedList<TypeParameterSyntax>(
                                    TypeParameter(
                                        Identifier(genB)))))
                        .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList<ParameterSyntax>(
                                    Parameter(
                                        Identifier("f"))
                                    .WithType(CodeGenUtil.FuncType(genA, structB)))))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                InvocationExpression(
                                    IdentifierName("Bind"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                IdentifierName("f")))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));

                var selectManyMethod2 = MethodDeclaration(
                                            structC,
                                            Identifier("SelectMany"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithTypeParameterList(
                                            TypeParameterList(
                                                SeparatedList<TypeParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        TypeParameter(
                                                            Identifier(genB)),
                                                        Token(SyntaxKind.CommaToken),
                                                        TypeParameter(
                                                            Identifier(genC))})))
                                        .WithParameterList(
                                            ParameterList(
                                                SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("bind"))
                                                        .WithType(CodeGenUtil.FuncType(genA, structB)),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("project"))
                                                        .WithType(
                                                            CodeGenUtil.FuncType(genA, genB, genC))})))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                    IdentifierName("Bind"))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                SimpleLambdaExpression(
                                                                    Parameter(
                                                                        Identifier("a")),
                                                                    InvocationExpression(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            InvocationExpression(
                                                                                IdentifierName("bind"))
                                                                            .WithArgumentList(
                                                                                ArgumentList(
                                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                                        Argument(
                                                                                            IdentifierName("a"))))),
                                                                            IdentifierName("Map")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                                Argument(
                                                                                    SimpleLambdaExpression(
                                                                                        Parameter(
                                                                                            Identifier("b")),
                                                                                        InvocationExpression(
                                                                                            IdentifierName("project"))
                                                                                        .WithArgumentList(
                                                                                            ArgumentList(
                                                                                                SeparatedList<ArgumentSyntax>(
                                                                                                    new SyntaxNodeOrToken[]{
                                                                                                        Argument(
                                                                                                            IdentifierName("a")),
                                                                                                        Token(SyntaxKind.CommaToken),
                                                                                                        Argument(
                                                                                                            IdentifierName("b"))})))))))))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                var runMethod = MethodDeclaration(
                                        resultTypeA,
                                        Identifier("Run"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Parameter(
                                                        Identifier("env"))
                                                    .WithType(
                                                        ParseTypeName(rType)),
                                                    Token(SyntaxKind.CommaToken),
                                                    Parameter(
                                                        Identifier("state"))
                                                    .WithType(
                                                        IdentifierName(genS))})))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName(compName),
                                                    IdentifierName("Run")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SeparatedList<ArgumentSyntax>(
                                                        new SyntaxNodeOrToken[]{
                                                            Argument(
                                                                IdentifierName("env")),
                                                            Token(SyntaxKind.CommaToken),
                                                            Argument(
                                                                IdentifierName("state"))})))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var filterMethod =
                    MethodDeclaration(
                            structA,
                            Identifier("Filter"))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))
                        .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList<ParameterSyntax>(
                                    Parameter(
                                        Identifier("f"))
                                    .WithType(
                                        CodeGenUtil.FuncType(genA, PredefinedType(Token(SyntaxKind.BoolKeyword)))))))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                ObjectCreationExpression(structA)
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName(compName),
                                                        IdentifierName("Where")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("f")))))))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));

                var whereMethod =
                    MethodDeclaration(
                            structA,
                            Identifier("Where"))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))
                        .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList<ParameterSyntax>(
                                    Parameter(
                                        Identifier("f"))
                                    .WithType(
                                        CodeGenUtil.FuncType(genA, PredefinedType(Token(SyntaxKind.BoolKeyword)))))))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                InvocationExpression(
                                    IdentifierName("Filter"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                IdentifierName("f")))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));

                var doMethod = MethodDeclaration(structA, Identifier("Do"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("f"))
                                                .WithType(
                                                    CodeGenUtil.ActionType(genA)
                                                ))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structA)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName(compName),
                                                                    IdentifierName("Do")))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                        Argument(
                                                                            IdentifierName("f")))))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var strictMethod = MethodDeclaration(structA, Identifier("Strict"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structA)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName(compName),
                                                                    IdentifierName("Strict")))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var toSeqMethod = MethodDeclaration(
                                        GenericName(
                                            Identifier("Seq"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList<TypeSyntax>(
                                                    IdentifierName(genA)))),
                                        Identifier("ToSeq"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("env"))
                                                        .WithType(ParseTypeName(rType)),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("state"))
                                                        .WithType(
                                                            IdentifierName(genS))})))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName(compName),
                                                    IdentifierName("ToSeq")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SeparatedList<ArgumentSyntax>(
                                                        new SyntaxNodeOrToken[]{
                                                                Argument(
                                                                    IdentifierName("env")),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    IdentifierName("state"))})))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var iterMethod = MethodDeclaration(structUnit, Identifier("Iter"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("f"))
                                                .WithType(
                                                    CodeGenUtil.ActionType(genA)
                                                ))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structUnit)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName(compName),
                                                                    IdentifierName("Iter")))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                        Argument(
                                                                            IdentifierName("f")))))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var foldMethod = MethodDeclaration(
                                        CodeGenUtil.FuncType(rTypeSyntax, IdentifierName(genS), IdentifierName("State")),
                                        Identifier("Fold"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithTypeParameterList(
                                        TypeParameterList(
                                            SingletonSeparatedList<TypeParameterSyntax>(
                                                TypeParameter(
                                                    Identifier("State")))))
                                    .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("state"))
                                                        .WithType(
                                                            IdentifierName("State")),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("f"))
                                                        .WithType(
                                                            CodeGenUtil.FuncType("State", genA, "State"))
                                                        })))
                                    .WithBody(
                                        Block(
                                            LocalDeclarationStatement(
                                                VariableDeclaration(
                                                    IdentifierName("var"))
                                                .WithVariables(
                                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                        VariableDeclarator(
                                                            Identifier("self"))
                                                        .WithInitializer(
                                                            EqualsValueClause(
                                                                ThisExpression()))))),
                                            ReturnStatement(
                                                ParenthesizedLambdaExpression(
                                                    ParameterList(
                                                        SeparatedList<ParameterSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                    Parameter(
                                                                        Identifier("env")),
                                                                    Token(SyntaxKind.CommaToken),
                                                                    Parameter(
                                                                        Identifier("s"))})),
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        InvocationExpression(
                                                                            MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                MemberAccessExpression(
                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                    IdentifierName("self"),
                                                                                    IdentifierName(compName)),
                                                                                IdentifierName("Fold")))
                                                                        .WithArgumentList(
                                                                            ArgumentList(
                                                                                SeparatedList<ArgumentSyntax>(
                                                                                    new SyntaxNodeOrToken[]{
                                                                                            Argument(
                                                                                                IdentifierName("state")),
                                                                                            Token(SyntaxKind.CommaToken),
                                                                                            Argument(
                                                                                                IdentifierName("f"))}))),
                                                                        IdentifierName("Run")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("env")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        IdentifierName("s"))}))),
                                                            IdentifierName("IfFail")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    IdentifierName("state")))))))));

                var forAllMethod = MethodDeclaration(
                                            CodeGenUtil.FuncType(rTypeSyntax, IdentifierName(genS), PredefinedType(Token(SyntaxKind.BoolKeyword))),
                                            Identifier("ForAll"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(genA, PredefinedType(Token(SyntaxKind.BoolKeyword)))
                                                    ))))
                                        .WithBody(
                                            Block(
                                                LocalDeclarationStatement(
                                                    VariableDeclaration(
                                                        IdentifierName("var"))
                                                    .WithVariables(
                                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                            VariableDeclarator(
                                                                Identifier("self"))
                                                            .WithInitializer(
                                                                EqualsValueClause(
                                                                    ThisExpression()))))),
                                                ReturnStatement(
                                                    ParenthesizedLambdaExpression(
                                                        ParameterList(
                                                            SeparatedList<ParameterSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                        Parameter(
                                                                            Identifier("env")),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        Parameter(
                                                                            Identifier("s"))})),
                                                        InvocationExpression(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        InvocationExpression(
                                                                            MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                MemberAccessExpression(
                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                    IdentifierName("self"),
                                                                                    IdentifierName(compName)),
                                                                                IdentifierName("ForAll")))
                                                                        .WithArgumentList(
                                                                            ArgumentList(
                                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                                    Argument(
                                                                                        IdentifierName("f"))))),
                                                                        IdentifierName("Run")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("env")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        IdentifierName("s"))}))),
                                                            IdentifierName("IfFail")))
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.FalseLiteralExpression)))))))));

                var existsMethod = MethodDeclaration(
                                            CodeGenUtil.FuncType(rTypeSyntax, IdentifierName(genS), PredefinedType(Token(SyntaxKind.BoolKeyword))),
                                            Identifier("Exists"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(genA, PredefinedType(Token(SyntaxKind.BoolKeyword)))
                                                    ))))
                                        .WithBody(
                                            Block(
                                                LocalDeclarationStatement(
                                                    VariableDeclaration(
                                                        IdentifierName("var"))
                                                    .WithVariables(
                                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                            VariableDeclarator(
                                                                Identifier("self"))
                                                            .WithInitializer(
                                                                EqualsValueClause(
                                                                    ThisExpression()))))),
                                                ReturnStatement(
                                                    ParenthesizedLambdaExpression(
                                                        ParameterList(
                                                            SeparatedList<ParameterSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                        Parameter(
                                                                            Identifier("env")),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        Parameter(
                                                                            Identifier("s"))})),
                                                        InvocationExpression(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        InvocationExpression(
                                                                            MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                MemberAccessExpression(
                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                    IdentifierName("self"),
                                                                                    IdentifierName(compName)),
                                                                                IdentifierName("Exists")))
                                                                        .WithArgumentList(
                                                                            ArgumentList(
                                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                                    Argument(
                                                                                        IdentifierName("f"))))),
                                                                        IdentifierName("Run")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("env")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        IdentifierName("s"))}))),
                                                                IdentifierName("IfFail")))
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.FalseLiteralExpression)))))))));

                var localMethod = MethodDeclaration(
                                        structA,
                                        Identifier("Local"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("f"))
                                                .WithType(
                                                    CodeGenUtil.FuncType(rType, rType)
                                                 ))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structA)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            InvocationExpression(
                                                                CodeGenUtil.PreludeMember(
                                                                    GenericName(
                                                                        Identifier("local"))
                                                                    .WithTypeArgumentList(
                                                                        TypeArgumentList(
                                                                            SeparatedList<TypeSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    SyntaxFactory.ParseTypeName(monoidWType),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    SyntaxFactory.ParseTypeName(rType),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    SyntaxFactory.ParseTypeName(wType),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    IdentifierName(genS),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    IdentifierName(genA)})))))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            Argument(
                                                                                IdentifierName(compName)),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            Argument(
                                                                                IdentifierName("f"))})))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var listenMethod = MethodDeclaration(
                                            structAB,
                                            Identifier("Listen"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithTypeParameterList(
                                            TypeParameterList(
                                                SingletonSeparatedList<TypeParameterSyntax>(
                                                    TypeParameter(
                                                        Identifier(genB)))))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(wType, genB)
                                                    ))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(structAB)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName(compName),
                                                                        IdentifierName("Listen")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                            Argument(
                                                                                IdentifierName("f")))))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));


                var censorMethod = MethodDeclaration(structA, Identifier("Censor"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(wType, wType)
                                                     ))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(structA)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName(compName),
                                                                        IdentifierName("Censor")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                            Argument(
                                                                                IdentifierName("f")))))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                var memberDecls = new MemberDeclarationSyntax[]{
                            compField,
                            ctor,
                            CodeGenUtil.NullIfExists(members, returnMethod),
                            CodeGenUtil.NullIfExists(members, failMethod1),
                            CodeGenUtil.NullIfExists(members, failMethod2),
                            CodeGenUtil.NullIfExists(members, failMethod3),
                            CodeGenUtil.NullIfExists(members, failMethod4),
                            CodeGenUtil.NullIfExists(members, failMethod5),
                            CodeGenUtil.NullIfExists(members, mapMethod),
                            CodeGenUtil.NullIfExists(members, selectMethod),
                            CodeGenUtil.NullIfExists(members, bindMethod),
                            CodeGenUtil.NullIfExists(members, selectManyMethod1),
                            CodeGenUtil.NullIfExists(members, selectManyMethod2),
                            CodeGenUtil.NullIfExists(members, runMethod),
                            CodeGenUtil.NullIfExists(members, filterMethod),
                            CodeGenUtil.NullIfExists(members, whereMethod),
                            CodeGenUtil.NullIfExists(members, doMethod),
                            CodeGenUtil.NullIfExists(members, strictMethod),
                            CodeGenUtil.NullIfExists(members, toSeqMethod),
                            CodeGenUtil.NullIfExists(members, iterMethod),
                            CodeGenUtil.NullIfExists(members, foldMethod),
                            CodeGenUtil.NullIfExists(members, forAllMethod),
                            CodeGenUtil.NullIfExists(members, existsMethod),
                            CodeGenUtil.NullIfExists(members, localMethod),
                            CodeGenUtil.NullIfExists(members, listenMethod),
                            CodeGenUtil.NullIfExists(members, censorMethod)
                        };

                memberDecls = memberDecls.Where(m => m != null).ToArray();
                partialStruct = partialStruct.WithMembers(List(memberDecls));

                var returnFunc = MethodDeclaration(
                                        structA,
                                        Identifier(ctorName))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                    .WithTypeParameterList(applyToStruct.TypeParameterList)
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("value"))
                                                .WithType(
                                                    IdentifierName(genA)))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    structA,
                                                    IdentifierName(ctorName)))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            IdentifierName("value")))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var failFunc1 = MethodDeclaration(
                                        structA,
                                        Identifier(failName))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                    .WithTypeParameterList(applyToStruct.TypeParameterList)
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    structA,
                                                    IdentifierName(failName)))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var failFunc2 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithTypeParameterList(applyToStruct.TypeParameterList)
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("message"))
                                                    .WithType(
                                                        PredefinedType(
                                                            Token(SyntaxKind.StringKeyword))))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        structA,
                                                        IdentifierName(failName)))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("message")))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                var failFunc3 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithTypeParameterList(applyToStruct.TypeParameterList)
                                        .WithParameterList(
                                            ParameterList(
                                                SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("message"))
                                                        .WithType(
                                                            PredefinedType(
                                                                Token(SyntaxKind.StringKeyword))),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("exception"))
                                                        .WithType(CodeGenUtil.ExceptionType)})))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        structA,
                                                        IdentifierName(failName)))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                Argument(
                                                                    IdentifierName("message")),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    IdentifierName("exception"))})))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                var failFunc4 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                      .WithModifiers(
                                          TokenList(
                                              new[]{
                                                  Token(SyntaxKind.PublicKeyword),
                                                  Token(SyntaxKind.StaticKeyword)}))
                                      .WithTypeParameterList(applyToStruct.TypeParameterList)
                                      .WithParameterList(
                                          ParameterList(
                                              SingletonSeparatedList<ParameterSyntax>(
                                                  Parameter(
                                                      Identifier("exception"))
                                                  .WithType(CodeGenUtil.ExceptionType))))
                                      .WithExpressionBody(
                                          ArrowExpressionClause(
                                              InvocationExpression(
                                                  MemberAccessExpression(
                                                      SyntaxKind.SimpleMemberAccessExpression,
                                                      structA,
                                                      IdentifierName(failName)))
                                              .WithArgumentList(
                                                  ArgumentList(
                                                      SingletonSeparatedList<ArgumentSyntax>(
                                                          Argument(
                                                              IdentifierName("exception")))))))
                                      .WithSemicolonToken(
                                          Token(SyntaxKind.SemicolonToken));

                var failFunc5 = MethodDeclaration(
                                    structA,
                                    Identifier(failName))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithTypeParameterList(applyToStruct.TypeParameterList)
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("error"))
                                                    .WithType(
                                                        QualifiedName(
                                                            QualifiedName(
                                                                IdentifierName("LanguageExt"),
                                                                IdentifierName("Common")),
                                                            IdentifierName("Error"))))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        structA,
                                                        IdentifierName(failName)))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("error")))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                var asksFunc = MethodDeclaration(
                                    structA,
                                    Identifier("asks"))
                                .WithModifiers(
                                    TokenList(
                                        new []{
                                            Token(SyntaxKind.PublicKeyword),
                                            Token(SyntaxKind.StaticKeyword)}))
                                .WithTypeParameterList(applyToStruct.TypeParameterList)
                                .WithParameterList(
                                    ParameterList(
                                        SingletonSeparatedList<ParameterSyntax>(
                                            Parameter(
                                                Identifier("f"))
                                            .WithType(
                                                CodeGenUtil.FuncType(rType, genA)
                                            ))))
                                .WithExpressionBody(
                                    ArrowExpressionClause(
                                        ObjectCreationExpression(structA)
                                        .WithArgumentList(
                                            ArgumentList(
                                                SingletonSeparatedList<ArgumentSyntax>(
                                                    Argument(
                                                        ParenthesizedLambdaExpression(
                                                            ParameterList(
                                                                SeparatedList<ParameterSyntax>(
                                                                    new SyntaxNodeOrToken[]{
                                                                        Parameter(
                                                                            Identifier("env")),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        Parameter(
                                                                            Identifier("state"))})),
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    resultTypeA,
                                                                    IdentifierName("New")))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            Argument(
                                                                                IdentifierName("state")),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            Argument(
                                                                                InvocationExpression(
                                                                                    IdentifierName("f"))
                                                                                .WithArgumentList(
                                                                                    ArgumentList(
                                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                                            Argument(
                                                                                                IdentifierName("env"))))))}))))))))))
                                .WithSemicolonToken(
                                    Token(SyntaxKind.SemicolonToken));

                var askFuncBody = ArrowExpressionClause(
                    ObjectCreationExpression(structR)
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(
                                Argument(
                                    ParenthesizedLambdaExpression(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Parameter(
                                                        Identifier("env")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Parameter(
                                                        Identifier("state"))})),
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                resultTypeR,
                                                IdentifierName("New")))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Argument(
                                                            IdentifierName("state")),
                                                        Token(SyntaxKind.CommaToken),
                                                        Argument(
                                                            IdentifierName("env"))})))))))));

                var askFunc = applyToStruct.TypeParameterList.Parameters.Count == 1
                              ? PropertyDeclaration(structR, Identifier("ask"))
                                    .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                                    .WithExpressionBody(askFuncBody)
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)) as MemberDeclarationSyntax
                              : MethodDeclaration(
                                        structR,
                                        Identifier("ask"))
                                  .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                  .WithExpressionBody(askFuncBody)
                                  .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                if (noAGen.Parameters.Count != 0 && askFunc is MethodDeclarationSyntax askFuncMethod)
                {
                    askFunc = askFuncMethod.WithTypeParameterList(noAGen);
                }

                var getFuncBody = ArrowExpressionClause(
                                    ObjectCreationExpression(structS)
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    ParenthesizedLambdaExpression(
                                                        ParameterList(
                                                            SeparatedList<ParameterSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                    Parameter(
                                                                        Identifier("env")),
                                                                    Token(SyntaxKind.CommaToken),
                                                                    Parameter(
                                                                        Identifier("state"))})),
                                                        InvocationExpression(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                resultTypeS,
                                                                IdentifierName("New")))
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SeparatedList<ArgumentSyntax>(
                                                                    new SyntaxNodeOrToken[]{
                                                                        Argument(
                                                                            IdentifierName("state")),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        Argument(
                                                                            IdentifierName("state"))})))))))));

                var getFunc = applyToStruct.TypeParameterList.Parameters.Count == 1
                              ? PropertyDeclaration(structS, Identifier("get"))
                                    .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                                    .WithExpressionBody(getFuncBody)
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)) as MemberDeclarationSyntax
                              : MethodDeclaration(
                                        structS,
                                        Identifier("get"))
                                  .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                                  .WithExpressionBody(getFuncBody)
                                  .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                if (noAGen.Parameters.Count != 0 && getFunc is MethodDeclarationSyntax getFuncMethod)
                {
                    getFunc = getFuncMethod.WithTypeParameterList(noAGen);
                }

                var getsFunc = MethodDeclaration(
                                            structA,
                                            Identifier("gets"))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                    .WithTypeParameterList(applyToStruct.TypeParameterList)
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(genS, genA)
                                                    ))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(structA)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                ParenthesizedLambdaExpression(
                                                                    ParameterList(
                                                                        SeparatedList<ParameterSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Parameter(
                                                                                    Identifier("env")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Parameter(
                                                                                    Identifier("state"))})),
                                                                    InvocationExpression(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            resultTypeA,
                                                                            IdentifierName("New")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("state")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        InvocationExpression(
                                                                                            IdentifierName("f"))
                                                                                        .WithArgumentList(
                                                                                            ArgumentList(
                                                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                                                    Argument(
                                                                                                        IdentifierName("state"))))))}))))))))))
                                                                .WithSemicolonToken(
                                                                    Token(SyntaxKind.SemicolonToken));

                var putFunc = MethodDeclaration(
                                            structUnit,
                                            Identifier("put"))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("value"))
                                                    .WithType(
                                                        IdentifierName(genS)))))
                                        .WithExpressionBody(
                                             ArrowExpressionClause(
                                                ObjectCreationExpression(structUnit)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                ParenthesizedLambdaExpression(
                                                                    ParameterList(
                                                                        SeparatedList<ParameterSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Parameter(
                                                                                    Identifier("env")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Parameter(
                                                                                    Identifier("state"))})),
                                                                    InvocationExpression(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            resultTypeUnit,
                                                                            IdentifierName("New")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("value")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(DefaultExpression(CodeGenUtil.UnitType))}))))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                if (noAGen.Parameters.Count != 0)
                {
                    putFunc = putFunc.WithTypeParameterList(noAGen);
                }

                var modifyFunc = MethodDeclaration(
                                        structUnit,
                                        Identifier("modify"))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("f"))
                                                .WithType(
                                                    CodeGenUtil.FuncType(genS, genS)
                                                ))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structUnit)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            ParenthesizedLambdaExpression(
                                                                ParameterList(
                                                                    SeparatedList<ParameterSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            Parameter(
                                                                                Identifier("env")),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            Parameter(
                                                                                Identifier("state"))})),
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        resultTypeUnit,
                                                                        IdentifierName("New")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Argument(
                                                                                    InvocationExpression(
                                                                                        IdentifierName("f"))
                                                                                    .WithArgumentList(
                                                                                        ArgumentList(
                                                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                                                Argument(
                                                                                                    IdentifierName("state")))))),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Argument(
                                                                                    DefaultExpression(CodeGenUtil.UnitType))}))))))))))
                                                            .WithSemicolonToken(
                                                                    Token(SyntaxKind.SemicolonToken));

                if (noAGen.Parameters.Count != 0)
                {
                    modifyFunc = modifyFunc.WithTypeParameterList(noAGen);
                }

                var localFunc = MethodDeclaration(
                                        structA,
                                        Identifier("local"))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                    .WithTypeParameterList(applyToStruct.TypeParameterList)
                                    .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Parameter(Identifier("ma"))
                                                    .WithType(structA),
                                                    Token(SyntaxKind.CommaToken),
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(rType, rType)
                                                    )})))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("ma"),
                                                    IdentifierName("Local")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            IdentifierName("f")))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var PassFunc = MethodDeclaration(
                                            structA,
                                            Identifier("Pass"))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithTypeParameterList(applyToStruct.TypeParameterList)
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("ma"))
                                                    .WithModifiers(
                                                        TokenList(
                                                            Token(SyntaxKind.ThisKeyword)))
                                                    .WithType(structPass))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                ObjectCreationExpression(
                                                    structA)
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            IdentifierName("ma"),
                                                                            IdentifierName(compName)),
                                                                        IdentifierName("Pass")))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));


                var passFunc = MethodDeclaration(
                                        structA,
                                        Identifier("pass"))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                    .WithTypeParameterList(applyToStruct.TypeParameterList)
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("ma"))
                                                .WithType(structPass))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structA)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("ma"),
                                                                        IdentifierName(compName)),
                                                                    IdentifierName("Pass")))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var listenFunc = MethodDeclaration(
                                        structAB,
                                        Identifier("listen"))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                   .WithTypeParameterList(applyToStruct.TypeParameterList.AddParameters(TypeParameter(genB)))
                                   .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Parameter(
                                                        Identifier("ma"))
                                                    .WithType(structA),
                                                    Token(SyntaxKind.CommaToken),
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        CodeGenUtil.FuncType(wType, genB)
                                                    )})))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("ma"),
                                                    IdentifierName("Listen")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            IdentifierName("f")))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var censorFunc = MethodDeclaration(
                                            structA,
                                            Identifier("censor"))
                                        .WithModifiers(
                                            TokenList(
                                                new[]{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithTypeParameterList(applyToStruct.TypeParameterList)
                                        .WithParameterList(
                                            ParameterList(
                                                SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("ma"))
                                                        .WithType(structA),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("f"))
                                                        .WithType(
                                                            CodeGenUtil.FuncType(wType, wType)
                                                         )})))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("ma"),
                                                        IdentifierName("Censor")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("f")))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                var tellFunc = MethodDeclaration(
                                        structUnit,
                                        Identifier("tell"))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("what"))
                                                .WithType(ParseTypeName(wType)))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structUnit)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            InvocationExpression(
                                                                CodeGenUtil.PreludeMember(
                                                                    GenericName(
                                                                        Identifier("tell"))
                                                                    .WithTypeArgumentList(
                                                                        TypeArgumentList(
                                                                            SeparatedList<TypeSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    ParseTypeName(monoidWType),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    ParseTypeName(rType),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    ParseTypeName(wType),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    IdentifierName(genS),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    CodeGenUtil.UnitType})))))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                        Argument(
                                                                            IdentifierName("what")))))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                if (noAGen.Parameters.Count != 0)
                {
                    tellFunc = tellFunc.WithTypeParameterList(noAGen);
                }

                var prelude = ClassDeclaration(structName)
                        .WithModifiers(
                            TokenList(
                                new[]{
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword),
                                    Token(SyntaxKind.PartialKeyword)}))
                        .WithMembers(
                            List(
                                new MemberDeclarationSyntax[]{
                                    returnFunc,
                                    failFunc1,
                                    failFunc2,
                                    failFunc3,
                                    failFunc4,
                                    failFunc5,
                                    asksFunc,
                                    askFunc,
                                    getFunc,
                                    getsFunc,
                                    putFunc,
                                    modifyFunc,
                                    localFunc,
                                    PassFunc,
                                    passFunc,
                                    listenFunc,
                                    censorFunc,
                                    tellFunc
                                }));

                prelude = CodeGenUtil.AddMembersToPrelude(prelude, applyToStruct, "ask", (ITypeSymbol)rTypeConst);
                prelude = CodeGenUtil.AddMembersToPrelude(prelude, applyToStruct, "get", (ITypeSymbol)sTypeConst);

                return Task.FromResult(results.Add(partialStruct).Add(prelude));
            }
            else
            {
                if (fixedState)
                {
                    CodeGenUtil.ReportError($"Type can't be made into a RWS monad.  It must be a partial struct with one or more generic parameters.", "RWS Code-Gen", context.ProcessingNode, progress);
                }
                else
                {
                    CodeGenUtil.ReportError($"Type can't be made into a RWS monad.  It must be a partial struct with two or more generic parameters, the first generic parameter being the state, the second being the bound value type.", "RWS Code-Gen", context.ProcessingNode, progress);
                }

                return Task.FromResult(results);
            }
        }
    }
}
