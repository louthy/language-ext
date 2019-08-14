using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.CodeGen
{
    /// <summary>
    /// Reader Generator
    /// </summary>
    public class ReaderGenerator : ICodeGenerator
    {
        readonly string envType;
        readonly object envTypeConst;
        readonly string ctorName;
        readonly string failName;

        /// <summary>
        /// Provides a With function for record types
        /// </summary>
        public ReaderGenerator(AttributeData attributeData)
        {
            envTypeConst = attributeData.ConstructorArguments[0].Value;
            envType = attributeData.ConstructorArguments[0].Value.ToString();
            ctorName = attributeData.ConstructorArguments[1].Value?.ToString() ?? "Return";
            failName = attributeData.ConstructorArguments[2].Value?.ToString() ?? "Fail";
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
            TransformationContext context, 
            IProgress<Diagnostic> progress, 
            CancellationToken cancellationToken)
        {
            var results = SyntaxFactory.List<MemberDeclarationSyntax>();

            if (context.ProcessingNode is StructDeclarationSyntax applyToStruct && applyToStruct.TypeParameterList.Parameters.Count >= 1)
            {
                // Apply a suffix to the name of a copy of the struct.
                var partialStruct = StructDeclaration($"{applyToStruct.Identifier}")
                                        .WithModifiers(applyToStruct.Modifiers)
                                        .WithTypeParameterList(applyToStruct.TypeParameterList);

                var structName = applyToStruct.Identifier;
                var genA = applyToStruct.TypeParameterList.Parameters.Last().ToString();
                var genB = CodeGenUtil.NextGenName(genA);
                var genC = CodeGenUtil.NextGenName(genB);
                var structA = ParseTypeName($"{applyToStruct.Identifier}<{applyToStruct.TypeParameterList.Parameters}>");
                var structB = CodeGenUtil.MakeGenericStruct(applyToStruct, genB);
                var structC = CodeGenUtil.MakeGenericStruct(applyToStruct, genC);
                var structEnv = CodeGenUtil.MakeGenericStruct(applyToStruct, envType);
                var structUnit = CodeGenUtil.MakeGenericStruct(applyToStruct, "LanguageExt.Unit");

                var compType = ParseTypeName($"LanguageExt.Reader<{envType}, {applyToStruct.TypeParameterList.Parameters.Last()}>");

                // Internal field used to store the reader monad
                var compField = FieldDeclaration(
                                VariableDeclaration(compType)
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier("__comp")))))
                            .WithModifiers(TokenList(new[] { Token(SyntaxKind.ReadOnlyKeyword) }));

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
                                        IdentifierName("__comp"),
                                        IdentifierName("comp"))))
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken));

                // Return :: A -> MA
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
                                                            SimpleLambdaExpression(
                                                                Parameter(
                                                                    Identifier("env")),
                                                                TupleExpression(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            Argument(
                                                                                IdentifierName("value")),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            Argument(
                                                                                LiteralExpression(
                                                                                    SyntaxKind.FalseLiteralExpression))})))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                // Fail :: Unit -> MA
                var failMethod = PropertyDeclaration(structA, Identifier(failName))
                                    .WithModifiers(
                                        TokenList(
                                            new[]{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.StaticKeyword)}))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structA)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            SimpleLambdaExpression(
                                                                Parameter(
                                                                    Identifier("env")),
                                                                TupleExpression(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            Argument(
                                                                                LiteralExpression(
                                                                                    SyntaxKind.DefaultLiteralExpression,
                                                                                    Token(SyntaxKind.DefaultKeyword))),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            Argument(
                                                                                LiteralExpression(
                                                                                    SyntaxKind.TrueLiteralExpression))})))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                // SelectMany :: MA -> (A -> MB) -> (A -> B -> C) -> MC
                var selectManyMethod = 
                    MethodDeclaration(structC, Identifier("SelectMany"))
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
                                            .WithType(
                                                GenericName(
                                                    Identifier("Func"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SeparatedList<TypeSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                IdentifierName(genA),
                                                                Token(SyntaxKind.CommaToken),
                                                                structB})))),
                                            Token(SyntaxKind.CommaToken),
                                            Parameter(
                                                Identifier("project"))
                                            .WithType(
                                                GenericName(
                                                    Identifier("Func"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SeparatedList<TypeSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                IdentifierName(genA),
                                                                Token(SyntaxKind.CommaToken),
                                                                IdentifierName(genB),
                                                                Token(SyntaxKind.CommaToken),
                                                                IdentifierName(genC)}))))})))
                            .WithExpressionBody(
                                ArrowExpressionClause(
                                    ObjectCreationExpression(structC)
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("__comp"),
                                                            IdentifierName("Bind")))
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
                                                                                MemberAccessExpression(
                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                    InvocationExpression(
                                                                                        IdentifierName("bind"))
                                                                                    .WithArgumentList(
                                                                                        ArgumentList(
                                                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                                                Argument(
                                                                                                    IdentifierName("a"))))),
                                                                                    IdentifierName("__comp")),
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
                                                                                                                IdentifierName("b"))})))))))))))))))))))
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken));

                // Run :: MA -> Env -> TryOption A
                var runMethod = MethodDeclaration(
                                    GenericName(
                                        Identifier("TryOption"))
                                    .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList<TypeSyntax>(
                                                    IdentifierName(genA)))),
                                            Identifier("Run"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("env"))
                                                    .WithType(
                                                        IdentifierName(envType)))))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("__comp"),
                                                        IdentifierName("Run")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("env")))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                // Do :: MA -> (A -> Unit) -> MA
                var doMethod = MethodDeclaration(
                                    structA,
                                    Identifier("Do"))
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PublicKeyword)))
                                .WithParameterList(
                                    ParameterList(
                                        SingletonSeparatedList<ParameterSyntax>(
                                            Parameter(
                                                Identifier("f"))
                                            .WithType(
                                                GenericName(
                                                    Identifier("Action"))
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            IdentifierName(genA))))))))
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
                                                                IdentifierName("__comp"),
                                                                IdentifierName("Do")))
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                    Argument(
                                                                        IdentifierName("f")))))))))))
                                .WithSemicolonToken(
                                    Token(SyntaxKind.SemicolonToken));

                // Strict :: MA -> MA
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
                                                                        IdentifierName("__comp"),
                                                                        IdentifierName("Strict")))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));


                // ToSeq :: MA -> Seq A
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
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("env"))
                                                .WithType(
                                                    IdentifierName(envType)))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("__comp"),
                                                    IdentifierName("ToSeq")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            IdentifierName("env")))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                // Iter :: MA -> (A -> void) -> Unit
                var iterMethod = MethodDeclaration(
                                        structUnit,
                                        Identifier("Iter"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("f"))
                                                .WithType(
                                                    GenericName(
                                                        Identifier("Action"))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SingletonSeparatedList<TypeSyntax>(
                                                                IdentifierName(genA))))))))
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
                                                                    IdentifierName("__comp"),
                                                                    IdentifierName("Iter")))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                        Argument(
                                                                            IdentifierName("f")))))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                var foldMethod = MethodDeclaration(
                                        GenericName(
                                            Identifier("Func"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SeparatedList<TypeSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        IdentifierName(envType),
                                                        Token(SyntaxKind.CommaToken),
                                                        IdentifierName("S")}))),
                                        Identifier("Fold"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithTypeParameterList(
                                        TypeParameterList(
                                            SingletonSeparatedList<TypeParameterSyntax>(
                                                TypeParameter(
                                                    Identifier("S")))))
                                    .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Parameter(
                                                        Identifier("state"))
                                                    .WithType(
                                                        IdentifierName("S")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        GenericName(
                                                            Identifier("Func"))
                                                        .WithTypeArgumentList(
                                                            TypeArgumentList(
                                                                SeparatedList<TypeSyntax>(
                                                                    new SyntaxNodeOrToken[]{
                                                                        IdentifierName("S"),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        IdentifierName(genA),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        IdentifierName("S")}))))})))
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
                                                SimpleLambdaExpression(
                                                    Parameter(
                                                        Identifier("env")),
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
                                                                                IdentifierName("__comp")),
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
                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                        Argument(
                                                                            IdentifierName("env"))))),
                                                            IdentifierName("IfNoneOrFail")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    IdentifierName("state")))))))));

                var forAllMethod = MethodDeclaration(
                                        GenericName(
                                            Identifier("Func"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SeparatedList<TypeSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        IdentifierName(envType),
                                                        Token(SyntaxKind.CommaToken),
                                                        PredefinedType(
                                                            Token(SyntaxKind.BoolKeyword))}))),
                                        Identifier("ForAll"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Parameter(
                                                        Identifier("f"))
                                                    .WithType(
                                                        GenericName(
                                                            Identifier("Func"))
                                                        .WithTypeArgumentList(
                                                            TypeArgumentList(
                                                                SeparatedList<TypeSyntax>(
                                                                    new SyntaxNodeOrToken[]{
                                                                        IdentifierName(genA),
                                                                        Token(SyntaxKind.CommaToken),
                                                                        PredefinedType(
                                                                            Token(SyntaxKind.BoolKeyword))}))))})))
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
                                                SimpleLambdaExpression(
                                                    Parameter(
                                                        Identifier("env")),
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
                                                                                IdentifierName("__comp")),
                                                                            IdentifierName("ForAll")))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                                Argument(
                                                                                    IdentifierName("f"))))),
                                                                    IdentifierName("Run")))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                        Argument(
                                                                            IdentifierName("env"))))),
                                                            IdentifierName("IfNoneOrFail")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    LiteralExpression(
                                                                        SyntaxKind.FalseLiteralExpression)))))))));

                var existsMethod = MethodDeclaration(
                                        GenericName(
                                            Identifier("Func"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SeparatedList<TypeSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        IdentifierName(envType),
                                                        Token(SyntaxKind.CommaToken),
                                                        PredefinedType(
                                                            Token(SyntaxKind.BoolKeyword))}))),
                                        Identifier("Exists"))
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithParameterList(
                                            ParameterList(
                                                SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("f"))
                                                        .WithType(
                                                            GenericName(
                                                                Identifier("Func"))
                                                            .WithTypeArgumentList(
                                                                TypeArgumentList(
                                                                    SeparatedList<TypeSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            IdentifierName(genA),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            PredefinedType(
                                                                                Token(SyntaxKind.BoolKeyword))}))))})))
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
                                                    SimpleLambdaExpression(
                                                        Parameter(
                                                            Identifier("env")),
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
                                                                                    IdentifierName("__comp")),
                                                                                IdentifierName("Exists")))
                                                                        .WithArgumentList(
                                                                            ArgumentList(
                                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                                    Argument(
                                                                                        IdentifierName("f"))))),
                                                                        IdentifierName("Run")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                            Argument(
                                                                                IdentifierName("env"))))),
                                                                IdentifierName("IfNoneOrFail")))
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SingletonSeparatedList<ArgumentSyntax>(
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.FalseLiteralExpression)))))))));

                var localMethod = MethodDeclaration(structA, Identifier("Local"))
                                      .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                      .WithParameterList(
                                          ParameterList(
                                              SingletonSeparatedList<ParameterSyntax>(
                                                  Parameter(Identifier("f"))
                                                    .WithType(GenericName(Identifier("Func"))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SeparatedList<TypeSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                    ParseTypeName(envType),
                                                                    Token(SyntaxKind.CommaToken),
                                                                    ParseTypeName(envType)
                                                                })))))))
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
                                                                            IdentifierName("LanguageExt"),
                                                                            IdentifierName("Prelude")),
                                                                        IdentifierName("local")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Argument(
                                                                                    IdentifierName("__comp")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Argument(
                                                                                    IdentifierName("f"))})))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken));

                // [name] :: MA -> (A -> B) -> MB
                MethodDeclarationSyntax MapMethod(string name) => 
                    MethodDeclaration(structB, Identifier(name))
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
                                        GenericName(
                                            Identifier("Func"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SeparatedList<TypeSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        IdentifierName(genA),
                                                        Token(SyntaxKind.CommaToken),
                                                        IdentifierName(genB)})))))))
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
                                                        IdentifierName("__comp"),
                                                        IdentifierName("Map")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("f")))))))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));

                // [name] :: MA -> (A -> MB) -> MB
                MethodDeclarationSyntax BindMethod(string name) =>
                    MethodDeclaration(structB, Identifier(name))
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
                                            GenericName(
                                                Identifier("Func"))
                                            .WithTypeArgumentList(
                                                TypeArgumentList(
                                                    SeparatedList<TypeSyntax>(
                                                        new SyntaxNodeOrToken[]{
                                                            IdentifierName(genA),
                                                            Token(SyntaxKind.CommaToken),
                                                            structB })))))))
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
                                                            IdentifierName("__comp"),
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
                                                                            IdentifierName("__comp")))))))))))))
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken));

                // [name] :: MA -> (A -> bool) -> MA
                MethodDeclarationSyntax FilterMethod(string name) =>
                    MethodDeclaration(structA, Identifier(name))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))
                        .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList<ParameterSyntax>(
                                    Parameter(
                                        Identifier("f"))
                                    .WithType(
                                        GenericName(
                                            Identifier("Func"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SeparatedList<TypeSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        IdentifierName(genA),
                                                        Token(SyntaxKind.CommaToken),
                                                        PredefinedType(
                                                            Token(SyntaxKind.BoolKeyword))})))))))
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
                                                        IdentifierName("__comp"),
                                                        IdentifierName("Where")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("f")))))))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));


                partialStruct = partialStruct.WithMembers(
                    List(
                        new MemberDeclarationSyntax[]{
                            compField,
                            ctor,
                            returnMethod,
                            failMethod,
                            MapMethod("Map"),
                            MapMethod("Select"),
                            BindMethod("Bind"),
                            BindMethod("SelectMany"),
                            selectManyMethod,
                            runMethod,
                            FilterMethod("Filter"),
                            FilterMethod("Where"),
                            doMethod,
                            strictMethod,
                            toSeqMethod,
                            iterMethod,
                            foldMethod,
                            forAllMethod,
                            existsMethod,
                            localMethod }));

                // Return :: A -> MA
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

                // Fail :: Unit -> MA
                var failFunc = MethodDeclaration(
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
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                structA,
                                                IdentifierName(failName))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                // asks :: (Env -> A) -> MA
                var asksFunc = MethodDeclaration(
                                        structA,
                                        Identifier("asks"))
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
                                                    GenericName(
                                                        Identifier("Func"))
                                                    .WithTypeArgumentList(
                                                        TypeArgumentList(
                                                            SeparatedList<TypeSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                    IdentifierName(envType),
                                                                    Token(SyntaxKind.CommaToken),
                                                                    IdentifierName(genA)})))))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            ObjectCreationExpression(structA)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            SimpleLambdaExpression(
                                                                Parameter(
                                                                    Identifier("env")),
                                                                TupleExpression(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            Argument(
                                                                                InvocationExpression(
                                                                                    IdentifierName("f"))
                                                                                .WithArgumentList(
                                                                                    ArgumentList(
                                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                                            Argument(
                                                                                                IdentifierName("env")))))),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            Argument(
                                                                                LiteralExpression(
                                                                                    SyntaxKind.FalseLiteralExpression))})))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken));

                // ask :: M Env
                var askFunc = applyToStruct.TypeParameterList.Parameters.Count == 1
                    ? FieldDeclaration(
                            VariableDeclaration(structEnv)
                            .WithVariables(
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    VariableDeclarator(
                                        Identifier("ask"))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            ObjectCreationExpression(structEnv)
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            SimpleLambdaExpression(
                                                                Parameter(
                                                                    Identifier("env")),
                                                                TupleExpression(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            Argument(
                                                                                IdentifierName("env")),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            Argument(
                                                                                LiteralExpression(
                                                                                    SyntaxKind.FalseLiteralExpression))}))))))))))))
                        .WithModifiers(
                            TokenList(
                                new[]{
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword),
                                    Token(SyntaxKind.ReadOnlyKeyword)})) as MemberDeclarationSyntax
                    : MethodDeclaration(
                            structEnv,
                            Identifier("ask"))
                        .WithModifiers(
                            TokenList(
                                new[]{
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword)}))
                        .WithTypeParameterList(
                            TypeParameterList(
                                SeparatedList<TypeParameterSyntax>(
                                    applyToStruct.TypeParameterList.Parameters.Take(applyToStruct.TypeParameterList.Parameters.Count - 1))))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                ObjectCreationExpression(structEnv)
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                SimpleLambdaExpression(
                                                    Parameter(
                                                        Identifier("env")),
                                                    TupleExpression(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                Argument(
                                                                    IdentifierName("env")),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    LiteralExpression(
                                                                        SyntaxKind.FalseLiteralExpression))})))))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));

                var localFunc = MethodDeclaration(structA, Identifier("local"))
                                    .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                                    .WithTypeParameterList(applyToStruct.TypeParameterList)
                                    .WithParameterList(
                                        ParameterList(
                                            SeparatedList<ParameterSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Parameter(Identifier("ma")).WithType(structA),
                                                    Token(SyntaxKind.CommaToken),
                                                    Parameter(Identifier("f"))
                                                        .WithType(
                                                            GenericName(
                                                                Identifier("Func"))
                                                            .WithTypeArgumentList(
                                                                TypeArgumentList(
                                                                    SeparatedList<TypeSyntax>(
                                                                        new SyntaxNodeOrToken[]{
                                                                            ParseTypeName(envType),
                                                                            Token(SyntaxKind.CommaToken),
                                                                            ParseTypeName(envType)
                                                                        }))))})))
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
                                    failFunc,
                                    asksFunc,
                                    askFunc,
                                    localFunc
                                }));

                // Ask
                var ask = applyToStruct.TypeParameterList.Parameters.Count > 1
                            ? MemberAccessExpression(
                                  SyntaxKind.SimpleMemberAccessExpression,
                                  InvocationExpression(
                                      GenericName(Identifier("ask"))
                                          .WithTypeArgumentList(
                                                TypeArgumentList(
                                                     SeparatedList<TypeSyntax>(
                                                         applyToStruct.TypeParameterList
                                                                      .Parameters
                                                                      .Take(applyToStruct.TypeParameterList.Parameters.Count - 1)
                                                                      .Select(p => IdentifierName(p.Identifier.ToString())))))),
                                  IdentifierName("Map"))
                            : MemberAccessExpression(
                                  SyntaxKind.SimpleMemberAccessExpression,
                                       IdentifierName("ask"),
                                       IdentifierName("Map"));


                prelude = CodeGenUtil.AddMembersToPrelude(prelude, applyToStruct, "ask", (ITypeSymbol)envTypeConst);

                return Task.FromResult<SyntaxList<MemberDeclarationSyntax>>(results.Add(partialStruct).Add(prelude));
            }
            else
            {
                return Task.FromResult<SyntaxList<MemberDeclarationSyntax>>(results);
            }
        }
    }
}
