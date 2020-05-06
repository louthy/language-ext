using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
                        CodeGenUtil.ReportError($"Free monads must have only one generic argument",
                            "Free monad Code-Gen", context.ProcessingNode, progress);
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }

                    if (applyTo.TypeParameterList.Parameters.Count == 0)
                    {
                        CodeGenUtil.ReportError($"Free monads must a generic argument", "Free monad Code-Gen",
                            context.ProcessingNode, progress);
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


                    var hasStaticBiMap = applyTo.Members
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => m.Identifier.Text == "BiMap")
                        .Where(m => m.ParameterList.Parameters.Count == 3)
                        .Where(m => m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.StaticKeyword)))
                        .Where(m => m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.PublicKeyword)))
                        .Any();

                    var failType = applyTo.Members
                        .OfType<MethodDeclarationSyntax>()
                        .Where(HasFailAttr)
                        .Where(m => m.ParameterList.Parameters.Count == 1)
                        .Select(m => m.ParameterList.Parameters.First().Type)
                        .FirstOrDefault();

                    var caseMethods = applyTo.Members
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => !m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.StaticKeyword)))
                        .Select(m => MakeFree(m, thisType, failType))
                        .ToArray();

                    var firstPure = caseMethods
                        .Where(HasPureAttr)
                        .Where(m => m.ParameterList.Parameters.Count == 1)
                        .Where(m => m.ReturnType.IsEquivalentTo(typeA))
                        .Where(m => m.ParameterList.Parameters.First().Type.ToString() == genA)
                        .FirstOrDefault();

                    var firstFail = caseMethods
                        .Where(HasFailAttr)
                        .Where(m => m.ParameterList.Parameters.Count < 2)
                        .Where(m => m.ReturnType.IsEquivalentTo(typeA))
                        .FirstOrDefault();

                    var failTypeCount = caseMethods
                        .Where(HasFailAttr)
                        .Count();

                    if (failTypeCount > 1)
                    {
                        CodeGenUtil.ReportError(
                            $"Type can't be made into a free monad because more than one member is marked with the 'Fail' attribute",
                            "Free monad Code-Gen", context.ProcessingNode, progress);
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }

                    if (firstPure == null)
                    {
                        CodeGenUtil.ReportError(
                            $"Type can't be made into a free monad because no method in the interface has [Pure] attribute that takes a single argument of type '{genA}' and returns a '{genA}'",
                            "Free monad Code-Gen", context.ProcessingNode, progress);
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }

                    if (firstFail != null && mapIsStatic && !hasStaticBiMap)
                    {
                        CodeGenUtil.ReportError(
                            $"Type can't be made into a free monad because no static 'BiMap' method found",
                            "Free monad Code-Gen", context.ProcessingNode, progress);
                        return Task.FromResult(List<MemberDeclarationSyntax>());
                    }

                    var caseRes = caseMethods
                        .Zip(Enumerable.Range(1, int.MaxValue), (m, i) => (m, i))
                        .Select(m => CodeGenUtil.MakeCaseType(
                            context: context,
                            progress: progress,
                            applyToIdentifier: applyTo.Identifier,
                            applyToMembers: applyTo.Members,
                            applyToTypeParams: applyTo.TypeParameterList,
                            applyToModifiers: applyTo.Modifiers,
                            applyToConstraints: applyTo.ConstraintClauses,
                            caseIdentifier: m.m.Identifier,
                            caseTypeParams: m.m.TypeParameterList,
                            caseParams: m.m.ParameterList
                                .Parameters
                                .Select(p => (p.Identifier, p.Type, p.Modifiers, p.AttributeLists))
                                .ToList(),
                            baseSpec: BaseSpec.Interface,
                            caseIsClass: true,
                            caseIsPartial: false,
                            includeWithAndLenses: false,
                            tag: m.i))
                        .ToList();

                    var ok = caseRes.All(x => x.Success);
                    var cases = caseRes.Select(c => c.Type);

                    var staticCtorClass = MakeStaticClass
                    (
                        applyToIdentifier: applyTo.Identifier,
                        applyToMembers: caseMethods,
                        applyToTypeParams: applyTo.TypeParameterList,
                        applyToConstraints: applyTo.ConstraintClauses,
                        pure: firstPure,
                        fail: firstFail,
                        failType: failType,
                        mapIsStatic: mapIsStatic
                    );

                    return Task.FromResult(ok
                        ? List<MemberDeclarationSyntax>().AddRange(cases).Add(staticCtorClass)
                        : List<MemberDeclarationSyntax>());
                }

                CodeGenUtil.ReportError($"Type can't be made into a free monad.  It must be an interface",
                    "Free monad Code-Gen", context.ProcessingNode, progress);
                return Task.FromResult(List<MemberDeclarationSyntax>());
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

        static bool IsPureAttr(AttributeSyntax s) => s.Name.ToString() == "Pure";
        static bool IsFailAttr(AttributeSyntax s) => s.Name.ToString() == "Fail";

        static bool HasPureAttr(MethodDeclarationSyntax m) =>
            m.AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(IsPureAttr);

        static bool HasFailAttr(MethodDeclarationSyntax m) =>
            m.AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(IsFailAttr);


        static MemberDeclarationSyntax[] AddMonadDefaults(
            SyntaxToken applyToIdentifier,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);

            var typeA = MakeTypeName(applyToIdentifier.Text, genA);
            var liftedTypeA = MakeTypeName(applyToIdentifier.Text, typeA.ToString());
            var typeB = MakeTypeName(applyToIdentifier.Text, genB);
            var typeC = MakeTypeName(applyToIdentifier.Text, genC);
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
                    new SyntaxNodeOrToken[] {typeParamA, comma, typeParamB}));
            var typeParamsABC = TypeParameterList(
                SeparatedList<TypeParameterSyntax>(
                    new SyntaxNodeOrToken[] {typeParamA, comma, typeParamB, comma, typeParamC}));
            var pubStatMods = TokenList(new[] {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)});

            var thisMA = Parameter(Identifier("ma"))
                .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                .WithType(typeA);

            var thisMMA = Parameter(Identifier("mma"))
                .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                .WithType(liftedTypeA);

            return new MemberDeclarationSyntax[]
            {
                MethodDeclaration(
                        typeB,
                        Identifier("Select"))
                    .WithModifiers(pubStatMods)
                    .WithTypeParameterList(typeParamsAB)
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    thisMA, comma, Parameter(Identifier("f"))
                                        .WithType(mapFuncType)
                                })))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(
                                    IdentifierName("Map"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(IdentifierName("ma")), comma, Argument(IdentifierName("f"))
                                            })))))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                MethodDeclaration(typeB, Identifier("SelectMany"))
                    .WithModifiers(pubStatMods)
                    .WithTypeParameterList(typeParamsAB)
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    thisMA, comma, Parameter(Identifier("f")).WithType(bindFuncType)
                                })))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(
                                    IdentifierName("Bind"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(
                                                    IdentifierName("ma")),
                                                comma, Argument(
                                                    IdentifierName("f"))
                                            })))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken)),
                MethodDeclaration(typeC, Identifier("SelectMany"))
                    .WithModifiers(pubStatMods)
                    .WithTypeParameterList(typeParamsABC)
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    thisMA, comma, Parameter(Identifier("bind")).WithType(bindFuncType), comma,
                                    Parameter(Identifier("project")).WithType(projFuncType)
                                })))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(
                                    IdentifierName("Bind"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(
                                                    IdentifierName("ma")),
                                                comma, Argument(
                                                    SimpleLambdaExpression(
                                                        Parameter(
                                                            Identifier("a")),
                                                        InvocationExpression(
                                                                IdentifierName("Map"))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]
                                                                        {
                                                                            Argument(
                                                                                InvocationExpression(
                                                                                        IdentifierName("bind"))
                                                                                    .WithArgumentList(
                                                                                        ArgumentList(
                                                                                            SingletonSeparatedList(
                                                                                                Argument(
                                                                                                    IdentifierName(
                                                                                                        "a")))))),
                                                                            comma, Argument(
                                                                                SimpleLambdaExpression(
                                                                                    Parameter(
                                                                                        Identifier("b")),
                                                                                    InvocationExpression(
                                                                                            IdentifierName("project"))
                                                                                        .WithArgumentList(
                                                                                            ArgumentList(
                                                                                                SeparatedList<
                                                                                                    ArgumentSyntax>(
                                                                                                    new
                                                                                                        SyntaxNodeOrToken
                                                                                                        []
                                                                                                        {
                                                                                                            Argument(
                                                                                                                IdentifierName(
                                                                                                                    "a")),
                                                                                                            comma,
                                                                                                            Argument(
                                                                                                                IdentifierName(
                                                                                                                    "b"))
                                                                                                        })))))
                                                                        })))))
                                            })))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken)),
                MethodDeclaration(typeA, Identifier("Flatten"))
                    .WithModifiers(pubStatMods)
                    .WithTypeParameterList(typeParamsA)
                    .WithParameterList(ParameterList(SingletonSeparatedList(thisMMA)))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(
                                    IdentifierName("Bind"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(
                                                    IdentifierName("mma")),
                                                comma, Argument(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("LanguageExt"),
                                                            IdentifierName("Prelude")),
                                                        IdentifierName("identity")))
                                            })))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken))
            };
        }

        static MethodDeclarationSyntax MakeFree(MethodDeclarationSyntax m, TypeSyntax freeType, TypeSyntax failType)
        {
            if (HasPureAttr(m) || HasFailAttr(m))
            {
                return m.WithReturnType(freeType);
            }

            var nextType = QualifiedName(
                IdentifierName("System"),
                GenericName(
                        Identifier("Func"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] {m.ReturnType, Token(SyntaxKind.CommaToken), freeType}))));

            if (failType == null)
            {
                return m.WithParameterList(m.ParameterList
                        .AddParameters(
                            Parameter(Identifier("next"))
                                .WithType(nextType)))
                    .WithReturnType(freeType);
            }

            var failNextType = QualifiedName(
                IdentifierName("System"),
                GenericName(
                        Identifier("Func"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] {failType, Token(SyntaxKind.CommaToken), freeType}))));

            return m.WithParameterList(m.ParameterList
                    .AddParameters(
                        Parameter(Identifier("next"))
                            .WithType(nextType))
                    .AddParameters(
                        Parameter(Identifier("failNext"))
                            .WithType(failNextType))
                )
                .WithReturnType(freeType);
        }

        static ClassDeclarationSyntax MakeStaticClass(
            SyntaxToken applyToIdentifier,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            MethodDeclarationSyntax pure,
            MethodDeclarationSyntax fail,
            TypeSyntax failType,
            bool mapIsStatic
        )
        {
            var name = applyToIdentifier;
            var @class = ClassDeclaration(name)
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.StaticKeyword),
                        Token(SyntaxKind.PartialKeyword)));

            var returnType = ParseTypeName($"{applyToIdentifier}{applyToTypeParams}");

            var cases = applyToMembers
                .Select(m => MakeCaseCtorFunction
                (
                    applyToIdentifier: applyToIdentifier,
                    applyToTypeParams: applyToTypeParams,
                    applyToConstraints: applyToConstraints,
                    returnType: returnType,
                    method: m,
                    pure: pure,
                    fail: fail
                ))
                .ToArray();

            var bind = MakeBindFunction(applyToIdentifier, applyToMembers, applyToTypeParams, pure, fail, mapIsStatic);

            var map = mapIsStatic
                ? MakeMapExtension
                (
                    applyToIdentifier: applyToIdentifier,
                    applyToTypeParams: applyToTypeParams
                )
                : MakeMapFunction
                (
                    applyToIdentifier: applyToIdentifier,
                    applyToMembers: applyToMembers,
                    applyToTypeParams: applyToTypeParams,
                    pure: pure,
                    fail: fail
                );

            var monad = AddMonadDefaults(applyToIdentifier, applyToMembers, applyToTypeParams, applyToConstraints);

            if (fail == null)
                return @class.WithMembers(List(cases).Add(bind).Add(map).AddRange(monad));

            var biBind = MakeBiBindFunction
            (
                applyToIdentifier: applyToIdentifier,
                applyToMembers: applyToMembers,
                applyToTypeParams: applyToTypeParams,
                pure: pure,
                fail: fail,
                failType: failType,
                mapIsStatic: mapIsStatic
            );

            var bimap = mapIsStatic
                ? MakeBiMapExtension
                (
                    applyToIdentifier: applyToIdentifier,
                    applyToTypeParams: applyToTypeParams,
                    failType: failType
                )
                : MakeBiMapFunction
                (
                    applyToIdentifier: applyToIdentifier,
                    applyToMembers: applyToMembers,
                    applyToTypeParams: applyToTypeParams,
                    pure: pure,
                    fail: fail,
                    failType: failType
                );

            return @class.WithMembers(List(cases).Add(bind).Add(biBind).Add(map).Add(bimap).AddRange(monad));
        }

        static TypeSyntax MakeTypeName(string ident, string gen) =>
            ParseTypeName($"{ident}<{gen}>");

        static MethodDeclarationSyntax MakeBindFunction(
            SyntaxToken applyToIdentifier,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            MethodDeclarationSyntax pure,
            MethodDeclarationSyntax fail,
            bool mapIsStatic)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);

            var typeA = MakeTypeName(applyToIdentifier.Text, genA);
            var typeB = MakeTypeName(applyToIdentifier.Text, genB);
            var typeC = MakeTypeName(applyToIdentifier.Text, genC);
            var bindFunc = ParseTypeName($"System.Func<{genA}, {typeB}>");

            var mapNextFunc = mapIsStatic
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
                                                            SingletonSeparatedList(
                                                                Argument(
                                                                    IdentifierName("n")))))),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                ParenthesizedExpression(
                                                    IdentifierName("f")))
                                        }))),
                        IdentifierName("Flatten")))
                : InvocationExpression(IdentifierName("Flatten")).WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
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
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                IdentifierName("n"))))),
                                            IdentifierName("Map")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList(
                                                Argument(
                                                    IdentifierName("f")))))))));


            // this is only used if we have a fail path
            var mapFailFunc = mapIsStatic
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
                                                            IdentifierName("FailNext")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList(
                                                                Argument(
                                                                    IdentifierName("fn")))))),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                ParenthesizedExpression(
                                                    IdentifierName("f")))
                                        }))),
                        IdentifierName("Flatten")))
                : InvocationExpression(IdentifierName("Flatten")).WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("v"),
                                                        IdentifierName("FailNext")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                IdentifierName("fn"))))),
                                            IdentifierName("Map")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList(
                                                Argument(
                                                    IdentifierName("f")))))))));

            var pureFunc = new SyntaxNodeOrToken[]
            {
                SwitchExpressionArm(
                    DeclarationPattern(
                        ParseTypeName($"{pure.Identifier.Text}<{genA}>"),
                        SingleVariableDesignation(Identifier("v"))),
                    InvocationExpression(IdentifierName("f"))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("v"),
                                            IdentifierName(CodeGenUtil.MakeFirstCharUpper(pure.ParameterList.Parameters
                                                .First().Identifier.Text)))))))),
                Token(SyntaxKind.CommaToken)
            };

            // this will only be used if we have a fail path
            var failFunc = fail != null
                ? new SyntaxNodeOrToken[]
                {
                    SwitchExpressionArm(
                        DeclarationPattern(
                            MakeTypeName(fail.Identifier.Text, genA),
                            SingleVariableDesignation(Identifier("v"))),
                        ObjectCreationExpression(MakeTypeName(fail.Identifier.Text, genB))
                            .WithArgumentList(
                                fail.ParameterList.Parameters.Count == 0
                                    ? ArgumentList()
                                    : ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("v"),
                                                    IdentifierName(
                                                        CodeGenUtil.MakeFirstCharUpper(fail.ParameterList
                                                            .Parameters.First().Identifier.Text)))))))),
                    Token(SyntaxKind.CommaToken)
                }
                : null;


            var termimalFuncs = applyToMembers
                .Where(m => m != pure)
                .Where(m => m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
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
                                        SeparatedList(
                                            m.ParameterList
                                                .Parameters
                                                .Select(p =>
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("v"),
                                                            IdentifierName(
                                                                CodeGenUtil.MakeFirstCharUpper(p.Identifier))))))))),
                        Token(SyntaxKind.CommaToken)
                    });


            var freeFuncParams = fail == null
                ? new SyntaxNodeOrToken[] {Argument(SimpleLambdaExpression(Parameter(Identifier("n")), mapNextFunc))}
                : new SyntaxNodeOrToken[]
                {
                    Argument(SimpleLambdaExpression(Parameter(Identifier("n")), mapNextFunc)),
                    Token(SyntaxKind.CommaToken),
                    Argument(SimpleLambdaExpression(Parameter(Identifier("fn")), mapFailFunc))
                };

            var freeFuncParamsCount = fail == null ? 1 : 2;

            var freeFuncs = applyToMembers
                .Where(m => !m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
                .Where(m => !m.AttributeLists.SelectMany(a => a.Attributes).Any(IsFailAttr))
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
                                                .Take(m.ParameterList.Parameters.Count - freeFuncParamsCount)
                                                .SelectMany(p =>
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("v"),
                                                                IdentifierName(
                                                                    CodeGenUtil.MakeFirstCharUpper(p.Identifier
                                                                        .Text)))),
                                                        Token(SyntaxKind.CommaToken)
                                                    })
                                                .Concat(freeFuncParams))))),
                        Token(SyntaxKind.CommaToken)
                    });

            var tokens = new List<SyntaxNodeOrToken>();
            tokens.AddRange(pureFunc);
            if (failFunc != null)
                tokens.AddRange(failFunc);
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

        static MethodDeclarationSyntax MakeBiBindFunction(
            SyntaxToken applyToIdentifier,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            MethodDeclarationSyntax pure,
            MethodDeclarationSyntax fail,
            TypeSyntax failType,
            bool mapIsStatic)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);

            var typeA = MakeTypeName(applyToIdentifier.Text, genA);
            var typeB = MakeTypeName(applyToIdentifier.Text, genB);
            var typeC = MakeTypeName(applyToIdentifier.Text, genC);
            var bindFunc = ParseTypeName($"System.Func<{genA}, {typeB}>");
            var bindFailFuncType = failType != null
                ? ParseTypeName($"System.Func<{failType}, {typeB}>")
                : ParseTypeName($"System.Func<{typeB}>");

            var pureTypeA = MakeTypeName(pure.Identifier.Text, genA);
            var failTypeA = MakeTypeName(fail.Identifier.Text, genA);

            var mapFunc = mapIsStatic
                ? InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    typeA,
                                    IdentifierName("BiMap")))
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
                                                            SingletonSeparatedList(
                                                                Argument(
                                                                    IdentifierName("n")))))),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                ParenthesizedExpression(
                                                    IdentifierName("Succ"))),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                ParenthesizedExpression(
                                                    IdentifierName("Fail")))
                                        }))),
                        IdentifierName("Flatten")))
                : InvocationExpression(IdentifierName("Flatten")).WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
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
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                IdentifierName("n"))))),
                                            IdentifierName("BiMap")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]
                                                {
                                                    Argument(IdentifierName("Succ")), Token(SyntaxKind.CommaToken),
                                                    Argument(IdentifierName("Fail"))
                                                }
                                            )))))));


            var mapFailFunc = mapIsStatic
                ? InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    typeA,
                                    IdentifierName("BiMap")))
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
                                                            IdentifierName("FailNext")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList(
                                                                Argument(
                                                                    IdentifierName("fn")))))),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                ParenthesizedExpression(
                                                    IdentifierName("Succ"))),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                ParenthesizedExpression(
                                                    IdentifierName("Fail")))
                                        }))),
                        IdentifierName("Flatten")))
                : InvocationExpression(IdentifierName("Flatten")).WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("v"),
                                                        IdentifierName("FailNext")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                IdentifierName("fn"))))),
                                            IdentifierName("BiMap")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]
                                                {
                                                    Argument(IdentifierName("Succ")), Token(SyntaxKind.CommaToken),
                                                    Argument(IdentifierName("Fail"))
                                                }
                                            )))))));

            var pureFunc = new SyntaxNodeOrToken[]
            {
                SwitchExpressionArm(
                    DeclarationPattern(
                        pureTypeA,
                        SingleVariableDesignation(Identifier("v"))),
                    InvocationExpression(IdentifierName("Succ"))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("v"),
                                            IdentifierName(CodeGenUtil.MakeFirstCharUpper(pure.ParameterList.Parameters
                                                .First().Identifier.Text)))))))),
                Token(SyntaxKind.CommaToken)
            };

            var failFunc = new SyntaxNodeOrToken[]
            {
                SwitchExpressionArm(
                    DeclarationPattern(
                        failTypeA,
                        SingleVariableDesignation(Identifier("v"))),
                    InvocationExpression(IdentifierName("Fail"))
                        .WithArgumentList(
                            fail.ParameterList.Parameters.Count == 0
                                ? ArgumentList()
                                : ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("v"),
                                                IdentifierName(CodeGenUtil.MakeFirstCharUpper(fail.ParameterList
                                                    .Parameters
                                                    .First().Identifier.Text)))))))),
                Token(SyntaxKind.CommaToken)
            };

            var termimalFuncs = applyToMembers
                .Where(m => m != pure)
                .Where(m => m != fail)
                .Where(m => m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
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
                                        SeparatedList(
                                            m.ParameterList
                                                .Parameters
                                                .Select(p =>
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("v"),
                                                            IdentifierName(
                                                                CodeGenUtil.MakeFirstCharUpper(p.Identifier))))))))),
                        Token(SyntaxKind.CommaToken)
                    });


            var freeFuncs = applyToMembers
                .Where(m => m != fail)
                .Where(m => !m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
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
                                                .Take(m.ParameterList.Parameters.Count - 2)
                                                .SelectMany(p =>
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("v"),
                                                                IdentifierName(
                                                                    CodeGenUtil.MakeFirstCharUpper(
                                                                        p.Identifier.Text)))),
                                                        Token(SyntaxKind.CommaToken)
                                                    })
                                                .Concat(new SyntaxNodeOrToken[]
                                                {
                                                    Argument(SimpleLambdaExpression(Parameter(Identifier("n")),
                                                        mapFunc)),
                                                    Token(SyntaxKind.CommaToken), Argument(SimpleLambdaExpression(
                                                        Parameter(Identifier("fn")),
                                                        mapFailFunc))
                                                })
                                        )))),
                        Token(SyntaxKind.CommaToken)
                    });

            var tokens = new List<SyntaxNodeOrToken>();
            tokens.AddRange(pureFunc);
            tokens.AddRange(failFunc);
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

            return MethodDeclaration(typeB, Identifier("BiBind"))
                .WithModifiers(
                    TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
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
                                        Identifier("Succ"))
                                    .WithType(bindFunc),
                                Token(SyntaxKind.CommaToken), Parameter(
                                        Identifier("Fail"))
                                    .WithType(bindFailFuncType)
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
            TypeParameterListSyntax applyToTypeParams)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var typeA = MakeTypeName(applyToIdentifier.Text, genA);
            var typeB = MakeTypeName(applyToIdentifier.Text, genB);
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
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            MethodDeclarationSyntax pure,
            MethodDeclarationSyntax fail)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);

            var typeA = MakeTypeName(applyToIdentifier.Text, genA);
            var typeB = MakeTypeName(applyToIdentifier.Text, genB);
            var typeC = MakeTypeName(applyToIdentifier.Text, genC);
            var mapFuncType = ParseTypeName($"System.Func<{genA}, {genB}>");
            var pureTypeA = MakeTypeName(pure.Identifier.Text, genA);
            var pureTypeB = MakeTypeName(pure.Identifier.Text, genB);

            var mapNextFunc = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("v"),
                                    IdentifierName("Next")))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("n"))))),
                        IdentifierName("Map")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                IdentifierName("f")))));

            // this will only be used if we have a fail path
            var mapFailFunc = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("v"),
                                    IdentifierName("FailNext")))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("fn"))))),
                        IdentifierName("Map")))
                .WithArgumentList(
                    ArgumentList(
                        SingletonSeparatedList(
                            Argument(
                                IdentifierName("f")))));

            var pureFunc = new SyntaxNodeOrToken[]
            {
                SwitchExpressionArm(
                    DeclarationPattern(
                        pureTypeA,
                        SingleVariableDesignation(Identifier("v"))),
                    ObjectCreationExpression(pureTypeB)
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        InvocationExpression(IdentifierName("f"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("v"),
                                                                IdentifierName(
                                                                    CodeGenUtil.MakeFirstCharUpper(pure.ParameterList
                                                                        .Parameters.First().Identifier.Text)))))))))))),
                Token(SyntaxKind.CommaToken)
            };

            // this will only be used if we have a fail path
            var failFunc = fail != null
                ? new SyntaxNodeOrToken[]
                {
                    SwitchExpressionArm(
                        DeclarationPattern(
                            MakeTypeName(fail.Identifier.Text, genA),
                            SingleVariableDesignation(Identifier("v"))),
                        ObjectCreationExpression(MakeTypeName(fail.Identifier.Text, genB))
                            .WithArgumentList(
                                fail.ParameterList.Parameters.Count == 0
                                    ? ArgumentList()
                                    : ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("v"),
                                                    IdentifierName(
                                                        CodeGenUtil.MakeFirstCharUpper(fail.ParameterList
                                                            .Parameters.First().Identifier.Text)))))))),
                    Token(SyntaxKind.CommaToken)
                }
                : null;

            var termimalFuncs = applyToMembers
                .Where(m => m != pure)
                .Where(m => m != fail)
                .Where(m => m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
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
                                        SeparatedList(
                                            m.ParameterList
                                                .Parameters
                                                .Select(p =>
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("v"),
                                                            IdentifierName(
                                                                CodeGenUtil.MakeFirstCharUpper(p.Identifier))))))))),
                        Token(SyntaxKind.CommaToken)
                    });


            var freeFuncParams = fail == null
                ? new SyntaxNodeOrToken[] {Argument(SimpleLambdaExpression(Parameter(Identifier("n")), mapNextFunc))}
                : new SyntaxNodeOrToken[]
                {
                    Argument(SimpleLambdaExpression(Parameter(Identifier("n")), mapNextFunc)),
                    Token(SyntaxKind.CommaToken),
                    Argument(SimpleLambdaExpression(Parameter(Identifier("fn")), mapFailFunc))
                };

            var freeFuncParamsCount = fail == null ? 1 : 2;

            var freeFuncs = applyToMembers
                .Where(m => !m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
                .Where(m => !m.AttributeLists.SelectMany(a => a.Attributes).Any(IsFailAttr))
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
                                                .Take(m.ParameterList.Parameters.Count - freeFuncParamsCount)
                                                .SelectMany(p =>
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("v"),
                                                                IdentifierName(
                                                                    CodeGenUtil.MakeFirstCharUpper(
                                                                        p.Identifier.Text)))),
                                                        Token(SyntaxKind.CommaToken)
                                                    })
                                                .Concat(freeFuncParams))))),
                        Token(SyntaxKind.CommaToken)
                    });

            var tokens = new List<SyntaxNodeOrToken>();
            tokens.AddRange(pureFunc);
            if (failFunc != null)
                tokens.AddRange(failFunc);
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
                    TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
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


        static MethodDeclarationSyntax MakeBiMapExtension(
            SyntaxToken applyToIdentifier,
            TypeParameterListSyntax applyToTypeParams,
            TypeSyntax failType)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var typeA = MakeTypeName(applyToIdentifier.Text, genA);
            var typeB = MakeTypeName(applyToIdentifier.Text, genB);
            var mapFuncType = ParseTypeName($"System.Func<{genA}, {genB}>");
            var mapFailFuncType = ParseTypeName($"System.Func<{failType}, {genB}>");

            return MethodDeclaration(
                    typeB,
                    Identifier("BiMap"))
                .WithModifiers(
                    TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
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
                                Token(SyntaxKind.CommaToken), Parameter(Identifier("Succ")).WithType(mapFuncType),
                                Token(SyntaxKind.CommaToken), Parameter(Identifier("Fail")).WithType(mapFailFuncType)
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    typeA,
                                    IdentifierName("BiMap")))
                            .WithArgumentList(
                                ArgumentList(
                                    SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            Argument(
                                                IdentifierName("ma")),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                IdentifierName("Succ")),
                                            Token(SyntaxKind.CommaToken), Argument(
                                                IdentifierName("Fail"))
                                        })))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
        }


        static MethodDeclarationSyntax MakeBiMapFunction(
            SyntaxToken applyToIdentifier,
            MethodDeclarationSyntax[] applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            MethodDeclarationSyntax pure,
            MethodDeclarationSyntax fail,
            TypeSyntax failType)
        {
            var genA = applyToTypeParams.Parameters.First().ToString();
            var genB = CodeGenUtil.NextGenName(genA);
            var genC = CodeGenUtil.NextGenName(genB);

            var typeA = MakeTypeName(applyToIdentifier.Text, genA);
            var typeB = MakeTypeName(applyToIdentifier.Text, genB);
            var typeC = MakeTypeName(applyToIdentifier.Text, genC);
            var mapFuncType = ParseTypeName($"System.Func<{genA}, {genB}>");
            var mapFailFuncType = failType != null
                ? ParseTypeName($"System.Func<{failType}, {genB}>")
                : ParseTypeName($"System.Func<{genB}>");
            var pureTypeA = MakeTypeName(pure.Identifier.Text, genA);
            var pureTypeB = MakeTypeName(pure.Identifier.Text, genB);
            var failTypeA = MakeTypeName(fail.Identifier.Text, genA);


            var mapNextFunc = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("v"),
                                    IdentifierName("Next")))
                            .WithArgumentList(
                                ArgumentList(SingletonSeparatedList(Argument(IdentifierName("n"))))),
                        IdentifierName("BiMap")))
                .WithArgumentList(
                    ArgumentList(
                        SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Argument(IdentifierName("Succ")), Token(SyntaxKind.CommaToken),
                                Argument(IdentifierName("Fail"))
                            }
                        )));

            var mapFailFunc = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("v"),
                                    IdentifierName("FailNext")))
                            .WithArgumentList(
                                ArgumentList(SingletonSeparatedList(Argument(IdentifierName("fn"))))),
                        IdentifierName("BiMap")))
                .WithArgumentList(
                    ArgumentList(
                        SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Argument(IdentifierName("Succ")), Token(SyntaxKind.CommaToken),
                                Argument(IdentifierName("Fail"))
                            }
                        )));

            var pureSuccFunc = new SyntaxNodeOrToken[]
            {
                SwitchExpressionArm(
                    DeclarationPattern(
                        pureTypeA,
                        SingleVariableDesignation(Identifier("v"))),
                    ObjectCreationExpression(pureTypeB)
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        InvocationExpression(IdentifierName("Succ"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("v"),
                                                                IdentifierName(
                                                                    CodeGenUtil.MakeFirstCharUpper(pure.ParameterList
                                                                        .Parameters.First().Identifier.Text)))))))))))),
                Token(SyntaxKind.CommaToken)
            };

            var pureFailFunc = new SyntaxNodeOrToken[]
            {
                SwitchExpressionArm(
                    DeclarationPattern(
                        failTypeA,
                        SingleVariableDesignation(Identifier("v"))),
                    ObjectCreationExpression(pureTypeB)
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        InvocationExpression(IdentifierName("Fail"))
                                            .WithArgumentList(
                                                fail.ParameterList.Parameters.Count == 0
                                                    ? ArgumentList()
                                                    : ArgumentList(
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName("v"),
                                                                    IdentifierName(
                                                                        CodeGenUtil.MakeFirstCharUpper(fail
                                                                            .ParameterList
                                                                            .Parameters.First().Identifier
                                                                            .Text)))))))))))),
                Token(SyntaxKind.CommaToken)
            };


            var termimalFuncs = applyToMembers
                .Where(m => m != pure)
                .Where(m => m != fail)
                .Where(m => m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
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
                                        SeparatedList(
                                            m.ParameterList
                                                .Parameters
                                                .Select(p =>
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("v"),
                                                            IdentifierName(
                                                                CodeGenUtil.MakeFirstCharUpper(p.Identifier))))))))),
                        Token(SyntaxKind.CommaToken)
                    });


            var freeFuncs = applyToMembers
                .Where(m => !m.AttributeLists.SelectMany(a => a.Attributes).Any(IsPureAttr))
                .Where(m => !m.AttributeLists.SelectMany(a => a.Attributes).Any(IsFailAttr))
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
                                                .Take(m.ParameterList.Parameters.Count - 2)
                                                .SelectMany(p =>
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("v"),
                                                                IdentifierName(
                                                                    CodeGenUtil.MakeFirstCharUpper(
                                                                        p.Identifier.Text)))),
                                                        Token(SyntaxKind.CommaToken)
                                                    })
                                                .Concat(new SyntaxNodeOrToken[]
                                                {
                                                    Argument(SimpleLambdaExpression(Parameter(Identifier("n")),
                                                        mapNextFunc)),
                                                    Token(SyntaxKind.CommaToken), Argument(SimpleLambdaExpression(
                                                        Parameter(Identifier("fn")),
                                                        mapFailFunc))
                                                })
                                        )))),
                        Token(SyntaxKind.CommaToken)
                    });

            var tokens = new List<SyntaxNodeOrToken>();
            tokens.AddRange(pureSuccFunc);
            tokens.AddRange(pureFailFunc);
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

            return MethodDeclaration(typeB, Identifier("BiMap"))
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
                                Parameter(Identifier("ma"))
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.ThisKeyword)))
                                    .WithType(typeA),
                                Token(SyntaxKind.CommaToken), Parameter(Identifier("Succ"))
                                    .WithType(mapFuncType),
                                Token(SyntaxKind.CommaToken), Parameter(Identifier("Fail"))
                                    .WithType(mapFailFuncType)
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
            MethodDeclarationSyntax pure,
            MethodDeclarationSyntax fail)
        {
            if (HasPureAttr(method) || HasFailAttr(method))
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
                        TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                    .WithParameterList(
                        method.ParameterList)
                    .WithConstraintClauses(applyToConstraints)
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            ObjectCreationExpression(thisType)
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(args)))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken));

                if (typeParamList != null)
                {
                    @case = @case.WithTypeParameterList(typeParamList);
                }

                return @case;
            }
            else
            {
                var skip = fail != null ? 1 : 0;

                var nextType = ((GenericNameSyntax)((QualifiedNameSyntax)method.ParameterList.Parameters
                            .Reverse()
                            .Skip(skip)
                            .First()
                            .Type)
                        .Right)
                    .TypeArgumentList
                    .Arguments
                    .First();

                var thisType = ParseTypeName($"{method.Identifier.Text}<{nextType}>");
                returnType = ParseTypeName($"{applyToIdentifier}<{nextType}>");

                var paramIdents = method.ParameterList
                    .Parameters
                    .Select(p => (SyntaxNodeOrToken)Argument(IdentifierName(p.Identifier.Text)))
                    .ToArray();

                //var nextType = QualifiedName(
                //    IdentifierName("System"),
                //    GenericName(
                //            Identifier("Func"))
                //        .WithTypeArgumentList(
                //            TypeArgumentList(
                //                SeparatedList<TypeSyntax>(
                //                    new SyntaxNodeOrToken[] { m.ReturnType, Token(SyntaxKind.CommaToken), freeType }))));

                if (fail != null)
                {
                    paramIdents[paramIdents.Length - 1] = Argument(GenericName(fail.Identifier.Text)
                        .WithTypeArgumentList(
                            TypeArgumentList(SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[] {nextType}))));
                    paramIdents[paramIdents.Length - 2] = Argument(IdentifierName(pure.Identifier.Text));
                }
                else
                {
                    paramIdents[paramIdents.Length - 1] = Argument(IdentifierName(pure.Identifier.Text));
                }

                var args = CodeGenUtil.Interleave(
                    paramIdents,
                    Token(SyntaxKind.CommaToken));

                var parameters = fail != null
                    ? method.ParameterList.Parameters
                        .RemoveAt(method.ParameterList.Parameters.Count - 1)
                        .RemoveAt(method.ParameterList.Parameters.Count - 2)
                    : method.ParameterList.Parameters
                        .RemoveAt(method.ParameterList.Parameters.Count - 1);


                var @case = MethodDeclaration(returnType, method.Identifier)
                    .WithModifiers(
                        TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                    .WithParameterList(
                        method.ParameterList.WithParameters(parameters))
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
