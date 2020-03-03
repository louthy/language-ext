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
using System.Reflection.Metadata.Ecma335;

namespace LanguageExt.CodeGen
{
    public class FreeGenerator : ICodeGenerator
    {
        public FreeGenerator(AttributeData attributeData)
        {
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
            TransformationContext context,
            IProgress<Diagnostic> progress,
            CancellationToken cancellationToken)
        {
            try
            {

                if (context.ProcessingNode is InterfaceDeclarationSyntax applyTo)
                {
                    if (applyTo.TypeParameterList.Parameters.Count > 1)
                    {
                        CodeGenUtil.ReportError($"Free monads must have only one generic argument", "Free monad Code-Gen", context.ProcessingNode, progress);
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }
                    if (applyTo.TypeParameterList.Parameters.Count == 0)
                    {
                        CodeGenUtil.ReportError($"Free monads must a generic argument", "Free monad Code-Gen", context.ProcessingNode, progress);
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }

                    var genA = applyTo.TypeParameterList.Parameters.First().ToString();
                    var genB = CodeGenUtil.NextGenName(genA);
                    var genC = CodeGenUtil.NextGenName(genB);
                    
                    var thisType = ParseTypeName($"{applyTo.Identifier.Text}{applyTo.TypeParameterList}");
                    
                    var typeA = MakeTypeName(applyTo.Identifier.Text, genA);
                    var typeB = MakeTypeName(applyTo.Identifier.Text, genB);
                    var mapFunc = ParseTypeName($"System.Func<{genA}, {genB}>");

                    var mapIsStatic = applyTo.Members
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => m.Identifier.Text == "Map")
                        .Where(m => m.ParameterList.Parameters.Count == 2)
                        .Where(m => m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.StaticKeyword)))
                        .Where(m => m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.PublicKeyword)))
                        // .Where(m => m.ReturnType.IsEquivalentTo(typeB))
                        // .Where(m => m.ParameterList.Parameters[0].Type.IsEquivalentTo(typeA))
                        // .Where(m => m.ParameterList.Parameters[1].Type.IsEquivalentTo(mapFunc))
                        .Any();
                    
                    var caseMethods = applyTo.Members
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => !m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.StaticKeyword)))
                        .Select(m => MakeFree(m, thisType))
                        .ToArray();
                    
                    var firstPure = caseMethods.Where(HasPureAttr)
                                               .Where(m => m.ParameterList.Parameters.Count == 1)
                                               .Where(m => m.ReturnType.IsEquivalentTo(typeA))
                                               .Where(m => m.ParameterList.Parameters.First().Type.ToString() == genA)
                                               .FirstOrDefault();

                    if (firstPure == null)
                    {
                        CodeGenUtil.ReportError($"Type can't be made into a free monad because no method in the interface has [Pure] attribute that takes a single argument of type '{genA}' and returns a '{genA}'", "Free monad Code-Gen", context.ProcessingNode, progress);
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }

                    var caseRes = caseMethods
                        .Zip(Enumerable.Range(1, int.MaxValue), (m, i) => (m, i))
                        .Select(m => CodeGenUtil.MakeCaseType(
                            context,
                            progress,
                            applyTo.Identifier,
                            applyTo.Members,
                            applyTo.TypeParameterList,
                            applyTo.Modifiers,
                            applyTo.ConstraintClauses,
                            m.m.Identifier,
                            m.m.TypeParameterList,
                            m.m.ParameterList
                                .Parameters
                                .Select(p => (p.Identifier, p.Type, p.Modifiers, p.AttributeLists))
                                .ToList(),
                            BaseSpec.Interface,
                            caseIsClass: true,
                            caseIsPartial: false,
                            includeWithAndLenses: false,
                            m.i))
                        .ToList();

                    var ok = caseRes.All(x => x.Success);
                    var cases = caseRes.Select(c => c.Type);

                    var staticCtorClass = MakeStaticClass(
                        applyTo.Identifier, 
                        caseMethods,
                        applyTo.TypeParameterList, 
                        applyTo.ConstraintClauses,
                        firstPure,
                        mapIsStatic);

                    if (ok)
                    {
                        return Task.FromResult(List<MemberDeclarationSyntax>().AddRange(cases).Add(staticCtorClass));
                    }
                    else
                    {
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }
                }
                else
                {
                    CodeGenUtil.ReportError($"Type can't be made into a free monad.  It must be an interface", "Free monad Code-Gen", context.ProcessingNode, progress);
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }

            }
            catch (Exception e)
            {
                CodeGenUtil.ReportError(e.Message, "Free monad Code-Gen", context.ProcessingNode, progress);
                foreach (var line in e.StackTrace.Split('\n'))
                {
                    CodeGenUtil.ReportError(line, "Free monad Code-Gen", context.ProcessingNode, progress);
                }

                return Task.FromResult(List<MemberDeclarationSyntax>());
            }
        }

        static bool HasPureAttr(MethodDeclarationSyntax m) =>
            m.AttributeLists != null &&
            m.AttributeLists
                .SelectMany<AttributeListSyntax, AttributeSyntax>(a => a.Attributes)
                .Any(a => a.Name.ToString() == "Pure");

        static MemberDeclarationSyntax[] AddMonadDefaults(
            SyntaxToken applyToIdentifier,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);
            
            var typeA = MakeTypeName(applyToIdentifier.Text,genA);
            var liftedTypeA = MakeTypeName(applyToIdentifier.Text, typeA.ToString());
            var typeB = MakeTypeName(applyToIdentifier.Text,genB);
            var typeC = MakeTypeName(applyToIdentifier.Text,genC);
            var bindFuncType = ParseTypeName($"System.Func<{genA}, {typeB}>");
            var mapFuncType = ParseTypeName($"System.Func<{genA}, {genB}>");
            var projFuncType = ParseTypeName($"System.Func<{genA}, {genB}, {genC}>");

            var comma = Token(SyntaxKind.CommaToken);
            var typeParamA = TypeParameter(Identifier(genA));
            var typeParamB = TypeParameter(Identifier(genB));
            var typeParamC = TypeParameter(Identifier(genC));
            var typeParamsA =
                TypeParameterList(SeparatedList<TypeParameterSyntax>(new SyntaxNodeOrToken[] {typeParamA}));
            
            var typeParamsAB = TypeParameterList(
                SeparatedList<TypeParameterSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        typeParamA,
                        comma, 
                        typeParamB
                    }));
            var typeParamsABC = TypeParameterList(
                SeparatedList<TypeParameterSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        typeParamA,
                        comma, 
                        typeParamB,
                        comma, 
                        typeParamC
                    }));
            var pubStatMods = TokenList(new[] {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)});

            var thisMA = Parameter(Identifier("ma"))
                .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                .WithType(typeA);

            var thisMMA = Parameter(Identifier("mma"))
                .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                .WithType(liftedTypeA);

            return new MemberDeclarationSyntax[]{
                MethodDeclaration(
                    typeB,
                    Identifier("Select"))
                .WithModifiers(pubStatMods)
                .WithTypeParameterList(typeParamsAB)
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]{
                                thisMA,
                                comma,
                                Parameter(
                                    Identifier("f"))
                                .WithType(mapFuncType)})))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("Map"))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]{
                                        Argument(
                                            IdentifierName("ma")),
                                        comma,
                                        Argument(
                                            IdentifierName("f"))})))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                MethodDeclaration(typeB, Identifier("SelectMany"))
                .WithModifiers(pubStatMods)
                .WithTypeParameterList(typeParamsAB)
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]{
                                thisMA,
                                comma,
                                Parameter(Identifier("f")).WithType(bindFuncType)})))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("Bind"))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]{
                                        Argument(
                                            IdentifierName("ma")),
                                        comma,
                                        Argument(
                                            IdentifierName("f"))})))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken)),
                MethodDeclaration(typeC, Identifier("SelectMany"))
                .WithModifiers(pubStatMods)
                .WithTypeParameterList(typeParamsABC)
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]{
                                thisMA,
                                comma,
                                Parameter(Identifier("bind")).WithType(bindFuncType),
                                comma,
                                Parameter(Identifier("project")).WithType(projFuncType)})))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("Bind"))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]{
                                        Argument(
                                            IdentifierName("ma")),
                                        comma,
                                        Argument(
                                            SimpleLambdaExpression(
                                                Parameter(
                                                    Identifier("a")),
                                                InvocationExpression(
                                                    IdentifierName("Map"))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                Argument(
                                                                    InvocationExpression(
                                                                        IdentifierName("bind"))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                                Argument(
                                                                                    IdentifierName("a")))))),
                                                                comma,
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
                                                                                        comma,
                                                                                        Argument(
                                                                                            IdentifierName("b"))})))))})))))})))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken)),
                MethodDeclaration(typeA, Identifier("Flatten"))
                    .WithModifiers(pubStatMods)
                    .WithTypeParameterList(typeParamsA)
                .WithParameterList(ParameterList(SingletonSeparatedList<ParameterSyntax>(thisMMA)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("Bind"))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]{
                                        Argument(
                                            IdentifierName("mma")),
                                        comma,
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("LanguageExt"),
                                                    IdentifierName("Prelude")),
                                                IdentifierName("identity")))})))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken))
                };
        }

        static MethodDeclarationSyntax MakeFree(MethodDeclarationSyntax m, TypeSyntax freeType)
        {
            bool isPure = m.AttributeLists != null && m.AttributeLists
                           .SelectMany(a => a.Attributes)
                           .Any(a => a.Name.ToString() == "Pure");

            if (isPure)
            {
                return m.WithReturnType(freeType);
            }
            else
            {
                var type = QualifiedName(
                    IdentifierName("System"),
                    GenericName(
                            Identifier("Func"))
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SeparatedList<TypeSyntax>(
                                    new SyntaxNodeOrToken[] {m.ReturnType, Token(SyntaxKind.CommaToken), freeType}))));

                return m.WithParameterList(m.ParameterList
                                            .AddParameters(Parameter(Identifier("next")).WithType(type)))
                        .WithReturnType(freeType);
            }
        }

        static ClassDeclarationSyntax MakeStaticClass(
            SyntaxToken applyToIdentifier,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            MethodDeclarationSyntax pure,
            bool mapIsStatic
            )
        {
            var name = applyToIdentifier;
            var @class = ClassDeclaration(name)
                            .WithModifiers(
                                TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword) }));

            var returnType = ParseTypeName($"{applyToIdentifier}{applyToTypeParams}");

            var cases = applyToMembers
                               .Select(m => MakeCaseCtorFunction(applyToIdentifier, applyToTypeParams, applyToConstraints, returnType, m, pure))
                               .ToArray();


            var bind = MakeBindFunction(applyToIdentifier, applyToConstraints, applyToMembers, applyToTypeParams, pure, mapIsStatic);
            var map = mapIsStatic
                ? MakeMapExtension(applyToIdentifier, applyToConstraints, applyToMembers, applyToTypeParams)
                : MakeMapFunction(applyToIdentifier, applyToConstraints, applyToMembers, applyToTypeParams, pure);
            var monad = AddMonadDefaults(applyToIdentifier, applyToMembers, applyToTypeParams, applyToConstraints);
            
            return @class.WithMembers(List(cases).Add(bind).Add(map).AddRange(monad));
        }

        static TypeSyntax MakeTypeName(string ident, string gen) =>
            ParseTypeName($"{ident}<{gen}>");
 
        static MethodDeclarationSyntax MakeBindFunction(
            SyntaxToken applyToIdentifier, 
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            MethodDeclarationSyntax pure,
            bool mapIsStatic)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);
            
            var typeA = MakeTypeName(applyToIdentifier.Text,genA);
            var typeB = MakeTypeName(applyToIdentifier.Text,genB);
            var typeC = MakeTypeName(applyToIdentifier.Text,genC);
            var bindFunc = ParseTypeName($"System.Func<{genA}, {typeB}>");

            var mapFunc = mapIsStatic
                ? InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    typeA,
                                    IdentifierName("Map")))
                            .WithArgumentList(
                                ArgumentList(
                                    SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            Argument(
                                                InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("v"),
                                                            IdentifierName("Next")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    IdentifierName("n")))))),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                ParenthesizedExpression(
                                                    IdentifierName("f")))
                                        }))),
                        IdentifierName("Bind")))
                : InvocationExpression(IdentifierName("Flatten")).WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList<ArgumentSyntax>(
                            Argument(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("v"),
                                            IdentifierName("Next")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    IdentifierName("n"))))),
                                IdentifierName("Map")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        IdentifierName("f")))))))));

            var pureFunc =
                    new SyntaxNodeOrToken[]
                    {
                        SwitchExpressionArm(
                            DeclarationPattern(
                                ParseTypeName($"{pure.Identifier.Text}<{genA}>"),
                                SingleVariableDesignation(Identifier("v"))),
                            InvocationExpression(IdentifierName("f"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("v"),
                                                    IdentifierName(CodeGenUtil.MakeFirstCharUpper(pure.ParameterList.Parameters.First().Identifier.Text)))))))),
                        Token(SyntaxKind.CommaToken)
                    };

            var termimalFuncs = applyToMembers
               .Where(m => m != pure && m.AttributeLists != null && m.AttributeLists.SelectMany(a => a.Attributes).Any(a => a.Name.ToString() == "Pure"))
               .SelectMany(m => 
                   new SyntaxNodeOrToken[]
                   {
                       SwitchExpressionArm(
                           DeclarationPattern(
                               ParseTypeName($"{m.Identifier.Text}<{genA}>"),
                               SingleVariableDesignation(Identifier("v"))),
                               ObjectCreationExpression(MakeTypeName(m.Identifier.Text, genB))
                              .WithArgumentList(
                                   ArgumentList(
                                       SeparatedList<ArgumentSyntax>(
                                           m.ParameterList
                                            .Parameters
                                            .Select(p =>  
                                               Argument(
                                                   MemberAccessExpression(
                                                       SyntaxKind.SimpleMemberAccessExpression,
                                                       IdentifierName("v"),
                                                       IdentifierName(CodeGenUtil.MakeFirstCharUpper(p.Identifier))))))))),
                       Token(SyntaxKind.CommaToken)
                   });


            var freeFuncs = applyToMembers
                .Where(m => m.AttributeLists == null || !m.AttributeLists.SelectMany(a => a.Attributes).Any(a => a.Name.ToString() == "Pure"))
                .SelectMany(m => 
                    new SyntaxNodeOrToken[]
                    {
                        SwitchExpressionArm(
                            DeclarationPattern(
                                ParseTypeName($"{m.Identifier.Text}<{genA}>"),
                                SingleVariableDesignation(Identifier("v"))),
                                ObjectCreationExpression(MakeTypeName(m.Identifier.Text, genB))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            Enumerable.Concat(
                                                m.ParameterList
                                                    .Parameters
                                                    .Take(m.ParameterList.Parameters.Count - 1)
                                                    .SelectMany(p =>
                                                        new SyntaxNodeOrToken[]{
                                                            Argument(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName("v"),
                                                                    IdentifierName(CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text)))),
                                                            Token(SyntaxKind.CommaToken) 
                                                        }),
                                                new SyntaxNodeOrToken [1] {
                                                    Argument(SimpleLambdaExpression(Parameter(Identifier("n")), mapFunc))
                                                }))))),
                        Token(SyntaxKind.CommaToken)
                    });

            var tokens = new List<SyntaxNodeOrToken>();
            tokens.AddRange(pureFunc);
            tokens.AddRange(termimalFuncs);
            tokens.AddRange(freeFuncs);
            tokens.Add(
                SwitchExpressionArm(
                    DiscardPattern(),
                    ThrowExpression(
                        ObjectCreationExpression(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("NotSupportedException")))
                        .WithArgumentList(ArgumentList()))));

            return MethodDeclaration(typeB, Identifier("Bind"))
                .WithModifiers(
                    TokenList(new[] {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)}))
                .WithTypeParameterList(
                    TypeParameterList(
                        SeparatedList<TypeParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                TypeParameter(
                                    Identifier(genA)),
                                Token(SyntaxKind.CommaToken), TypeParameter(
                                    Identifier(genB))
                            })))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(
                                        Identifier("ma"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.ThisKeyword)))
                                    .WithType(typeA),
                                Token(SyntaxKind.CommaToken), Parameter(
                                        Identifier("f"))
                                    .WithType(bindFunc)
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        SwitchExpression(
                                IdentifierName("ma"))
                            .WithArms(SeparatedList<SwitchExpressionArmSyntax>(tokens))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken))
                .NormalizeWhitespace();
        }

        static MethodDeclarationSyntax MakeMapExtension(
            SyntaxToken applyToIdentifier, 
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var typeA = MakeTypeName(applyToIdentifier.Text,genA);
            var typeB = MakeTypeName(applyToIdentifier.Text,genB);
            var mapFuncType = ParseTypeName($"System.Func<{genA}, {genB}>");

            return MethodDeclaration(
                    typeB,
                    Identifier("Map"))
                        .WithModifiers(
                            TokenList(
                                new[] {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)}))
                        .WithTypeParameterList(
                            TypeParameterList(
                                SeparatedList<TypeParameterSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        TypeParameter(
                                            Identifier(genA)),
                                        Token(SyntaxKind.CommaToken), TypeParameter(
                                            Identifier(genB))
                                    })))
                        .WithParameterList(
                            ParameterList(
                                SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        Parameter(
                                                Identifier("ma"))
                                            .WithModifiers(
                                                TokenList(
                                                    Token(SyntaxKind.ThisKeyword)))
                                            .WithType(typeA),
                                        Token(SyntaxKind.CommaToken), Parameter(Identifier("f")).WithType(mapFuncType)
                                    })))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            typeA,
                                            IdentifierName("Map")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]
                                                {
                                                    Argument(
                                                        IdentifierName("ma")),
                                                    Token(SyntaxKind.CommaToken), Argument(
                                                        IdentifierName("f"))
                                                })))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken));
        }

        static MethodDeclarationSyntax MakeMapFunction(
            SyntaxToken applyToIdentifier, 
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            MethodDeclarationSyntax pure)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);
            
            var typeA = MakeTypeName(applyToIdentifier.Text,genA);
            var typeB = MakeTypeName(applyToIdentifier.Text,genB);
            var typeC = MakeTypeName(applyToIdentifier.Text,genC);
            var mapFuncType = ParseTypeName($"System.Func<{genA}, {genB}>");
            var pureTypeA = MakeTypeName(pure.Identifier.Text, genA);
            var pureTypeB = MakeTypeName(pure.Identifier.Text, genB);

            var mapFunc = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    InvocationExpression(
                         MemberAccessExpression(
                             SyntaxKind.SimpleMemberAccessExpression,
                             IdentifierName("v"),
                             IdentifierName("Next")))
                        .WithArgumentList(ArgumentList(SingletonSeparatedList<ArgumentSyntax>(Argument(IdentifierName("n"))))),
                    IdentifierName("Map")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList<ArgumentSyntax>(
                            Argument(
                                IdentifierName("f")))));

            var pureFunc =
                    new SyntaxNodeOrToken[]
                    {
                        SwitchExpressionArm(
                            DeclarationPattern(
                                pureTypeA,
                                SingleVariableDesignation(Identifier("v"))),
                            ObjectCreationExpression(pureTypeB)
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                InvocationExpression(IdentifierName("f"))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("v"),
                                                                        IdentifierName(CodeGenUtil.MakeFirstCharUpper(pure.ParameterList.Parameters.First().Identifier.Text)))))))))))),
                        Token(SyntaxKind.CommaToken)
                    };

            var termimalFuncs = applyToMembers
               .Where(m => m != pure && m.AttributeLists != null && m.AttributeLists.SelectMany(a => a.Attributes).Any(a => a.Name.ToString() == "Pure"))
               .SelectMany(m => 
                   new SyntaxNodeOrToken[]
                   {
                       SwitchExpressionArm(
                           DeclarationPattern(
                               ParseTypeName($"{m.Identifier.Text}<{genA}>"),
                               SingleVariableDesignation(Identifier("v"))),
                               ObjectCreationExpression(MakeTypeName(m.Identifier.Text, genB))
                              .WithArgumentList(
                                   ArgumentList(
                                       SeparatedList<ArgumentSyntax>(
                                           m.ParameterList
                                            .Parameters
                                            .Select(p =>  
                                               Argument(
                                                   MemberAccessExpression(
                                                       SyntaxKind.SimpleMemberAccessExpression,
                                                       IdentifierName("v"),
                                                       IdentifierName(CodeGenUtil.MakeFirstCharUpper(p.Identifier))))))))),
                       Token(SyntaxKind.CommaToken)
                   });


            var freeFuncs = applyToMembers
                .Where(m => m.AttributeLists == null || !m.AttributeLists.SelectMany(a => a.Attributes).Any(a => a.Name.ToString() == "Pure"))
                .SelectMany(m => 
                    new SyntaxNodeOrToken[]
                    {
                        SwitchExpressionArm(
                            DeclarationPattern(
                                ParseTypeName($"{m.Identifier.Text}<{genA}>"),
                                SingleVariableDesignation(Identifier("v"))),
                                ObjectCreationExpression(MakeTypeName(m.Identifier.Text, genB))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            Enumerable.Concat(
                                                m.ParameterList
                                                    .Parameters
                                                    .Take(m.ParameterList.Parameters.Count - 1)
                                                    .SelectMany(p =>
                                                        new SyntaxNodeOrToken[]{
                                                            Argument(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName("v"),
                                                                    IdentifierName(CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text)))),
                                                            Token(SyntaxKind.CommaToken) 
                                                        }),
                                                new SyntaxNodeOrToken [1] {
                                                    Argument(SimpleLambdaExpression(Parameter(Identifier("n")), mapFunc))
                                                }))))),
                        Token(SyntaxKind.CommaToken)
                    });

            var tokens = new List<SyntaxNodeOrToken>();
            tokens.AddRange(pureFunc);
            tokens.AddRange(termimalFuncs);
            tokens.AddRange(freeFuncs);
            tokens.Add(
                SwitchExpressionArm(
                    DiscardPattern(),
                    ThrowExpression(
                        ObjectCreationExpression(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("NotSupportedException")))
                        .WithArgumentList(ArgumentList()))));

            return MethodDeclaration(typeB, Identifier("Map"))
                .WithModifiers(
                    TokenList(new[] {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)}))
                .WithTypeParameterList(
                    TypeParameterList(
                        SeparatedList<TypeParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                TypeParameter(
                                    Identifier(genA)),
                                Token(SyntaxKind.CommaToken), TypeParameter(
                                    Identifier(genB))
                            })))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(
                                        Identifier("ma"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.ThisKeyword)))
                                    .WithType(typeA),
                                Token(SyntaxKind.CommaToken), Parameter(
                                        Identifier("f"))
                                    .WithType(mapFuncType)
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        SwitchExpression(
                                IdentifierName("ma"))
                            .WithArms(SeparatedList<SwitchExpressionArmSyntax>(tokens))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken))
                .NormalizeWhitespace();
        }        

        static MemberDeclarationSyntax MakeCaseCtorFunction(
            SyntaxToken applyToIdentifier,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            TypeSyntax returnType, 
            MethodDeclarationSyntax method,
            MethodDeclarationSyntax pure)
        {
            bool isPure = method.AttributeLists
                                .SelectMany(a => a.Attributes)
                                .Any(a => a.Name.ToString() == "Pure");

            if (isPure)
            {
                var typeParamList = applyToTypeParams;
                if (method.TypeParameterList != null)
                {
                    typeParamList = typeParamList.AddParameters(method.TypeParameterList.Parameters.ToArray());
                }

                var thisType = ParseTypeName($"{method.Identifier.Text}{typeParamList}");

                var paramIdents = method.ParameterList
                    .Parameters
                    .Select(p => (SyntaxNodeOrToken)Argument(IdentifierName(p.Identifier.Text)))
                    .ToArray();

                var args = CodeGenUtil.Interleave(
                    paramIdents,
                    Token(SyntaxKind.CommaToken));

                var @case = MethodDeclaration(returnType, method.Identifier)
                    .WithModifiers(
                        TokenList(
                            new[]{
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)}))
                    .WithParameterList(
                        isPure
                            ? method.ParameterList
                            : method.ParameterList.WithParameters(method.ParameterList.Parameters.RemoveAt(method.ParameterList.Parameters.Count - 1)))
                    .WithConstraintClauses(applyToConstraints)
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            ObjectCreationExpression(thisType)
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(args)))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken));

                if(typeParamList != null)
                {
                    @case = @case.WithTypeParameterList(typeParamList);
                }
                return @case;
            }
            else
            {
                var nextType = ((GenericNameSyntax)((QualifiedNameSyntax)method.ParameterList.Parameters.Last().Type).Right)
                                    .TypeArgumentList
                                    .Arguments
                                    .First();

                var thisType = ParseTypeName($"{method.Identifier.Text}<{nextType}>");
                returnType = ParseTypeName($"{applyToIdentifier}<{nextType}>");

                var paramIdents = method.ParameterList
                    .Parameters
                    .Select(p => (SyntaxNodeOrToken)Argument(IdentifierName(p.Identifier.Text)))
                    .ToArray();

                paramIdents[paramIdents.Length - 1] = Argument(IdentifierName(pure.Identifier.Text));

                var args = CodeGenUtil.Interleave(
                    paramIdents,
                    Token(SyntaxKind.CommaToken));

                var @case = MethodDeclaration(returnType, method.Identifier)
                    .WithModifiers(
                        TokenList(
                            new[]{
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)}))
                    .WithParameterList(
                        method.ParameterList.WithParameters(method.ParameterList.Parameters.RemoveAt(method.ParameterList.Parameters.Count - 1)))
                    .WithConstraintClauses(applyToConstraints)
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            ObjectCreationExpression(thisType)
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(args)))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken));
                
                return @case;                
            }
        }
    }
}
