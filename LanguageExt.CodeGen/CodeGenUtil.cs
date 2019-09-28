using System;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LanguageExt.CodeGen
{
    internal static class CodeGenUtil
    {
        public static (ClassDeclarationSyntax PartialClass, TypeSyntax ReturnType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> Fields) GetState(TransformationContext context)
        {
            // Our generator is applied to any class that our attribute is applied to.
            var applyToClass = (ClassDeclarationSyntax)context.ProcessingNode;

            var classModifiers = SyntaxFactory.TokenList(
                    Enumerable.Concat(
                        applyToClass.Modifiers
                                    .Where(t => !t.IsKind(SyntaxKind.PartialKeyword)).AsEnumerable(),
                        new[] { SyntaxFactory.Token(SyntaxKind.PartialKeyword) }));

            // Apply a suffix to the name of a copy of the class.
            var partialClass = SyntaxFactory.ClassDeclaration($"{applyToClass.Identifier}")
                                            .WithModifiers(classModifiers);

            if (applyToClass.TypeParameterList != null)
            {
                partialClass = partialClass.WithTypeParameterList(applyToClass.TypeParameterList);
            }

            if (applyToClass.ConstraintClauses != null)
            {
                partialClass = partialClass.WithConstraintClauses(applyToClass.ConstraintClauses);
            }

            var returnType = CodeGenUtil.TypeFromClass(applyToClass);

            var indexedMembers = applyToClass.Members.Select((m, i) => (m, i));

            var fields = indexedMembers.Where(m => m.m is FieldDeclarationSyntax)
                                       .Select(m => (f: m.m as FieldDeclarationSyntax, m.i))
                                       .Where(m => m.f.Declaration.Variables.Count > 0)
                                       .Where(m => FirstCharIsUpper(m.f.Declaration.Variables[0].Identifier.ToString()))
                                       .Where(m => m.f.Modifiers.Any(SyntaxKind.PublicKeyword))
                                       .Where(m => m.f.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                                       .Where(m => !m.f.Modifiers.Any(SyntaxKind.StaticKeyword))
                                       .Select(m => ( 
                                           m.f.Declaration.Variables[0].Identifier,
                                           m.f.Declaration.Type,
                                           m.f.Modifiers,
                                           m.i
                                       ));
   
            var properties = indexedMembers.Where(m => m.m is PropertyDeclarationSyntax)
                                           .Select(m => (p: m.m as PropertyDeclarationSyntax, m.i))
                                           .Where(m => FirstCharIsUpper(m.p.Identifier.ToString()))
                                           .Where(m => m.p.Modifiers.Any(SyntaxKind.PublicKeyword))
                                           .Where(m => !m.p.Modifiers.Any(SyntaxKind.StaticKeyword))
                                           .Where(m => m.p.AccessorList.Accessors.Count == 1)
                                           .Where(m => m.p.AccessorList.Accessors[0].Kind() == SyntaxKind.GetAccessorDeclaration)
                                           .Where(m => m.p.AccessorList.Accessors[0].ExpressionBody == null)
                                           .Where(m => m.p.AccessorList.Accessors[0].Body == null)
                                           .Where(m => m.p.Initializer == null)
                                           .Select(m => (
                                               m.p.Identifier,
                                               m.p.Type,
                                               m.p.Modifiers,
                                               m.i
                                           ));
 
            var members = fields.Concat(properties)
                .OrderBy(m => m.i) // Preserve the order between properties and fields.
                .Select(m => (m.Identifier, m.Type, m.Modifiers))
                .ToList();

            return (partialClass, returnType, members);
        }

        internal static bool ForAll<A>(this IEnumerable<A> ma, Func<A, bool> f)
        {
            foreach(var a in ma)
            {
                if (!f(a)) return false;
            }
            return true;
        }

        internal static bool Exists<A>(this IEnumerable<A> ma, Func<A, bool> f)
        {
            foreach (var a in ma)
            {
                if (f(a)) return true;
            }
            return false;
        }

        public static ClassDeclarationSyntax AddLenses(ClassDeclarationSyntax partialClass, TypeSyntax returnType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members)
        {
            foreach (var member in members)
            {
                partialClass = AddLens(partialClass, returnType, member);
            }
            return partialClass;
        }

        public static ClassDeclarationSyntax AddLens(ClassDeclarationSyntax partialClass, TypeSyntax returnType, (SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers) member)
        {
            var lfield = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier("Lens"))
                                 .WithTypeArgumentList(
                                    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(new[] { returnType, member.Type }))))
                             .WithVariables(
                                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    SyntaxFactory.VariableDeclarator(MakeCamelCaseId(member.Identifier))
                                                 .WithInitializer(
                                                    SyntaxFactory.EqualsValueClause(
                                                        SyntaxFactory.InvocationExpression(
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.GenericName("Lens")
                                                                                    .WithTypeArgumentList(
                                                                                        SyntaxFactory.TypeArgumentList(
                                                                                            SyntaxFactory.SeparatedList<TypeSyntax>(new[] { returnType, member.Type }))),
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
                                                                                                SyntaxFactory.IdentifierName(member.Identifier.ToString())))),
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
                                                                                                                                SyntaxFactory.NameColon(member.Identifier.ToString()))))))))

                                                                                }))))))));

            lfield = lfield.WithModifiers(
                SyntaxFactory.TokenList(
                    Enumerable.Concat(
                        member.Modifiers.Where(m => m.IsKind(SyntaxKind.PublicKeyword) || m.IsKind(SyntaxKind.PrivateKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword)),
                        new[] { SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword) })));

            return partialClass.AddMembers(lfield);
        }

        public static ClassDeclarationSyntax AddWith(TransformationContext context, ClassDeclarationSyntax partialClass, TypeSyntax returnType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members)
        {
            var withParms = members.Select(m => (Id: m.Identifier,
                                                Type: m.Type,
                                                Info: context.SemanticModel.GetTypeInfo(m.Type)))
                                   .Select(m => (m.Id,
                                                 m.Type,
                                                 m.Info,
                                                 IsGeneric: !m.Info.Type.IsValueType && !m.Info.Type.IsReferenceType,
                                                 ParamType: m.Info.Type.IsValueType
                                                     ? SyntaxFactory.NullableType(m.Type)
                                                     : m.Type))
                                   .Select(m =>
                                        SyntaxFactory.Parameter(MakeFirstCharUpper(m.Id))
                                                     .WithType(m.ParamType)
                                                     .WithDefault(
                                                         m.IsGeneric
                                                             ? SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(m.Type))
                                                             : SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))))
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
                                                                    SyntaxFactory.BinaryExpression(
                                                                        SyntaxKind.CoalesceExpression,
                                                                        SyntaxFactory.IdentifierName(wa.Identifier),
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.ThisExpression(),
                                                                            SyntaxFactory.IdentifierName(wa.Identifier))))))), null)));

            partialClass = partialClass.AddMembers(withMethod);
            return partialClass;
        }

        public static TypeSyntax TypeFromClass(ClassDeclarationSyntax decl) =>
            SyntaxFactory.ParseTypeName($"{decl.Identifier}{decl.TypeParameterList}");// SyntaxFactory.IdentifierName(decl.Identifier);

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

        static bool FirstCharIsUpper(string name) =>
            name.Length > 0 && Char.IsUpper(name[0]);


        static HashSet<string> identifiers = new HashSet<string>(new[]
        {
            "abstract", "as", "base", "bool",
            "break", "byte", "case", "catch",
            "char", "checked", "class", "const",
            "continue", "decimal", "default", "delegate",
            "do", "double", "else", "enum",
            "event", "explicit", "extern", "false",
            "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit",
            "in", "int", "interface",
            "internal", "is", "lock", "long",
            "namespace", "new", "null", "object",
            "operator", "out", "out", "override",
            "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc",
            "static", "string", "struct", "switch",
            "this", "throw", "true", "try",
            "typeof", "uint", "ulong", "unchecked",
            "unsafe", "ushort", "using","void", "volatile",
            "while"
        });

        static bool IsIdentifier(string id) =>
            identifiers.Contains(id);

        static string MakeCamelCaseId(string id)
        {
            var id2 = $"{Char.ToLower(id[0])}{id.Substring(1)}";
            return IsIdentifier(id2)
                ? $"@{id2}"
                : id2;
        }

        public static SyntaxToken MakeCamelCaseId(SyntaxToken identifier) =>
            SyntaxFactory.Identifier(MakeCamelCaseId(identifier.ToString()));

        public static string NextGenName(string gen) =>
            $"{gen.Substring(0, gen.Length - 1)}{(char)((gen[gen.Length - 1] + 1))}";

        public static ClassDeclarationSyntax AddMembersToPrelude(ClassDeclarationSyntax prelude, StructDeclarationSyntax applyToStruct, string getter, ITypeSymbol symbol)
        {
            if (symbol == null) return prelude;

            var ask = MakeAsk(applyToStruct, getter);

            foreach (var member in symbol.GetMembers())
            {
                switch (member.Kind)
                {
                    case SymbolKind.Field:
                        var field = (IFieldSymbol)member;
                        if (applyToStruct.TypeParameterList.Parameters.Count < 2 && !field.IsStatic)
                        {
                            var fdecl = CreateFieldOrProperty(applyToStruct, getter, field.Name, field.Type);
                            prelude = prelude.AddMembers(fdecl);
                        }
                        break;

                    case SymbolKind.Property:
                        var prop = (IPropertySymbol)member;
                        if (applyToStruct.TypeParameterList.Parameters.Count < 2 && !prop.IsStatic && prop.Name != "this[]")
                        {
                            var fdecl = CreateFieldOrProperty(applyToStruct, getter, prop.Name, prop.Type);
                            prelude = prelude.AddMembers(fdecl);
                        }
                        break;

                    case SymbolKind.Method:
                        var method = (IMethodSymbol)member;

                        if (method.MethodKind == MethodKind.Ordinary &&
                            method.ReturnType.Name != "Void" &&
                            !method.IsStatic &&
                            method.Name != "GetHashCode" &&
                            method.Name != "Equals" &&
                            method.Name != "CompareTo" &&
                            method.Name != "ToString" &&
                            method.Name != "this" &&
                            method.Name != "GetEnumerator"
                            )
                        {
                            // Method generics
                            var generics = TypeParameterList(SeparatedList<TypeParameterSyntax>(method.TypeParameters.Select(a => TypeParameter(a.Name))));

                            var sparams = applyToStruct.TypeParameterList.Parameters.Count > 0
                                ? applyToStruct.TypeParameterList.Parameters.Take(applyToStruct.TypeParameterList.Parameters.Count - 1).ToList()
                                : applyToStruct.TypeParameterList.Parameters.ToList();

                            sparams.AddRange(generics.Parameters);

                            // Method args
                            var args = ParameterList(SeparatedList<ParameterSyntax>(
                                            method.Parameters.Select(a => Parameter(Identifier(a.Name)).WithType(SyntaxFactory.ParseTypeName(a.Type.ToString())))));

                            // Return type
                            var returnType = MakeGenericStruct(applyToStruct, method.ReturnType.ToString());

                            // Invocation
                            var invoke = generics.Parameters.Count == 0
                                ? MemberAccessExpression(
                                      SyntaxKind.SimpleMemberAccessExpression,
                                      IdentifierName("__env"),
                                      IdentifierName(method.Name))
                                : MemberAccessExpression(
                                      SyntaxKind.SimpleMemberAccessExpression,
                                      IdentifierName("__env"),
                                          GenericName(Identifier(method.Name))
                                              .WithTypeArgumentList(
                                                  TypeArgumentList(SeparatedList<TypeSyntax>(method.TypeParameters.Select(a => IdentifierName(a.Name))))));

                            var decl = MethodDeclaration(returnType, method.Name)
                                           .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                                           .WithParameterList(args)
                                           .WithExpressionBody(
                                                ArrowExpressionClause(
                                                    InvocationExpression(ask)
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    SimpleLambdaExpression(
                                                                        Parameter(Identifier("__env")),
                                                                        InvocationExpression(invoke)
                                                                        .WithArgumentList(
                                                                            ArgumentList(
                                                                                SeparatedList<ArgumentSyntax>(
                                                                                    method.Parameters.Select(a => Argument(IdentifierName(a.Name)))))))))))))
                                            .WithSemicolonToken(
                                                Token(SyntaxKind.SemicolonToken));

                            decl = sparams.Count == 0
                                ? decl
                                : decl.WithTypeParameterList(TypeParameterList(SeparatedList<TypeParameterSyntax>(sparams)));

                            prelude = prelude.AddMembers(decl);
                        }

                        break;
                }

            }

            return prelude;
        }

        static MemberAccessExpressionSyntax MakeAsk(StructDeclarationSyntax applyToStruct, string name)
        {
            // Ask
            return applyToStruct.TypeParameterList.Parameters.Count > 1
                        ? MemberAccessExpression(
                              SyntaxKind.SimpleMemberAccessExpression,
                              InvocationExpression(
                                  GenericName(Identifier(name))
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
                                   IdentifierName(name),
                                   IdentifierName("Map"));
        }

        static PropertyDeclarationSyntax CreateFieldOrProperty(StructDeclarationSyntax applyToStruct, string getter, string name, ITypeSymbol type)
        {
            var fdecl = PropertyDeclaration(
                            MakeGenericStruct(applyToStruct, type.ToString()),
                            Identifier(name))
                        .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(getter),
                                        IdentifierName("Map")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    SimpleLambdaExpression(
                                                        Parameter(
                                                            Identifier("__env")),
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("__env"),
                                                                IdentifierName(name)))))))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken)); ;
            return fdecl;
        }

        public static TypeSyntax MakeGenericStruct(StructDeclarationSyntax s, params string[] genAdd)
        {
            var nolast = s.TypeParameterList.Parameters.RemoveAt(s.TypeParameterList.Parameters.Count - 1);
            var gens = nolast.AddRange(genAdd.Select(gen => TypeParameter(gen)));
            return SyntaxFactory.ParseTypeName($"{s.Identifier}<{gens}>");
        }

        public static TypeSyntax ExceptionType = SystemType("Exception");

        public static TypeSyntax FuncType(params SyntaxNodeOrToken[] gens) =>
            SystemType("Func", gens);

        public static TypeSyntax FuncType(string gen, params SyntaxNodeOrToken[] gens)
        {
            var ngens = new SyntaxNodeOrToken[gens.Length + 1];
            ngens[0] = IdentifierName(gen);
            gens.CopyTo(ngens, 1);
            return FuncType(ngens);
        }

        public static TypeSyntax FuncType(string gen) =>
            SystemType("Func", gen);

        public static TypeSyntax FuncType(string genA, string genB) =>
            SystemType("Func", IdentifierName(genA), IdentifierName(genB));

        public static TypeSyntax FuncType(string genA, string genB, string genC) =>
            SystemType("Func", IdentifierName(genA), IdentifierName(genB), IdentifierName(genC));

        public static TypeSyntax ActionType(params SyntaxNodeOrToken[] gens) =>
            SystemType("Action", gens);

        public static TypeSyntax ActionType(string gen) =>
            SystemType("Action", gen);

        public static TypeSyntax SystemType(string name, params SyntaxNodeOrToken[] gens) =>
            gens.Length == 0
                ? QualifiedName(IdentifierName("System"), IdentifierName(name))
                : QualifiedName(
                      IdentifierName("System"),
                      GenericName(Identifier(name))
                        .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(Interleave(gens, Token(SyntaxKind.CommaToken))))));

        public static TypeSyntax SystemType(string name, string gen) =>
            QualifiedName(
                IdentifierName("System"), GenericName(
                Identifier(name))
            .WithTypeArgumentList(
                TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(gen)))));

        public static A[] Interleave<A>(A[] xs, A sep)
        {
            // Ugly, but fast

            if (xs.Length == 0) return new A[0];
            var nxs = new A[xs.Length * 2 - 1];
            for(int i = 0; i < nxs.Length; i++)
            {
                nxs[i] = i % 2 == 0
                    ? xs[i >> 1]
                    : sep;
            }
            return nxs;
        }

        public static MemberAccessExpressionSyntax PreludeMember(SimpleNameSyntax name) =>
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("LanguageExt"),
                    IdentifierName("Prelude")),
                name);

        public static HashSet<string> MemberNames(SyntaxList<MemberDeclarationSyntax> list) =>
            new HashSet<string>(
                list.Select(MemberName)
                    .Where(m => !String.IsNullOrWhiteSpace(m))
                    .Distinct());

        static string MemberName(MemberDeclarationSyntax decl) =>
            decl is PropertyDeclarationSyntax p ? p.Identifier.Text
          : decl is MethodDeclarationSyntax m   ? m.Identifier.Text
          : "";

        public static MemberDeclarationSyntax NullIfExists(HashSet<string> members, MemberDeclarationSyntax member) =>
            members.Contains(MemberName(member))
                ? null
                : member;

    }
}
