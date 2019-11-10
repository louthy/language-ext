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

                var cases = applyTo.Members
                                   .Where(m => m is MethodDeclarationSyntax)
                                   .Select(m => m as MethodDeclarationSyntax)
                                   .Zip(Enumerable.Range(1, Int32.MaxValue), (m, i) => (m, i))
                                   .Select(m => MakeCaseClass(context, applyTo.Identifier, applyTo.Members, applyTo.TypeParameterList, applyTo.Modifiers, applyTo.ConstraintClauses, m.m, m.i, true))
                                   .ToList();

                var staticCtorClass = MakeStaticConstructorClass(applyTo.Identifier, applyTo.Members, applyTo.TypeParameterList);

                return Task.FromResult(List<MemberDeclarationSyntax>().AddRange(cases).Add(staticCtorClass));
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

                var cases = applyToClass.Members
                                        .Where(m => m is MethodDeclarationSyntax)
                                        .Select(m => m as MethodDeclarationSyntax)
                                        .Zip(Enumerable.Range(1, Int32.MaxValue), (m, i) => (m, i))
                                        .Select(m => MakeCaseClass(context, applyToClass.Identifier, applyToClass.Members, applyToClass.TypeParameterList, applyToClass.Modifiers, applyToClass.ConstraintClauses, m.m, m.i, false))
                                        .ToList();

                var staticCtorClass = MakeStaticConstructorClass(applyToClass.Identifier, applyToClass.Members, applyToClass.TypeParameterList);

                var partialClass = MakeAbstractClass(applyToClass);
                var unionBase = MakeBaseFromAbstractClass(applyToClass);

                return Task.FromResult(List<MemberDeclarationSyntax>().AddRange(cases)
                                                                      .Add(staticCtorClass)
                                                                      .Add(partialClass)
                                                                      .Add(unionBase));
            }
            else
            {
                CodeGenUtil.ReportError($"Type can't be made into a union.  It must be an interface/abstract class", "Union Code-Gen", context.ProcessingNode, progress);
                return Task.FromResult(List<MemberDeclarationSyntax>());
            }
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

        static ClassDeclarationSyntax MakeStaticConstructorClass(
            SyntaxToken applyToIdentifier,
            SyntaxList<MemberDeclarationSyntax> applyToMembers,
            TypeParameterListSyntax applyToTypeParams
            )
        {
            var name = applyToIdentifier;
            if(applyToTypeParams == null)
            {
                name = Identifier($"{name}Con");
            }

            var @class = ClassDeclaration(name)
                            .WithModifiers(
                                TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword) }));

            var returnType = ParseTypeName($"{applyToIdentifier}{applyToTypeParams}");

            var cases = applyToMembers
                               .Where(m => m is MethodDeclarationSyntax)
                               .Select(m => m as MethodDeclarationSyntax)
                               .Select(m => MakeCaseCtorFunction(applyToTypeParams, returnType, m))
                               .ToList();

            return @class.WithMembers(List(cases));
        }

        static MemberDeclarationSyntax MakeCaseCtorFunction(
            TypeParameterListSyntax applyToTypeParams, 
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

        static ClassDeclarationSyntax MakeCaseClass(
            TransformationContext context, 
            SyntaxToken applyToIdentifier,
            SyntaxList<MemberDeclarationSyntax> applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxTokenList applyToModifiers,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            MethodDeclarationSyntax method,
            int tag,
            bool baseIsInterface)
        {
            var modifiers = TokenList(
                    Enumerable.Concat(
                        applyToModifiers.Where(t => !t.IsKind(SyntaxKind.PartialKeyword) && !t.IsKind(SyntaxKind.AbstractKeyword)).AsEnumerable(),
                        new[] { Token(SyntaxKind.SealedKeyword) }));

            var @class = ClassDeclaration(method.Identifier.Text)
                            .WithModifiers(modifiers)
                            .WithAttributeLists(
                                SingletonList(
                                    AttributeList(
                                        SingletonSeparatedList(
                                            Attribute(
                                                QualifiedName(
                                                    IdentifierName("System"),
                                                    IdentifierName("Serializable")))))));

            var typeParamList = applyToTypeParams;
            if(method.TypeParameterList != null)
            {
                typeParamList = typeParamList.AddParameters(method.TypeParameterList.Parameters.ToArray());
            }

            if (typeParamList != null)
            {
                @class = @class.WithTypeParameterList(typeParamList);
            }

            if (applyToConstraints != null)
            {
                @class = @class.WithConstraintClauses(applyToConstraints);
            }

            var abstractBaseType = ParseTypeName($"_{applyToIdentifier}Base{applyToTypeParams}");
            var returnType =     ParseTypeName($"{applyToIdentifier}{applyToTypeParams}");
            var thisType =       ParseTypeName($"{method.Identifier.Text}{typeParamList}");
            var thisEquatableType = ParseTypeName($"System.IEquatable<{method.Identifier.Text}{typeParamList}>");
            var thisComparableType = ParseTypeName($"System.IComparable<{method.Identifier.Text}{typeParamList}>");
            var comparableType = ParseTypeName($"System.IComparable");
            var serializableType = ParseTypeName($"System.Runtime.Serialization.ISerializable");

            var ctor = MakeConstructor(method);
            var dtor = MakeDeconstructor(method);

            var tagProp = PropertyDeclaration(
                                PredefinedType(
                                    Token(SyntaxKind.IntKeyword)),
                                Identifier("_Tag"))
                            .WithModifiers(
                                TokenList(
                                    new[]{
                                        Token(SyntaxKind.PublicKeyword),
                                        Token(SyntaxKind.OverrideKeyword)}))
                            .WithExpressionBody(
                                ArrowExpressionClause(
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(tag))))
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken));

            var fields = method.ParameterList
                               .Parameters
                               .Select(p => MakeField(returnType, p))
                               .ToList();

            fields.Add(tagProp);

            var publicMod = TokenList(Token(SyntaxKind.PublicKeyword));

            var fieldList = method.ParameterList
                                  .Parameters
                                  .Select(p => (Identifier(CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text)), p.Type, publicMod))
                                  .ToList();

            var impl = new List<MemberDeclarationSyntax>();
            if (baseIsInterface)
            {
                impl.AddRange(
                    applyToMembers
                        .Where(m => m is MethodDeclarationSyntax)
                        .Select(m => m as MethodDeclarationSyntax)
                        .Select(m => MakeExplicitInterfaceImpl(returnType, m)));
            }

            var dtype = CodeGenUtil.MakeDataTypeMembers(method.Identifier.Text, thisType, returnType, fieldList, baseIsInterface);

            fields.Add(ctor);
            fields.Add(dtor);
            fields.AddRange(dtype);
            fields.AddRange(impl);

            @class = @class.WithMembers(List(fields));

            var baseType = baseIsInterface
                               ? returnType
                               : abstractBaseType;

            // Derive from Record<UnionBaseType> and UnionBaseType
            @class = @class.WithBaseList(
                BaseList(
                    SeparatedList<BaseTypeSyntax>(
                        new SyntaxNodeOrToken[]{
                            SimpleBaseType(baseType),
                            Token(SyntaxKind.CommaToken),
                            SimpleBaseType(thisEquatableType),
                            Token(SyntaxKind.CommaToken),
                            SimpleBaseType(thisComparableType),
                            Token(SyntaxKind.CommaToken),
                            SimpleBaseType(comparableType),
                            Token(SyntaxKind.CommaToken),
                            SimpleBaseType(serializableType)
                        })));

            @class = CodeGenUtil.AddWith(context, @class, thisType, fieldList);
            @class = CodeGenUtil.AddLenses(@class, thisType, fieldList);

            return @class;
        }

        static MemberDeclarationSyntax MakeExplicitInterfaceImpl(TypeSyntax returnType, MethodDeclarationSyntax @case)
        {
            var method = MethodDeclaration(@case.ReturnType, @case.Identifier)
                            .WithExplicitInterfaceSpecifier(
                                ExplicitInterfaceSpecifier(ParseName(returnType.ToString())))
                            .WithParameterList(@case.ParameterList)
                            .WithExpressionBody(
                                ArrowExpressionClause(
                                    ThrowExpression(
                                        ObjectCreationExpression(
                                            QualifiedName(
                                                IdentifierName("System"),
                                                IdentifierName("NotSupportedException")))
                                        .WithArgumentList(ArgumentList()))))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            if(@case.TypeParameterList != null)
            {
                method = method.WithTypeParameterList(@case.TypeParameterList);
            }
            return method;
        }

        static MemberDeclarationSyntax MakeConstructor(MethodDeclarationSyntax method)
        {
            var assignments = method.ParameterList
                                    .Parameters
                                    .Select(p =>
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName(CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text))),
                                                IdentifierName(p.Identifier.Text))));


            return ConstructorDeclaration(Identifier(method.Identifier.Text))
                        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                        .WithParameterList(method.ParameterList)
                        .WithBody(Block(List(assignments)));
        }


        static MethodDeclarationSyntax MakeDeconstructor(MethodDeclarationSyntax method)
        {
            var assignments = method.ParameterList
                                    .Parameters
                                    .Select(p =>
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName(CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text)),
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName(CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text)))
                                                )))
                                    .ToArray();

            // Make the parameters start with an upper case letter and have the out modifier
            var parameters = method.ParameterList
                                   .Parameters
                                   .Select(p => p.WithIdentifier(Identifier(CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text)))
                                                 .WithModifiers(TokenList(Token(SyntaxKind.OutKeyword))))
                                   .ToArray();

            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Deconstruct"))
                       .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                       .WithParameterList(ParameterList(SeparatedList(parameters)))
                       .WithBody(Block(assignments));
        }

        static MemberDeclarationSyntax MakeField(TypeSyntax returnType, ParameterSyntax p)
        {
            var fieldName = CodeGenUtil.MakeFirstCharUpper(p.Identifier.Text);

            var field = FieldDeclaration(
                            VariableDeclaration(p.Type)
                                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(fieldName)))))
                            .WithModifiers(
                                TokenList(
                                    new[]{
                                        Token(SyntaxKind.PublicKeyword),
                                        Token(SyntaxKind.ReadOnlyKeyword)}))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)); ;

            return field;
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
                                            Identifier("_Tag"))
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
