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
                if(!AllMembersReturnInterface(applyTo, context, progress))
                {
                    return Task.FromResult(SyntaxFactory.List<MemberDeclarationSyntax>());
                }

                var cases = applyTo.Members
                                   .Where(m => m is MethodDeclarationSyntax)
                                   .Select(m => m as MethodDeclarationSyntax)
                                   .Select(m => MakeCaseClass(applyTo, m))
                                   .ToList();

                var staticCtorClass = MakeStaticConstructorClass(applyTo);

                return Task.FromResult(SyntaxFactory.List<MemberDeclarationSyntax>().AddRange(cases).Add(staticCtorClass));
            }
            else
            {
                CodeGenUtil.ReportError($"Type can't be made into a union.  It must be an interface/abstract class", "Union Code-Gen", context.ProcessingNode, progress);
                return Task.FromResult(SyntaxFactory.List<MemberDeclarationSyntax>());
            }
        }

        static bool AllMembersReturnInterface(
            InterfaceDeclarationSyntax applyTo,
            TransformationContext context,
            IProgress<Diagnostic> progress
            )
        {
            bool allMethods = true;
            foreach(var nonMethod in applyTo.Members.Where(m => !(m is MethodDeclarationSyntax)))
            {
                allMethods = false;
                CodeGenUtil.ReportError($"Type can't contain anything other than methods if you want to make it into a union type.", "Union Code-Gen", nonMethod, progress);
            }

            if(!allMethods)
            {
                return false;
            }

            var returnType = $"{applyTo.Identifier}{applyTo.TypeParameterList}";

            bool returnsTypesOk = true;
            foreach(var method in applyTo.Members.Select(m => m as MethodDeclarationSyntax))
            {
                if(method.ReturnType.ToString() != returnType)
                {
                    CodeGenUtil.ReportError($"Methods in union types must return the same type as the interface/abstract class they're defined in. ({method.ReturnType} != {returnType})", "Union Code-Gen", method, progress);
                    returnsTypesOk = false;
                }
            }
            return returnsTypesOk;
        }

        static ClassDeclarationSyntax MakeStaticConstructorClass(InterfaceDeclarationSyntax applyTo)
        {
            var name = applyTo.Identifier;
            if(applyTo.TypeParameterList == null)
            {
                name = Identifier($"{name}Con");
            }

            var @class = ClassDeclaration(name)
                            .WithModifiers(
                                TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword) }));

            var returnType = ParseTypeName($"{applyTo.Identifier}{applyTo.TypeParameterList}");

            var cases = applyTo.Members
                               .Where(m => m is MethodDeclarationSyntax)
                               .Select(m => m as MethodDeclarationSyntax)
                               .Select(m => MakeCaseCtorFunction(applyTo, returnType, m))
                               .ToList();

            return @class.WithMembers(List(cases));
        }

        static MemberDeclarationSyntax MakeCaseCtorFunction(InterfaceDeclarationSyntax applyTo, TypeSyntax returnType, MethodDeclarationSyntax method)
        {
            var typeParamList = applyTo.TypeParameterList;
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

        static ClassDeclarationSyntax MakeCaseClass(InterfaceDeclarationSyntax applyTo, MethodDeclarationSyntax method)
        {
            var modifiers = TokenList(
                    Enumerable.Concat(
                        applyTo.Modifiers.Where(t => !t.IsKind(SyntaxKind.PartialKeyword)).AsEnumerable(),
                        new[] { SyntaxFactory.Token(SyntaxKind.PartialKeyword) }));


            var @class = ClassDeclaration(method.Identifier.Text).WithModifiers(modifiers);

            var typeParamList = applyTo.TypeParameterList;
            if(method.TypeParameterList != null)
            {
                typeParamList = typeParamList.AddParameters(method.TypeParameterList.Parameters.ToArray());
            }

            if (typeParamList != null)
            {
                @class = @class.WithTypeParameterList(typeParamList);
            }

            if (applyTo.ConstraintClauses != null)
            {
                @class = @class.WithConstraintClauses(applyTo.ConstraintClauses);
            }

            var returnType =     ParseTypeName($"{applyTo.Identifier}{applyTo.TypeParameterList}");
            var thisType =       ParseTypeName($"{method.Identifier.Text}{typeParamList}");
            var thisRecordType = ParseTypeName($"LanguageExt.Record<{method.Identifier.Text}{typeParamList}>");

            var ctor = MakeConstructor(method);
            var dtor = MakeDeconstructor(method);

            var fields = method.ParameterList
                               .Parameters
                               .Select(p => MakeField(returnType, p))
                               .ToList();

            var impl = applyTo.Members
                              .Where(m => m is MethodDeclarationSyntax)
                              .Select(m => m as MethodDeclarationSyntax)
                              .Select(m => MakeExplicitInterfaceImpl(returnType, m))
                              .ToList();


            fields.Add(ctor);
            fields.Add(dtor);
            fields.AddRange(impl);

            @class = @class.WithMembers(List(fields));

            @class = @class.WithBaseList(
                BaseList(
                    SeparatedList<BaseTypeSyntax>(
                        new SyntaxNodeOrToken[]{
                            SimpleBaseType(thisRecordType),
                            Token(SyntaxKind.CommaToken),
                            SimpleBaseType(returnType)
                        })));

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
    }
}

//public interface Option<A>
//{
//    Option<A> Some(A value);
//    Option<A> None();
//}

//public class Some<A> : Option<A>, Record<Some<A>>
//{
//    public readonly A Value;
//    public Some(A value)
//    {
//        Value = value;
//    }

//    [EditorBrowsable(EditorBrowsableState.Never)]
//    Option<A> Option<A>.None() => new None<A>();
//    [EditorBrowsable(EditorBrowsableState.Never)]
//    Option<A> Option<A>.Some(A value) => new Some<A>(value);
//}

//public class None<A> : Option<A>, Record<None<A>>
//{
//    public None()
//    {
//    }

//    [EditorBrowsable(EditorBrowsableState.Never)]
//    Option<A> Option<A>.None() => new None<A>();
//    [EditorBrowsable(EditorBrowsableState.Never)]
//    Option<A> Option<A>.Some(A value) => new Some<A>(value);
//}

//public static partial class Option
//{
//    public static Option<A> Some<A>(A value) => new Some<A>(value);
//    public static Option<A> None<A>() => new None<A>();
//}
