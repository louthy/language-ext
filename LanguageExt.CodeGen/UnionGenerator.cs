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
    public class UnionGenerator : ICodeGenerator
    {
        public UnionGenerator(AttributeData attributeData)
        {
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
            TransformationContext context,
            IProgress<Diagnostic> progress,
            CancellationToken cancellationToken)
        {
            if (context.ProcessingNode is InterfaceDeclarationSyntax applyTo)
            {
                if(!AllMembersReturnInterface(applyTo.Identifier, applyTo.Members, applyTo.TypeParameterList, context, progress))
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }

                var caseMembers = applyTo.Members
                                         .OfType<MethodDeclarationSyntax>()
                                         .Where(m => !m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.StaticKeyword)))
                                         .ToArray();


                var caseRes = caseMembers
                                   .Zip(Enumerable.Range(1, Int32.MaxValue), (m, i) => (m, i))
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
                                                    includeWithAndLenses: true,
                                                    includeMatch: true,
                                                    m.i))
                                   .ToList();

                var ok         = caseRes.All(x => x.Success);
                var cases      = caseRes.Select(c => c.Type);
                var includeMap = caseMembers.All(m => m.TypeParameterList == null || m.TypeParameterList.Parameters.Count == 0);

                var staticCtorClass = MakeStaticConstructorClass(
                    applyTo.Identifier, 
                    applyTo.Members,
                    applyTo.TypeParameterList, 
                    applyTo.ConstraintClauses,
                    CodeGenUtil.VisibilityModifier(applyTo.Modifiers),
                    includeMatch: true,
                    includeMap: includeMap);

                if (ok)
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>().AddRange(cases).Add(staticCtorClass));
                }
                else
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }
            }
            else if (context.ProcessingNode is ClassDeclarationSyntax applyToClass)
            {
                if(!applyToClass.Modifiers.Where(m => m.IsKind(SyntaxKind.PartialKeyword)).Any())
                {
                    CodeGenUtil.ReportError($"Type can't be made into a union.  It must be an abstract partial class or an interface", "Union Code-Gen", context.ProcessingNode, progress);
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }
                if (!applyToClass.Modifiers.Where(m => m.IsKind(SyntaxKind.AbstractKeyword)).Any())
                {
                    CodeGenUtil.ReportError($"Type can't be made into a union.  It must be an abstract partial class or an interface", "Union Code-Gen", context.ProcessingNode, progress);
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }

                if (!AllMembersReturnInterface(applyToClass.Identifier, applyToClass.Members, applyToClass.TypeParameterList, context, progress))
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }

                var methods = applyToClass.Members
                                          .OfType<MethodDeclarationSyntax>()
                                          .Where(NotOverloadableMethod)
                                          .ToList();

                var caseRes = methods.Zip(Enumerable.Range(1, Int32.MaxValue), (m, i) => (m, i))
                                     .Select(m => CodeGenUtil.MakeCaseType(
                                                 context,
                                                 progress,
                                                 applyToClass.Identifier,
                                                 applyToClass.Members,
                                                 applyToClass.TypeParameterList,
                                                 applyToClass.Modifiers,
                                                 applyToClass.ConstraintClauses,
                                                 m.m.Identifier,
                                                 m.m.TypeParameterList,
                                                 m.m.ParameterList
                                                  .Parameters
                                                  .Select(p => (p.Identifier, p.Type, p.Modifiers, p.AttributeLists))
                                                  .ToList(),
                                                 BaseSpec.Abstract,
                                                 caseIsClass: true,
                                                 caseIsPartial: false,
                                                 includeWithAndLenses: true,
                                                 includeMatch: true,
                                                 m.i))
                                     .ToList();

                var ok           = caseRes.All(x => x.Success);
                var cases        = caseRes.Select(c => c.Type);
                var includeMatch = methods.All(m => m.TypeParameterList == null || m.TypeParameterList.Parameters.Count == 0);
                var includeMap   = includeMatch;

                var staticCtorClass = MakeStaticConstructorClass(
                    applyToClass.Identifier, 
                    applyToClass.Members, 
                    applyToClass.TypeParameterList, 
                    applyToClass.ConstraintClauses,
                    CodeGenUtil.VisibilityModifier(applyToClass.Modifiers),
                    includeMatch: !includeMatch,
                    includeMap: includeMap);

                var partialClass = MakeAbstractClass(applyToClass);
                if (includeMatch)
                {
                    partialClass = partialClass.AddMembers(MakeAbstractMatch(methods, applyToClass.TypeParameterList));
                }

                var unionBase = MakeBaseFromAbstractClass(applyToClass);

                if (ok)
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>().AddRange(cases)
                                                                          .Add(staticCtorClass)
                                                                          .Add(partialClass)
                                                                          .Add(unionBase));
                }
                else
                {
                    return Task.FromResult(List<MemberDeclarationSyntax>());
                }
            }
            else
            {
                CodeGenUtil.ReportError($"Type can't be made into a union.  It must be an interface/abstract class", "Union Code-Gen", context.ProcessingNode, progress);
                return Task.FromResult(List<MemberDeclarationSyntax>());
            }
        }

        bool NotOverloadableMethod(MethodDeclarationSyntax method)
        {
            var name = method.Identifier.Text;
            return !(name == "ToString" || name == "GetHashCode" || name == "Map");
        }

        static bool AllMembersReturnInterface(
            SyntaxToken applyToIdentifier,
            SyntaxList<MemberDeclarationSyntax> applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            TransformationContext context,
            IProgress<Diagnostic> progress
            )
        {
            bool allMethods = true;
            foreach(var nonMethod in applyToMembers.Where(m => !(m is MethodDeclarationSyntax)))
            {
                allMethods = false;
                CodeGenUtil.ReportError($"Type can't contain anything other than methods if you want to make it into a union type.", "Union Code-Gen", nonMethod, progress);
            }

            if(!allMethods)
            {
                return false;
            }

            var returnType = $"{applyToIdentifier}{applyToTypeParams}";

            bool returnsTypesOk = true;
            foreach(var method in applyToMembers.Select(m => m as MethodDeclarationSyntax))
            {
                if(method.ReturnType.ToString() != returnType)
                {
                    CodeGenUtil.ReportError($"Methods in union types must return the same type as the interface/abstract class they're defined in. ({method.ReturnType} != {returnType})", "Union Code-Gen", method, progress);
                    returnsTypesOk = false;
                }
            }
            return returnsTypesOk;
        }

        static MemberDeclarationSyntax[] MakeAbstractMatch(
            List<MethodDeclarationSyntax> methods, 
            TypeParameterListSyntax applyToTypeParams)
        {
            applyToTypeParams ??= TypeParameterList();
            
            var allGens = methods.Aggregate(TypeParameterList(),
                                            (s, m) =>
                                                m.TypeParameterList == null || m.TypeParameterList.Parameters.Count == 0
                                                    ? s
                                                    : m.TypeParameterList.Parameters.Aggregate(
                                                        s,
                                                        (s1, p) => s1.AddParameters(TypeParameter(Identifier($"{m.Identifier}_{p.Identifier}")))))
                                 .AddParameters(TypeParameter(Identifier("RETURN")));

            TypeParameterListSyntax funParams(MethodDeclarationSyntax m)
            {
                var ps = m.TypeParameterList == null || m.TypeParameterList.Parameters.Count == 0
                    ? applyToTypeParams
                    : m.TypeParameterList.Parameters.Aggregate(
                        applyToTypeParams,
                        (s, p) => s.AddParameters(TypeParameter(Identifier($"{m.Identifier}_{p.Identifier}"))));

                return ps == null
                           ? ps
                           : ps.Parameters.Count == 0
                               ? null
                               : ps;
            }

            var fnParams = methods.Select(m =>
                                              Parameter(m.Identifier)
                                                 .WithType(
                                                      QualifiedName(
                                                          IdentifierName("System"),
                                                          GenericName(
                                                                  Identifier("Func"))
                                                             .WithTypeArgumentList(
                                                                  TypeArgumentList(
                                                                      SeparatedList<TypeSyntax>(
                                                                          new SyntaxNodeOrToken[]
                                                                          {
                                                                              ParseTypeName($"{m.Identifier.Text}{funParams(m)}"), 
                                                                              Token(SyntaxKind.CommaToken), IdentifierName("RETURN")
                                                                          }))))))
                                  .ToList();

            var fnParams1 = CodeGenUtil.Interleave(fnParams.Select(x => (SyntaxNodeOrToken)x).ToArray(), Token(SyntaxKind.CommaToken));

            var method = MethodDeclaration(
                             IdentifierName("RETURN"),
                             Identifier("Match"))
                        .WithModifiers(
                             TokenList(
                                 new[] {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AbstractKeyword)}))
                        .WithTypeParameterList(allGens)
                        .WithParameterList(
                             ParameterList(
                                 SeparatedList<ParameterSyntax>(fnParams1)))
                        .WithSemicolonToken(
                             Token(SyntaxKind.SemicolonToken));

            return new[] {method};
        }

        static ClassDeclarationSyntax MakeStaticConstructorClass(
            SyntaxToken applyToIdentifier,
            SyntaxList<MemberDeclarationSyntax> applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            SyntaxToken visibilityModifier,
            bool includeMatch,
            bool includeMap
            )
        {
            var name = applyToIdentifier;
            if(applyToTypeParams == null)
            {
                name = Identifier($"{name}Con");
            }

            var @class = ClassDeclaration(name)
                            .WithModifiers(
                                TokenList(new[] { visibilityModifier, Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword) }));

            var returnType = ParseTypeName($"{applyToIdentifier}{applyToTypeParams}");

            var methods = applyToMembers
                         .OfType<MethodDeclarationSyntax>()
                         .Where(m => !m.Modifiers.Any(mo => mo.IsKind(SyntaxKind.StaticKeyword)))
                         .ToList();

            var cases = methods.Select(m => MakeCaseCtorFunction(applyToTypeParams, applyToConstraints, returnType, m))
                               .ToList();

            if (includeMatch)
            {
                cases.Add(MakeMatchFunction(applyToTypeParams, applyToConstraints, returnType, methods));
            }

            if (includeMap && applyToTypeParams != null && applyToTypeParams.Parameters.Count == 1)
            {
                cases.Add(CodeGenUtil.MakeMapFunction(0, "Map", applyToIdentifier, methods.ToArray(), applyToTypeParams, null, null, true));
                cases.Add(CodeGenUtil.MakeMapFunction(0, "Select", applyToIdentifier, methods.ToArray(), applyToTypeParams, null, null, true));
            }

            return @class.WithMembers(List(cases));
        }

        static MethodDeclarationSyntax MakeMatchFunction(
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            TypeSyntax returnType,
            List<MethodDeclarationSyntax> methods)
        {
            var maParam = Parameter(Identifier("_ma"))
                         .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                         .WithType(returnType);

            applyToTypeParams ??= TypeParameterList();
            
            var allGens = methods.Aggregate(applyToTypeParams,
                                            (s, m) =>
                                                m.TypeParameterList == null || m.TypeParameterList.Parameters.Count == 0
                                                    ? s
                                                    : m.TypeParameterList.Parameters.Aggregate(
                                                        s,
                                                        (s1, p) => s1.AddParameters(TypeParameter(Identifier($"{m.Identifier}_{p.Identifier}")))))
                                 .AddParameters(TypeParameter(Identifier("RETURN")));

            TypeParameterListSyntax funParams(MethodDeclarationSyntax m)
            {
                var ps = m.TypeParameterList == null || m.TypeParameterList.Parameters.Count == 0
                    ? applyToTypeParams
                    : m.TypeParameterList.Parameters.Aggregate(
                        applyToTypeParams,
                        (s, p) => s.AddParameters(TypeParameter(Identifier($"{m.Identifier}_{p.Identifier}"))));

                return ps == null
                           ? ps
                           : ps.Parameters.Count == 0
                               ? null
                               : ps;
            }

            var fnParams = methods.Select(m =>
                                              Parameter(m.Identifier)
                                                 .WithType(
                                                      QualifiedName(
                                                          IdentifierName("System"),
                                                          GenericName(
                                                                  Identifier("Func"))
                                                             .WithTypeArgumentList(
                                                                  TypeArgumentList(
                                                                      SeparatedList<TypeSyntax>(
                                                                          new SyntaxNodeOrToken[]
                                                                          {
                                                                              ParseTypeName($"{m.Identifier.Text}{funParams(m)}"), 
                                                                              Token(SyntaxKind.CommaToken), IdentifierName("RETURN")
                                                                          }))))))
                                  .ToList();

            var otherwise = SwitchExpressionArm(
                DiscardPattern(),
                ThrowExpression(
                    ObjectCreationExpression(
                            QualifiedName(
                                IdentifierName("LanguageExt"),
                                IdentifierName("ValueIsNullException")))
                       .WithArgumentList(
                            ArgumentList())));

            var matches = methods.Select(m => SwitchExpressionArm(
                                             DeclarationPattern(
                                                 ParseTypeName($"{m.Identifier.Text}{funParams(m)}"),
                                                 SingleVariableDesignation(
                                                     Identifier("value"))),
                                             InvocationExpression(
                                                     IdentifierName(m.Identifier.Text))
                                                .WithArgumentList(
                                                     ArgumentList(
                                                         SingletonSeparatedList<ArgumentSyntax>(
                                                             Argument(
                                                                 IdentifierName("value")))))))
                                 .ToList();

            matches.Add(otherwise);
            var matches1 = CodeGenUtil.Interleave(matches.Select(x => (SyntaxNodeOrToken)x).ToArray(), Token(SyntaxKind.CommaToken));

            fnParams.Insert(0, maParam);
            var fnParams1 = CodeGenUtil.Interleave(fnParams.Select(x => (SyntaxNodeOrToken)x).ToArray(), Token(SyntaxKind.CommaToken));

            var method = MethodDeclaration(
                             IdentifierName("RETURN"),
                             Identifier("Match"))
                        .WithModifiers(
                             TokenList(
                                 new[] {Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)}))
                        .WithConstraintClauses(applyToConstraints)
                        .WithTypeParameterList(allGens)
                        .WithParameterList(
                             ParameterList(
                                 SeparatedList<ParameterSyntax>(fnParams1)))
                        .WithExpressionBody(
                             ArrowExpressionClause(
                                 SwitchExpression(
                                         IdentifierName("_ma"))
                                    .WithArms(
                                         SeparatedList<SwitchExpressionArmSyntax>(matches1))))
                        .WithSemicolonToken(
                             Token(SyntaxKind.SemicolonToken));

            return method;
        }

        static MemberDeclarationSyntax MakeCaseCtorFunction(
            TypeParameterListSyntax applyToTypeParams,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            TypeSyntax returnType, 
            MethodDeclarationSyntax method)
        {
            var typeParamList = applyToTypeParams;
            if (method.TypeParameterList != null)
            {
                typeParamList = typeParamList.AddParameters(method.TypeParameterList.Parameters.ToArray());
            }

            var thisType = ParseTypeName($"{method.Identifier.Text}{typeParamList}");

            var args = CodeGenUtil.Interleave(
                            method.ParameterList
                                  .Parameters
                                  .Select(p => (SyntaxNodeOrToken)Argument(IdentifierName(p.Identifier.Text)))
                                  .ToArray(),
                            Token(SyntaxKind.CommaToken));

            var @case = MethodDeclaration(returnType, method.Identifier)
                        .WithModifiers(
                            TokenList(
                                new[]{
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword)}))
                        .WithParameterList(method.ParameterList)
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

        static ClassDeclarationSyntax MakeAbstractClass(ClassDeclarationSyntax applyTo)
        {
            var returnType = ParseTypeName($"{applyTo.Identifier}{applyTo.TypeParameterList}");
            var thisEquatableType = ParseTypeName($"System.IEquatable<{applyTo.Identifier}{applyTo.TypeParameterList}>");
            var thisComparableType = ParseTypeName($"System.IComparable<{applyTo.Identifier}{applyTo.TypeParameterList}>");
            var comparableType = ParseTypeName($"System.IComparable");
            //var serializableType = ParseTypeName($"System.Runtime.Serialization.ISerializable");

            return ClassDeclaration(applyTo.Identifier)
                            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                    AttributeList(
                                        SingletonSeparatedList<AttributeSyntax>(
                                            Attribute(
                                                QualifiedName(
                                                    IdentifierName("System"),
                                                    IdentifierName("Serializable")))))))
                            .WithTypeParameterList(applyTo.TypeParameterList)
                            .WithModifiers(
                                TokenList(
                                    new[]{
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.AbstractKeyword),
                                    Token(SyntaxKind.PartialKeyword)}))
                            .WithBaseList(
                                BaseList(
                                    SeparatedList<BaseTypeSyntax>(
                                        new SyntaxNodeOrToken[]{
                                        SimpleBaseType(thisEquatableType),
                                        Token(SyntaxKind.CommaToken),
                                        SimpleBaseType(thisComparableType),
                                        Token(SyntaxKind.CommaToken),
                                        SimpleBaseType(comparableType)})))
                            .WithMembers(
                                List<MemberDeclarationSyntax>(
                                    new MemberDeclarationSyntax[]{
                                    PropertyDeclaration(
                                            PredefinedType(
                                                Token(SyntaxKind.IntKeyword)),
                                            Identifier("@Tag"))
                                        .WithModifiers(
                                            TokenList(
                                                new []{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.AbstractKeyword)}))
                                        .WithAccessorList(
                                            AccessorList(
                                                SingletonList<AccessorDeclarationSyntax>(
                                                    AccessorDeclaration(
                                                        SyntaxKind.GetAccessorDeclaration)
                                                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))))),
                                    MethodDeclaration(
                                        PredefinedType(
                                            Token(SyntaxKind.IntKeyword)),
                                        Identifier("CompareTo"))
                                    .WithModifiers(
                                        TokenList(
                                            new []{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.AbstractKeyword)}))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("obj"))
                                                .WithType(
                                                    PredefinedType(
                                                        Token(SyntaxKind.ObjectKeyword))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                    MethodDeclaration(
                                        PredefinedType(
                                            Token(SyntaxKind.IntKeyword)),
                                        Identifier("CompareTo"))
                                    .WithModifiers(
                                        TokenList(
                                            new []{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.AbstractKeyword)}))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("other"))
                                                .WithType(returnType))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                    MethodDeclaration(
                                        PredefinedType(
                                            Token(SyntaxKind.BoolKeyword)),
                                        Identifier("Equals"))
                                    .WithModifiers(
                                        TokenList(
                                            new []{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.AbstractKeyword)}))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("other"))
                                                .WithType(returnType))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                    MethodDeclaration(
                                        PredefinedType(
                                            Token(SyntaxKind.BoolKeyword)),
                                        Identifier("Equals"))
                                    .WithModifiers(
                                        TokenList(
                                            new []{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.OverrideKeyword)}))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList<ParameterSyntax>(
                                                Parameter(
                                                    Identifier("obj"))
                                                .WithType(
                                                    PredefinedType(
                                                        Token(SyntaxKind.ObjectKeyword))))))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                            BinaryExpression(
                                                SyntaxKind.LogicalAndExpression,
                                                IsPatternExpression(
                                                    IdentifierName("obj"),
                                                    DeclarationPattern(
                                                        returnType,
                                                        SingleVariableDesignation(
                                                            Identifier("tobj")))),
                                                InvocationExpression(
                                                    IdentifierName("Equals"))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("tobj"))))))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                    MethodDeclaration(
                                        PredefinedType(
                                            Token(SyntaxKind.IntKeyword)),
                                        Identifier("GetHashCode"))
                                    .WithModifiers(
                                        TokenList(
                                            new []{
                                                Token(SyntaxKind.PublicKeyword),
                                                Token(SyntaxKind.OverrideKeyword)}))
                                    .WithExpressionBody(
                                        ArrowExpressionClause(
                                           ThrowExpression(
                                                ObjectCreationExpression(
                                                    QualifiedName(
                                                        IdentifierName("System"),
                                                        IdentifierName("NotSupportedException")))
                                                .WithArgumentList(ArgumentList()))))
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                    OperatorDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.BoolKeyword)),
                        Token(SyntaxKind.EqualsEqualsToken))
                    .WithModifiers(
                        TokenList(
                            new []{
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)}))
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]{
                                    Parameter(
                                        Identifier("x"))
                                    .WithType(
                                        returnType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(
                                        returnType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            BinaryExpression(
                                SyntaxKind.LogicalOrExpression,
                                InvocationExpression(
                                    IdentifierName("ReferenceEquals"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]{
                                                Argument(
                                                    IdentifierName("x")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                                    IdentifierName("y"))}))),
                                ParenthesizedExpression(
                                    BinaryExpression(
                                        SyntaxKind.CoalesceExpression,
                                        ConditionalAccessExpression(
                                            IdentifierName("x"),
                                            InvocationExpression(
                                                MemberBindingExpression(
                                                    IdentifierName("Equals")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            IdentifierName("y")))))),
                                        LiteralExpression(
                                            SyntaxKind.FalseLiteralExpression))))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken)),
                    OperatorDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.BoolKeyword)),
                        Token(SyntaxKind.ExclamationEqualsToken))
                    .WithModifiers(
                        TokenList(
                            new []{
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)}))
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]{
                                    Parameter(
                                        Identifier("x"))
                                    .WithType(
                                        returnType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(
                                        returnType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            PrefixUnaryExpression(
                                SyntaxKind.LogicalNotExpression,
                                ParenthesizedExpression(
                                    BinaryExpression(
                                        SyntaxKind.EqualsExpression,
                                        IdentifierName("x"),
                                        IdentifierName("y"))))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken)),
                    OperatorDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.BoolKeyword)),
                        Token(SyntaxKind.GreaterThanToken))
                    .WithModifiers(
                        TokenList(
                            new []{
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)}))
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]{
                                    Parameter(
                                        Identifier("x"))
                                    .WithType(
                                        returnType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(
                                        returnType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            BinaryExpression(
                                SyntaxKind.LogicalAndExpression,
                                BinaryExpression(
                                    SyntaxKind.LogicalAndExpression,
                                    PrefixUnaryExpression(
                                        SyntaxKind.LogicalNotExpression,
                                        InvocationExpression(
                                            IdentifierName("ReferenceEquals"))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Argument(
                                                            IdentifierName("x")),
                                                        Token(SyntaxKind.CommaToken),
                                                        Argument(
                                                            IdentifierName("y"))})))),
                                    PrefixUnaryExpression(
                                        SyntaxKind.LogicalNotExpression,
                                        InvocationExpression(
                                            IdentifierName("ReferenceEquals"))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Argument(
                                                            IdentifierName("x")),
                                                        Token(SyntaxKind.CommaToken),
                                                        Argument(
                                                            LiteralExpression(
                                                                SyntaxKind.NullLiteralExpression))}))))),
                                BinaryExpression(
                                    SyntaxKind.GreaterThanExpression,
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("x"),
                                            IdentifierName("CompareTo")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    IdentifierName("y"))))),
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(0))))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken)),
                    OperatorDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.BoolKeyword)),
                        Token(SyntaxKind.LessThanToken))
                    .WithModifiers(
                        TokenList(
                            new []{
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)}))
                    .WithParameterList(
                        ParameterList(
                            SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]{
                                    Parameter(
                                        Identifier("x"))
                                    .WithType(
                                        returnType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(
                                        returnType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            BinaryExpression(
                                SyntaxKind.LogicalAndExpression,
                                PrefixUnaryExpression(
                                    SyntaxKind.LogicalNotExpression,
                                    InvocationExpression(
                                        IdentifierName("ReferenceEquals"))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        IdentifierName("x")),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        IdentifierName("y"))})))),
                                ParenthesizedExpression(
                                    BinaryExpression(
                                        SyntaxKind.LogicalOrExpression,
                                        BinaryExpression(
                                            SyntaxKind.LogicalAndExpression,
                                            InvocationExpression(
                                                IdentifierName("ReferenceEquals"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SeparatedList<ArgumentSyntax>(
                                                        new SyntaxNodeOrToken[]{
                                                            Argument(
                                                                IdentifierName("x")),
                                                            Token(SyntaxKind.CommaToken),
                                                            Argument(
                                                                LiteralExpression(
                                                                    SyntaxKind.NullLiteralExpression))}))),
                                            PrefixUnaryExpression(
                                                SyntaxKind.LogicalNotExpression,
                                                InvocationExpression(
                                                    IdentifierName("ReferenceEquals"))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                Argument(
                                                                    IdentifierName("y")),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    LiteralExpression(
                                                                        SyntaxKind.NullLiteralExpression))}))))),
                                        BinaryExpression(
                                            SyntaxKind.LessThanExpression,
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("x"),
                                                    IdentifierName("CompareTo")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            IdentifierName("y"))))),
                                            LiteralExpression(
                                                SyntaxKind.NumericLiteralExpression,
                                                Literal(0))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken)),
                                        OperatorDeclaration(
                                            PredefinedType(
                                                Token(SyntaxKind.BoolKeyword)),
                                            Token(SyntaxKind.GreaterThanEqualsToken))
                                        .WithModifiers(
                                            TokenList(
                                                new []{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithParameterList(
                                            ParameterList(
                                                SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("x"))
                                                        .WithType(
                                                            returnType),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("y"))
                                                        .WithType(
                                                            returnType)})))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                BinaryExpression(
                                                    SyntaxKind.LogicalOrExpression,
                                                    InvocationExpression(
                                                        IdentifierName("ReferenceEquals"))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                    Argument(
                                                                        IdentifierName("x")),
                                                                    Token(SyntaxKind.CommaToken),
                                                                    Argument(
                                                                        IdentifierName("y"))}))),
                                                    ParenthesizedExpression(
                                                        BinaryExpression(
                                                            SyntaxKind.LogicalAndExpression,
                                                            PrefixUnaryExpression(
                                                                SyntaxKind.LogicalNotExpression,
                                                                InvocationExpression(
                                                                    IdentifierName("ReferenceEquals"))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Argument(
                                                                                    IdentifierName("x")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Argument(
                                                                                    LiteralExpression(
                                                                                        SyntaxKind.NullLiteralExpression))})))),
                                                            BinaryExpression(
                                                                SyntaxKind.GreaterThanOrEqualExpression,
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("x"),
                                                                        IdentifierName("CompareTo")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                            Argument(
                                                                                IdentifierName("y"))))),
                                                                LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    Literal(0))))))))
                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken)),
                                        OperatorDeclaration(
                                            PredefinedType(
                                                Token(SyntaxKind.BoolKeyword)),
                                            Token(SyntaxKind.LessThanEqualsToken))
                                        .WithModifiers(
                                            TokenList(
                                                new []{
                                                    Token(SyntaxKind.PublicKeyword),
                                                    Token(SyntaxKind.StaticKeyword)}))
                                        .WithParameterList(
                                            ParameterList(
                                                SeparatedList<ParameterSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        Parameter(
                                                            Identifier("x"))
                                                        .WithType(
                                                            returnType),
                                                        Token(SyntaxKind.CommaToken),
                                                        Parameter(
                                                            Identifier("y"))
                                                        .WithType(
                                                            returnType)})))
                                        .WithExpressionBody(
                                            ArrowExpressionClause(
                                                BinaryExpression(
                                                    SyntaxKind.LogicalOrExpression,
                                                    InvocationExpression(
                                                        IdentifierName("ReferenceEquals"))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                    Argument(
                                                                        IdentifierName("x")),
                                                                    Token(SyntaxKind.CommaToken),
                                                                    Argument(
                                                                        IdentifierName("y"))}))),
                                                    ParenthesizedExpression(
                                                        BinaryExpression(
                                                            SyntaxKind.LogicalOrExpression,
                                                            BinaryExpression(
                                                                SyntaxKind.LogicalAndExpression,
                                                                InvocationExpression(
                                                                    IdentifierName("ReferenceEquals"))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]{
                                                                                Argument(
                                                                                    IdentifierName("x")),
                                                                                Token(SyntaxKind.CommaToken),
                                                                                Argument(
                                                                                    LiteralExpression(
                                                                                        SyntaxKind.NullLiteralExpression))}))),
                                                                PrefixUnaryExpression(
                                                                    SyntaxKind.LogicalNotExpression,
                                                                    InvocationExpression(
                                                                        IdentifierName("ReferenceEquals"))
                                                                    .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[]{
                                                                                    Argument(
                                                                                        IdentifierName("y")),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        LiteralExpression(
                                                                                            SyntaxKind.NullLiteralExpression))}))))),
                                                            BinaryExpression(
                                                                SyntaxKind.LessThanOrEqualExpression,
                                                                InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("x"),
                                                                        IdentifierName("CompareTo")))
                                                                .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                                            Argument(
                                                                                IdentifierName("y"))))),
                                                                LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    Literal(0))))))))

                                        .WithSemicolonToken(
                                            Token(SyntaxKind.SemicolonToken))}));
        }

        static ClassDeclarationSyntax MakeBaseFromAbstractClass(ClassDeclarationSyntax applyTo)
        {
            var members = applyTo.Members
                             .Where(m => m is MethodDeclarationSyntax)
                             .Select(m => m as MethodDeclarationSyntax)
                             .ToList();

            var thisType = ParseTypeName($"{applyTo.Identifier.Text}{applyTo.TypeParameterList}");

            var methods = members.Select(m =>
                                        MethodDeclaration(
                                                thisType,
                                                Identifier(m.Identifier.Text))
                                            .WithModifiers(
                                                TokenList(
                                                    new[]{
                                                        Token(SyntaxKind.PublicKeyword),
                                                        Token(SyntaxKind.OverrideKeyword)}))
                                            .WithTypeParameterList(m.TypeParameterList)
                                            .WithParameterList(m.ParameterList)
                                            .WithExpressionBody(
                                                ArrowExpressionClause(
                                                    ThrowExpression(
                                                        ObjectCreationExpression(
                                                            QualifiedName(
                                                                IdentifierName("System"),
                                                                IdentifierName("NotSupportedException")))
                                                        .WithArgumentList(
                                                            ArgumentList()))))
                                            .WithSemicolonToken(
                                                Token(SyntaxKind.SemicolonToken)))
                                .ToList();

            return ClassDeclaration($"_{applyTo.Identifier}Base")
                .WithAttributeLists(SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                QualifiedName(
                                    IdentifierName("System"), IdentifierName("Serializable")))))))
                .WithModifiers(applyTo.Modifiers)
                .WithTypeParameterList(applyTo.TypeParameterList)
                .WithConstraintClauses(applyTo.ConstraintClauses)
                .WithBaseList(
                    BaseList(
                        SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(thisType))))
                .WithMembers(List<MemberDeclarationSyntax>(methods));
        }
    }
}
