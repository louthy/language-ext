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
    public enum AllowedType
    {
        Class = 1,
        Struct = 2,
        ClassOrStruct = 3
    }

    public enum BaseSpec
    {
        Interface,
        Abstract,
        None
    }

    internal static class CodeGenUtil
    {
        public static readonly TypeSyntax ExceptionType;
        public static readonly TypeSyntax UnitType;

        static CodeGenUtil()
        {
            var classInstances = QualifiedName(IdentifierName("LanguageExt"), IdentifierName("ClassInstances"));
            ExceptionType = SystemType("Exception");
            UnitType = ParseTypeName("LanguageExt.Unit");
        }

        public static void ReportInfo(string message, string codeGenCategory, SyntaxNode node, IProgress<Diagnostic> progress) =>
            Report(message, codeGenCategory, DiagnosticSeverity.Info, node, progress);

        public static void ReportWarning(string message, string codeGenCategory, SyntaxNode node, IProgress<Diagnostic> progress) =>
            Report(message, codeGenCategory, DiagnosticSeverity.Warning, node, progress);

        public static void ReportError(string message, string codeGenCategory, SyntaxNode node, IProgress<Diagnostic> progress) =>
            Report(message, codeGenCategory, DiagnosticSeverity.Error, node, progress);

        public static void Report(string message, string codeGenCategory, DiagnosticSeverity severity, SyntaxNode node, IProgress<Diagnostic> progress)
        {
            progress.Report(Diagnostic.Create(
                new DiagnosticDescriptor(
                    $"CG{Math.Abs(message.GetHashCode()>>16)}",
                    message,
                    message,
                    codeGenCategory,
                    severity,
                    true,
                    message),
                node.GetLocation()));
        }

        public static (TypeDeclarationSyntax PartialType, TypeSyntax ReturnType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> Fields) GetState(TransformationContext context, IProgress<Diagnostic> progress, AllowedType allowedTypes, string codeGenCategory)
        {
            // Our generator is applied to any class that our attribute is applied to.
            var applyTo = (TypeDeclarationSyntax)context.ProcessingNode;

            // Remove and re-add `partial`
            var modifiers = TokenList(
                    Enumerable.Concat(
                        applyTo.Modifiers
                                    .Where(t => !t.IsKind(SyntaxKind.PartialKeyword)).AsEnumerable(),
                        new[] { Token(SyntaxKind.PartialKeyword) }));

            // Ensure class or struct
            var partialType = applyTo switch
            {
                ClassDeclarationSyntax _ when (allowedTypes & AllowedType.Class) == AllowedType.Class =>
                    ClassDeclaration($"{applyTo.Identifier}").WithModifiers(modifiers),

                StructDeclarationSyntax _ when (allowedTypes & AllowedType.Struct) == AllowedType.Struct =>
                    StructDeclaration($"{applyTo.Identifier}").WithModifiers(modifiers),

                _ => (TypeDeclarationSyntax)null
            };

            if (partialType is null)
            {
                var error = allowedTypes switch
                {
                    AllowedType.Class => "Type should be a struct",
                    AllowedType.Struct => "Type should be a class",
                    AllowedType.ClassOrStruct => "Type should be a class or struct",
                    _ => throw new NotSupportedException()
                };
                ReportError(error, codeGenCategory, applyTo, progress);
                return default;
            }

            // Apply type params
            if (applyTo.TypeParameterList != null)
            {
                partialType = partialType.WithTypeParameterList(applyTo.TypeParameterList);
            }

            // Apply type constraints
            if (applyTo.ConstraintClauses != null)
            {
                partialType = partialType.WithConstraintClauses(applyTo.ConstraintClauses);
            }

            // Get the return type as a TypeSyntax
            var returnType = TypeFromTypeDecl(applyTo);

            // Provide an index for each member
            var indexedMembers = applyTo.Members.Select((m, i) => (m, i)).ToList();

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
                                           .Where(m => m.p.AccessorList != null && m.p.AccessorList.Accessors != null)
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

            return (partialType, returnType, members);
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

        public static TypeDeclarationSyntax AddLenses(TypeDeclarationSyntax partialClass, TypeSyntax returnType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members)
        {
            foreach (var member in members)
            {
                partialClass = AddLens(partialClass, returnType, member);
            }
            return partialClass;
        }

        public static TypeDeclarationSyntax AddLens(TypeDeclarationSyntax partialClass, TypeSyntax returnType, (SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers) member)
        {
            var lfield = FieldDeclaration(
                VariableDeclaration(
                    GenericName(Identifier("Lens"))
                                 .WithTypeArgumentList(
                                    TypeArgumentList(SeparatedList<TypeSyntax>(new[] { returnType, member.Type }))))
                             .WithVariables(
                                SingletonSeparatedList<VariableDeclaratorSyntax>(
                                    VariableDeclarator(MakeCamelCaseId(member.Identifier))
                                                 .WithInitializer(
                                                    EqualsValueClause(
                                                        InvocationExpression(
                                                                            MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                GenericName("Lens")
                                                                                    .WithTypeArgumentList(
                                                                                        TypeArgumentList(
                                                                                            SeparatedList<TypeSyntax>(new[] { returnType, member.Type }))),
                                                                                    IdentifierName("New")))
                                                                     .WithArgumentList(
                                                                        ArgumentList(
                                                                            SeparatedList<ArgumentSyntax>(
                                                                                new SyntaxNodeOrToken[] {
                                                                                    Argument(
                                                                                        SimpleLambdaExpression(
                                                                                            Parameter(
                                                                                                Identifier("_x")),
                                                                                            MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                IdentifierName("_x"),
                                                                                                IdentifierName(MakeFirstCharUpper(member.Identifier))))),
                                                                                    Token(SyntaxKind.CommaToken),
                                                                                    Argument(
                                                                                        SimpleLambdaExpression(
                                                                                            Parameter(
                                                                                                Identifier("_x")),
                                                                                                SimpleLambdaExpression(
                                                                                                    Parameter(
                                                                                                        Identifier("_y")),
                                                                                                        InvocationExpression(
                                                                                                            MemberAccessExpression(
                                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                                IdentifierName("_y"),
                                                                                                                IdentifierName("With")))
                                                                                                            .WithArgumentList(
                                                                                                                ArgumentList(
                                                                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                                                                        Argument(
                                                                                                                            IdentifierName("_x"))
                                                                                                                            .WithNameColon(
                                                                                                                                NameColon(MakeFirstCharUpper(member.Identifier.ToString()).ToString()))))))))

                                                                                }))))))));

            lfield = lfield.WithModifiers(
                TokenList(
                    Enumerable.Concat(
                        member.Modifiers.Where(m => m.IsKind(SyntaxKind.PublicKeyword) || m.IsKind(SyntaxKind.PrivateKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword)),
                        new[] { Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.ReadOnlyKeyword) })));

            return partialClass.AddMembers(lfield);
        }

        public static TypeDeclarationSyntax AddWith(TransformationContext context, TypeDeclarationSyntax partialClass, TypeSyntax returnType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members)
        {
            var withParms = members.Select(m => (Id: m.Identifier,
                                                Type: m.Type,
                                                Info: context.SemanticModel.GetTypeInfo(m.Type)))
                                   .Select(m => (m.Id,
                                                 m.Type,
                                                 m.Info,
                                                 IsGeneric: !m.Info.Type.IsValueType && !m.Info.Type.IsReferenceType,
                                                 ParamType: m.Info.Type.IsValueType
                                                     ? NullableType(m.Type)
                                                     : m.Type))
                                   .Select(m =>
                                        Parameter(MakeFirstCharUpper(m.Id))
                                                     .WithType(m.ParamType)
                                                     .WithDefault(
                                                         m.IsGeneric
                                                             ? EqualsValueClause(DefaultExpression(m.Type))
                                                             : EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression))))
                                   .ToArray();

            var withMethod = MethodDeclaration(returnType, "With")
                                          .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(withParms)))
                                          .WithModifiers(TokenList(
                                              Token(SyntaxKind.PublicKeyword)))
                                          .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                                          .WithExpressionBody(
                                              ArrowExpressionClause(
                                                  ObjectCreationExpression(
                                                      returnType,
                                                      ArgumentList(
                                                          SeparatedList<ArgumentSyntax>(
                                                              withParms.Select(wa =>
                                                                Argument(
                                                                    BinaryExpression(
                                                                        SyntaxKind.CoalesceExpression,
                                                                        IdentifierName(wa.Identifier),
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            ThisExpression(),
                                                                            IdentifierName(wa.Identifier))))))), null)));

            partialClass = partialClass.AddMembers(withMethod);
            return partialClass;
        }

        public static TypeSyntax TypeFromTypeDecl(TypeDeclarationSyntax decl) =>
            ParseTypeName($"{decl.Identifier}{decl.TypeParameterList}");

        public static SyntaxToken MakeFirstCharUpper(SyntaxToken identifier)
        {
            var id = identifier.ToString();
            var id2 = $"{Char.ToUpper(id[0])}{id.Substring(1)}";
            return Identifier(id2);
        }

        public static string MakeFirstCharUpper(string identifier)
        {
            var id = identifier;
            var id2 = $"{Char.ToUpper(id[0])}{id.Substring(1)}";
            return id2;
        }

        public static SyntaxToken MakeFirstCharLower(SyntaxToken identifier)
        {
            var id = identifier.ToString();
            var id2 = $"{Char.ToLower(id[0])}{id.Substring(1)}";
            return Identifier(id2);
        }

        public static string MakeFirstCharLower(string identifier)
        {
            var id = identifier;
            var id2 = $"{Char.ToLower(id[0])}{id.Substring(1)}";
            return id2;
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
            Identifier(MakeCamelCaseId(identifier.ToString()));

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
                                            method.Parameters.Select(a => Parameter(Identifier(a.Name)).WithType(ParseTypeName(a.Type.ToString())))));

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
            return ParseTypeName($"{s.Identifier}<{gens}>");
        }

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

        public static HashSet<string> MethodNames(SyntaxList<MemberDeclarationSyntax> list) =>
            new HashSet<string>(
                list.Select(MethodName)
                    .Where(m => !String.IsNullOrWhiteSpace(m))
                    .Distinct());

        public static HashSet<string> MemberNames(SyntaxList<MemberDeclarationSyntax> list) =>
            new HashSet<string>(
                list.Select(MemberName)
                    .Where(m => !String.IsNullOrWhiteSpace(m))
                    .Distinct());

        static string MethodName(MemberDeclarationSyntax decl) =>
          decl is MethodDeclarationSyntax m ? m.Identifier.Text : "";

        static string MemberName(MemberDeclarationSyntax decl) =>
            decl is PropertyDeclarationSyntax p ? p.Identifier.Text
          : decl is MethodDeclarationSyntax m   ? m.Identifier.Text
          : "";

        public static MemberDeclarationSyntax NullIfExists(HashSet<string> members, MemberDeclarationSyntax member) =>
            members.Contains(MemberName(member))
                ? null
                : member;

        public static MemberDeclarationSyntax[] MakeDataTypeMembers(string typeName, TypeSyntax thisType, TypeSyntax baseType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members, BaseSpec baseSpec, bool typeIsClass)
        {
            var nmembers = new List<MemberDeclarationSyntax>();
            nmembers.AddRange(MakeSerialisationMembers(typeName, members));
            nmembers.AddRange(typeIsClass ? MakeClassOperatorMembers(thisType) : MakeStructOperatorMembers(thisType));
            nmembers.AddRange(MakeEqualityMembers(thisType, baseType, members, baseSpec));
            nmembers.AddRange(MakeOrderingMembers(thisType, baseType, members, baseSpec));
            nmembers.Add(MakeGetHashCode(members));
            nmembers.Add(MakeToString(typeName, members));
            return nmembers.ToArray();
        }

        static MemberDeclarationSyntax MakeToString(string typeName, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members)
        {
            var statements = new List<StatementSyntax>();

            if (members.Count == 0)
            {
                statements.Add(ReturnStatement(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal(typeName))));
            }
            else
            {
                var comma = ExpressionStatement(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("sb"),
                                        IdentifierName("Append")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                InterpolatedStringExpression(
                                                    Token(SyntaxKind.InterpolatedStringStartToken))
                                                .WithContents(
                                                    SingletonList<InterpolatedStringContentSyntax>(
                                                        InterpolatedStringText()
                                                        .WithTextToken(
                                                            Token(
                                                                TriviaList(),
                                                                SyntaxKind.InterpolatedStringTextToken,
                                                                ", ",
                                                                ", ",
                                                                TriviaList())))))))));



                statements.Add(
                    LocalDeclarationStatement(
                        VariableDeclaration(
                            IdentifierName("var"))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("sb"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                            QualifiedName(
                                                QualifiedName(
                                                    IdentifierName("System"),
                                                    IdentifierName("Text")),
                                                IdentifierName("StringBuilder")))
                                        .WithArgumentList(
                                            ArgumentList())))))));

                statements.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("sb"),
                                    IdentifierName("Append")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                members.Count == 0 ? Literal($"{typeName}") : Literal($"{typeName}("))))))));

                statements.AddRange(members.SelectMany(m =>
                                new StatementSyntax[] {
                            ExpressionStatement(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("sb"),
                                        IdentifierName("Append")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                ConditionalExpression(
                                                    InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("LanguageExt"),
                                                            IdentifierName("Prelude")),
                                                        IdentifierName("isnull")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    IdentifierName(m.Identifier.Text))))),
                                                    InterpolatedStringExpression(
                                                        Token(SyntaxKind.InterpolatedStringStartToken))
                                                    .WithContents(
                                                        SingletonList<InterpolatedStringContentSyntax>(
                                                            InterpolatedStringText()
                                                            .WithTextToken(
                                                                Token(
                                                                    TriviaList(),
                                                                    SyntaxKind.InterpolatedStringTextToken,
                                                                    $"{m.Identifier.Text}: [null]",
                                                                    $"{m.Identifier.Text}: [null]",
                                                                    TriviaList())))),
                                                    InterpolatedStringExpression(
                                                        Token(SyntaxKind.InterpolatedStringStartToken))
                                                    .WithContents(
                                                        List<InterpolatedStringContentSyntax>(
                                                            new InterpolatedStringContentSyntax[]{
                                                                InterpolatedStringText()
                                                                .WithTextToken(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.InterpolatedStringTextToken,
                                                                        $"{m.Identifier.Text}: ",
                                                                        $"{m.Identifier.Text}: ",
                                                                        TriviaList())),
                                                                Interpolation(
                                                                    IdentifierName(m.Identifier.Text))})))))))),
                            comma }));

                // Remove last comma
                statements.RemoveAt(statements.Count - 1);

                statements.Add(
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("sb"),
                                IdentifierName("Append")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(")"))))))));

                statements.Add(
                    ReturnStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("sb"),
                                IdentifierName("ToString")))));

            }

            return MethodDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.StringKeyword)),
                        Identifier("ToString"))
                    .WithModifiers(TokenList( new[]{ Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)}))
                    .WithBody(
                        Block(statements));
        }

        static MemberDeclarationSyntax MakeGetHashCode(List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members)
        {
            BlockSyntax block = null;

            if (members.Count == 0)
            {
                block = Block(
                        SingletonList<StatementSyntax>(
                            ReturnStatement(
                                LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    Literal(0)))));
            }
            else
            {
                var statements = new List<StatementSyntax>();
                statements.AddRange(members.Select(m =>
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName("state"),
                                        BinaryExpression(
                                            SyntaxKind.MultiplyExpression,
                                            ParenthesizedExpression(
                                                BinaryExpression(
                                                    SyntaxKind.ExclusiveOrExpression,
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            DefaultExpression(EqDefaultType(m.Type)),
                                                            IdentifierName("GetHashCode")))
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SingletonSeparatedList<ArgumentSyntax>(
                                                                Argument(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        ThisExpression(),
                                                                        IdentifierName(MakeFirstCharUpper(m.Identifier.Text))))))),
                                                    IdentifierName("state"))),
                                            IdentifierName("fnvPrime"))))));

                block = Block(
                            LocalDeclarationStatement(
                                VariableDeclaration(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword)))
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier("fnvOffsetBasis"))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                PrefixUnaryExpression(
                                                    SyntaxKind.UnaryMinusExpression,
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(2128831035))))))))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.ConstKeyword))),
                            LocalDeclarationStatement(
                                VariableDeclaration(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword)))
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier("fnvPrime"))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                LiteralExpression(
                                                    SyntaxKind.NumericLiteralExpression,
                                                    Literal(16777619)))))))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.ConstKeyword))),
                            LocalDeclarationStatement(
                                VariableDeclaration(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword)))
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier("state"))
                                        .WithInitializer(
                                            EqualsValueClause(
                                                IdentifierName("fnvOffsetBasis")))))),
                            CheckedStatement(
                                SyntaxKind.UncheckedStatement,
                                Block(statements)),
                            ReturnStatement(
                                IdentifierName("state")));
            }

            return MethodDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.IntKeyword)),
                            Identifier("GetHashCode"))
                        .WithModifiers(
                            TokenList(
                                new[]{
                                        Token(SyntaxKind.PublicKeyword),
                                        Token(SyntaxKind.OverrideKeyword)}))
                        .WithBody(block);
            }

        static IEnumerable<MemberDeclarationSyntax> MakeOrderingMembers(TypeSyntax thisType, TypeSyntax baseType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members, BaseSpec baseSpec)
        {
            var ords = new List<MemberDeclarationSyntax>();

            ords.Add(MethodDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.IntKeyword)),
                            Identifier("CompareTo"))
                        .WithModifiers(
                                baseSpec == BaseSpec.Abstract
                                    ? TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
                                    : TokenList(Token(SyntaxKind.PublicKeyword)))
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
                                ConditionalExpression(
                                    IsPatternExpression(
                                        IdentifierName("obj"),
                                        DeclarationPattern(
                                            baseType,
                                            SingleVariableDesignation(
                                                Identifier("p")))),
                                    InvocationExpression(
                                        IdentifierName("CompareTo"))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    IdentifierName("p"))))),
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(1)))))
                        .WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken)));

            if (baseSpec == BaseSpec.Abstract)
            {
                ords.Add(MethodDeclaration(
                                PredefinedType(
                                    Token(SyntaxKind.IntKeyword)),
                                Identifier("CompareTo"))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList<ParameterSyntax>(
                                        Parameter(
                                            Identifier("obj"))
                                        .WithType(baseType))))
                            .WithExpressionBody(
                                ArrowExpressionClause(
                                    ConditionalExpression(
                                        IsPatternExpression(
                                            IdentifierName("obj"),
                                            DeclarationPattern(
                                                thisType,
                                                SingleVariableDesignation(
                                                    Identifier("tobj")))),
                                        InvocationExpression(
                                            IdentifierName("CompareTo"))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SingletonSeparatedList<ArgumentSyntax>(
                                                    Argument(
                                                        IdentifierName("tobj"))))),
                                        ConditionalExpression(
                                            IsPatternExpression(
                                                IdentifierName("obj"),
                                                ConstantPattern(
                                                    LiteralExpression(
                                                        SyntaxKind.NullLiteralExpression))),
                                            LiteralExpression(
                                                SyntaxKind.NumericLiteralExpression,
                                                Literal(1)),
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("@Tag"),
                                                    IdentifierName("CompareTo")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("obj"),
                                                                IdentifierName("@Tag"))))))))))
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken)));
            }

            if (members.Count == 0)
            {
                ords.Add(MethodDeclaration(
                                        PredefinedType(
                                            Token(SyntaxKind.IntKeyword)),
                                        Identifier("CompareTo"))
                                    .WithModifiers(
                                        TokenList(Token(SyntaxKind.PublicKeyword)))
                                    .WithParameterList(
                                        ParameterList(
                                            SingletonSeparatedList(
                                                Parameter(
                                                    Identifier("other"))
                                                .WithType(thisType))))
                                    .WithBody(
                                        Block(
                                            IfStatement(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("LanguageExt"),
                                                            IdentifierName("Prelude")),
                                                        IdentifierName("isnull")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("other"))))),
                                                ReturnStatement(
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(1)))),
                                            ReturnStatement(
                                                LiteralExpression(
                                                    SyntaxKind.NumericLiteralExpression,
                                                    Literal(0))))));

                return ords;
            }
            else
            {
                var statements = new List<StatementSyntax>();

                statements.Add(
                    IfStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("LanguageExt"),
                                    IdentifierName("Prelude")),
                                IdentifierName("isnull")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            IdentifierName("other"))))),
                        ReturnStatement(
                            LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                Literal(1)))));

                statements.Add(
                    LocalDeclarationStatement(
                        VariableDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.IntKeyword)))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("cmp"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        LiteralExpression(
                                            SyntaxKind.NumericLiteralExpression,
                                            Literal(0))))))));

                statements.AddRange(
                    members.SelectMany(m =>
                        new StatementSyntax[] {
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("cmp"),
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            DefaultExpression(OrdDefaultType(m.Type)),
                                            IdentifierName("Compare")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            ThisExpression(),
                                                            IdentifierName(MakeFirstCharUpper(m.Identifier.Text)))),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("other"),
                                                            IdentifierName(MakeFirstCharUpper(m.Identifier.Text))))}))))),
                            IfStatement(
                                BinaryExpression(
                                    SyntaxKind.NotEqualsExpression,
                                    IdentifierName("cmp"),
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(0))),
                                ReturnStatement(
                                    IdentifierName("cmp")))
                                }));

                statements.Add(ReturnStatement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0))));

                ords.Add(MethodDeclaration(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword)),
                                    Identifier("CompareTo"))
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PublicKeyword)))
                                .WithParameterList(
                                    ParameterList(
                                        SingletonSeparatedList(
                                            Parameter(
                                                Identifier("other"))
                                            .WithType(thisType))))
                                .WithBody(Block(statements)));

                return ords;
            }
        }

        static IEnumerable<MemberDeclarationSyntax> MakeEqualityMembers(TypeSyntax thisType, TypeSyntax baseType, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members, BaseSpec baseSpec)
        {
            var statements = new List<StatementSyntax>();

            statements.Add(
                IfStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("LanguageExt"),
                                IdentifierName("Prelude")),
                            IdentifierName("isnull")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        IdentifierName("other"))))),
                    ReturnStatement(
                        LiteralExpression(
                            SyntaxKind.FalseLiteralExpression))));

            statements.AddRange(
                members.Select(m =>
                    IfStatement(
                        PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    DefaultExpression(EqDefaultType(m.Type)),
                                    IdentifierName("Equals")))
                            .WithArgumentList(
                                ArgumentList(
                                    SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]{
                                            Argument(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName(MakeFirstCharUpper(m.Identifier.Text)))),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("other"),
                                                    IdentifierName(MakeFirstCharUpper(m.Identifier.Text))))})))),
                        ReturnStatement(
                            LiteralExpression(
                                SyntaxKind.FalseLiteralExpression)))));

            statements.Add(ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression)));

            var eqTyped = MethodDeclaration(
                                PredefinedType(
                                    Token(SyntaxKind.BoolKeyword)),
                                Identifier("Equals"))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword)))
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList(Parameter(Identifier("other")).WithType(thisType))))
                            .WithBody(
                                Block(statements));

            var eqUntyped = MethodDeclaration(
                                    PredefinedType(
                                        Token(SyntaxKind.BoolKeyword)),
                                    Identifier("Equals"))
                                .WithModifiers(
                                    TokenList(
                                        new[]{
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
                                                    thisType,
                                                    SingleVariableDesignation(
                                                        Identifier("tobj")))),
                                            InvocationExpression(
                                                IdentifierName("Equals"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(
                                                            IdentifierName("tobj"))))))))
                                .WithSemicolonToken(
                                    Token(SyntaxKind.SemicolonToken));

            if (baseSpec == BaseSpec.Interface || baseSpec == BaseSpec.None)
            {
                return new[] { eqTyped, eqUntyped };
            }
            else
            {
                var eqBaseTyped = MethodDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.BoolKeyword)),
                        Identifier("Equals"))
                    .WithModifiers(
                        TokenList(
                            new[]{
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.OverrideKeyword)}))
                    .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList<ParameterSyntax>(
                                Parameter(
                                    Identifier("obj"))
                                .WithType(baseType))))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            BinaryExpression(
                                SyntaxKind.LogicalAndExpression,
                                IsPatternExpression(
                                    IdentifierName("obj"),
                                    DeclarationPattern(
                                        thisType,
                                        SingleVariableDesignation(
                                            Identifier("tobj")))),
                                InvocationExpression(
                                    IdentifierName("Equals"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                IdentifierName("tobj"))))))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken));

                return new[] { eqTyped, eqUntyped, eqBaseTyped };
            }
        }

        static IEnumerable<MemberDeclarationSyntax> MakeStructOperatorMembers(TypeSyntax thisType) =>
            new MemberDeclarationSyntax[]{
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
                                    .WithType(thisType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(thisType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("x"),
                                    IdentifierName("Equals")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            IdentifierName("y")))))))
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
                                    .WithType(thisType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(thisType)})))
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
                                    .WithType(thisType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(thisType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
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
                                    Literal(0)))))
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
                                    .WithType(thisType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(thisType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
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
                                    Literal(0)))))
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
                                    .WithType(thisType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(thisType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
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
                                    Literal(0)))))
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
                                    .WithType(thisType),
                                    Token(SyntaxKind.CommaToken),
                                    Parameter(
                                        Identifier("y"))
                                    .WithType(thisType)})))
                    .WithExpressionBody(
                        ArrowExpressionClause(
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
                                    Literal(0)))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken)) };

        static IEnumerable<MemberDeclarationSyntax> MakeClassOperatorMembers(TypeSyntax thisType)
        {
            var eqeq = OperatorDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.BoolKeyword)),
                            Token(SyntaxKind.EqualsEqualsToken))
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
                                                    Identifier("x"))
                                                .WithType(
                                                    thisType),
                                                Token(SyntaxKind.CommaToken),
                                                Parameter(
                                                    Identifier("y"))
                                                .WithType(
                                                    thisType)})))
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
                            Token(SyntaxKind.SemicolonToken));

            var noteq = OperatorDeclaration(
                                PredefinedType(
                                    Token(SyntaxKind.BoolKeyword)),
                                Token(SyntaxKind.ExclamationEqualsToken))
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
                                                Identifier("x"))
                                            .WithType(
                                                thisType),
                                            Token(SyntaxKind.CommaToken),
                                            Parameter(
                                                Identifier("y"))
                                            .WithType(
                                                thisType)})))
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
                                Token(SyntaxKind.SemicolonToken));

            var gt = OperatorDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.BoolKeyword)),
                            Token(SyntaxKind.GreaterThanToken))
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
                                            Identifier("x"))
                                        .WithType(
                                            thisType),
                                        Token(SyntaxKind.CommaToken),
                                        Parameter(
                                            Identifier("y"))
                                        .WithType(
                                            thisType)})))
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
                            Token(SyntaxKind.SemicolonToken));

            var gte = OperatorDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.BoolKeyword)),
                            Token(SyntaxKind.GreaterThanEqualsToken))
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
                                            Identifier("x"))
                                        .WithType(
                                            thisType),
                                        Token(SyntaxKind.CommaToken),
                                        Parameter(
                                            Identifier("y"))
                                        .WithType(
                                            thisType)})))
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
                            Token(SyntaxKind.SemicolonToken));

            var lt = OperatorDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.BoolKeyword)),
                            Token(SyntaxKind.LessThanToken))
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
                                            Identifier("x"))
                                        .WithType(
                                            thisType),
                                        Token(SyntaxKind.CommaToken),
                                        Parameter(
                                            Identifier("y"))
                                        .WithType(
                                            thisType)})))
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
                            Token(SyntaxKind.SemicolonToken));

            var lte = OperatorDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.BoolKeyword)),
                            Token(SyntaxKind.LessThanEqualsToken))
                        .WithModifiers(
                            TokenList(
                                new[] {
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.StaticKeyword) }))
                        .WithParameterList(
                            ParameterList(
                                SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[] {
                                        Parameter(
                                            Identifier("x"))
                                        .WithType(
                                            thisType),
                                        Token(SyntaxKind.CommaToken),
                                        Parameter(
                                            Identifier("y"))
                                        .WithType(
                                            thisType) })))
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
                            Token(SyntaxKind.SemicolonToken));

            return new[] { eqeq, noteq, gt, lt, gte, lte };
        }

        static MemberDeclarationSyntax[] MakeSerialisationMembers(string typeName, List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> members)
        {
            var gets = members.Select(m =>
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    ThisExpression(),
                                    IdentifierName(MakeFirstCharUpper(m.Identifier.Text))),
                                CastExpression(
                                    m.Type,
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("info"),
                                            IdentifierName("GetValue")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SeparatedList<ArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    Argument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(m.Identifier.Text))),
                                                    Token(SyntaxKind.CommaToken),
                                                    Argument(
                                                        TypeOfExpression(m.Type))})))))));

            var sets = members.Select(m =>
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("info"),
                                    IdentifierName("AddValue")))
                            .WithArgumentList(
                                ArgumentList(
                                    SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]{
                                            Argument(
                                                LiteralExpression(
                                                    SyntaxKind.StringLiteralExpression,
                                                    Literal(m.Identifier.Text))),
                                            Token(SyntaxKind.CommaToken),
                                            Argument(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName(MakeFirstCharUpper(m.Identifier.Text))))})))));

            var ctor = ConstructorDeclaration(
                            Identifier(typeName))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword)))
                        .WithParameterList(
                            ParameterList(
                                SeparatedList<ParameterSyntax>(
                                    new SyntaxNodeOrToken[]{
                                                Parameter(
                                                    Identifier("info"))
                                                .WithType(
                                                    QualifiedName(
                                                        QualifiedName(
                                                            QualifiedName(
                                                                IdentifierName("System"),
                                                                IdentifierName("Runtime")),
                                                            IdentifierName("Serialization")),
                                                        IdentifierName("SerializationInfo"))),
                                                Token(SyntaxKind.CommaToken),
                                                Parameter(
                                                    Identifier("context"))
                                                .WithType(
                                                    QualifiedName(
                                                        QualifiedName(
                                                            QualifiedName(
                                                                IdentifierName("System"),
                                                                IdentifierName("Runtime")),
                                                            IdentifierName("Serialization")),
                                                        IdentifierName("StreamingContext")))})))
                        .WithBody(
                            Block(gets));

            var getObjData = MethodDeclaration(
                                 PredefinedType(
                                     Token(SyntaxKind.VoidKeyword)),
                                 Identifier("GetObjectData"))
                             .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword)))
                             .WithParameterList(
                                 ParameterList(
                                     SeparatedList<ParameterSyntax>(
                                         new SyntaxNodeOrToken[]{
                                             Parameter(
                                                 Identifier("info"))
                                             .WithType(
                                                 QualifiedName(
                                                     QualifiedName(
                                                         QualifiedName(
                                                             IdentifierName("System"),
                                                             IdentifierName("Runtime")),
                                                         IdentifierName("Serialization")),
                                                     IdentifierName("SerializationInfo"))),
                                             Token(SyntaxKind.CommaToken),
                                             Parameter(
                                                 Identifier("context"))
                                             .WithType(
                                                 QualifiedName(
                                                     QualifiedName(
                                                         QualifiedName(
                                                             IdentifierName("System"),
                                                             IdentifierName("Runtime")),
                                                         IdentifierName("Serialization")),
                                                     IdentifierName("StreamingContext")))})))
                             .WithBody(
                                 Block(sets));

            return new MemberDeclarationSyntax[] { ctor, getObjData };
        }

        public static TypeSyntax EqDefaultType(TypeSyntax genericParam) =>
            QualifiedName(
                QualifiedName(
                    IdentifierName("LanguageExt"),
                    IdentifierName("ClassInstances")),
                GenericName(
                    Identifier("EqDefault"))
                .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList(genericParam))));

        public static TypeSyntax OrdDefaultType(TypeSyntax genericParam) =>
            QualifiedName(
                QualifiedName(
                    IdentifierName("LanguageExt"),
                    IdentifierName("ClassInstances")),
                GenericName(
                    Identifier("OrdDefault"))
                .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList(genericParam))));

        /// <summary>
        /// Makes a case class
        /// This can be a case within a union, or a standalone record (which can be seen as a union 
        /// type with 1 case)
        /// 
        /// In the parameter list, below, anything prefixed with `applyTo` represents either the record-type
        /// or the union-type (not the case).  Anything prefixed with `case` contains information relating to
        /// the case, which for record types is also the record.
        /// 
        /// </summary>
        public static TypeDeclarationSyntax MakeCaseType(
            TransformationContext context, 
            SyntaxToken applyToIdentifier,
            SyntaxList<MemberDeclarationSyntax> applyToMembers,
            TypeParameterListSyntax applyToTypeParams,
            SyntaxTokenList applyToModifiers,
            SyntaxList<TypeParameterConstraintClauseSyntax> applyToConstraints,
            SyntaxToken caseIdentifier,
            TypeParameterListSyntax caseTypeParams,
            List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> caseParams,
            BaseSpec baseSpec,
            bool caseIsClass,
            bool caseIsPartial,
            int tag)
        {
            var lmodifiers = applyToModifiers.Where(t => !t.IsKind(SyntaxKind.PartialKeyword) && !t.IsKind(SyntaxKind.AbstractKeyword))
                                            .ToList();

            if (caseIsClass)
            {
                lmodifiers.Add(Token(SyntaxKind.SealedKeyword));
            }
            if(caseIsPartial)
            {
                lmodifiers.Add(Token(SyntaxKind.PartialKeyword));
            }

            var modifiers = TokenList(lmodifiers);

            var type = caseIsClass 
                ? ClassDeclaration(caseIdentifier.Text) 
                : StructDeclaration(caseIdentifier.Text) as TypeDeclarationSyntax;

            type = type.WithModifiers(modifiers)
                       .WithAttributeLists(
                           SingletonList(
                               AttributeList(
                                   SingletonSeparatedList(
                                       Attribute(
                                           QualifiedName(
                                               IdentifierName("System"),
                                               IdentifierName("Serializable")))))));

            var typeParamList = applyToTypeParams;
            if(caseTypeParams != null)
            {
                typeParamList = typeParamList == null
                    ? caseTypeParams
                    : typeParamList.AddParameters(caseTypeParams.Parameters.ToArray());
            }

            if (typeParamList != null)
            {
                type = type.WithTypeParameterList(typeParamList);
            }

            if (applyToConstraints != null)
            {
                type = type.WithConstraintClauses(applyToConstraints);
            }

            var abstractBaseType = ParseTypeName($"_{applyToIdentifier}Base{applyToTypeParams}");
            var interfaceType =     ParseTypeName($"{applyToIdentifier}{applyToTypeParams}");
            var thisType =       ParseTypeName($"{caseIdentifier.Text}{typeParamList}");
            var thisEquatableType = ParseTypeName($"System.IEquatable<{caseIdentifier.Text}{typeParamList}>");
            var thisComparableType = ParseTypeName($"System.IComparable<{caseIdentifier.Text}{typeParamList}>");
            var comparableType = ParseTypeName($"System.IComparable");
            var serializableType = ParseTypeName($"System.Runtime.Serialization.ISerializable");

            var ctor = MakeConstructor(caseIdentifier.Text, caseParams);
            var dtor = MakeDeconstructor(caseParams);

            var fields = baseSpec == BaseSpec.None
                ? new List<MemberDeclarationSyntax>()
                : caseParams.Select(p => MakeField(p.Identifier.Text, p.Type))
                            .ToList();

            if (baseSpec == BaseSpec.Abstract)
            {
                var tagProp = PropertyDeclaration(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword)),
                                    Identifier("@Tag"))
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

                fields.Add(tagProp);
            }

            var publicMod = TokenList(Token(SyntaxKind.PublicKeyword));

            var impl = new List<MemberDeclarationSyntax>();
            if (baseSpec == BaseSpec.Interface)
            {
                impl.AddRange(
                    applyToMembers
                        .Where(m => m is MethodDeclarationSyntax)
                        .Select(m => m as MethodDeclarationSyntax)
                        .Select(m => MakeExplicitInterfaceImpl(interfaceType, thisType, m.Identifier, m.ParameterList, m.TypeParameterList)));
            }

            var dtype = MakeDataTypeMembers(caseIdentifier.Text, thisType, interfaceType, caseParams, baseSpec, caseIsClass);

            fields.Add(ctor);
            fields.Add(dtor);
            fields.AddRange(dtype);
            fields.AddRange(impl);

            type = type.WithMembers(List(fields));

            // Derive from Record<UnionBaseType> and UnionBaseType
            type = baseSpec switch
            {
                BaseSpec.Interface =>
                    type.WithBaseList(
                        BaseList(
                            SeparatedList<BaseTypeSyntax>(
                                new SyntaxNodeOrToken[]{
                                    SimpleBaseType(interfaceType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(thisEquatableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(thisComparableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(comparableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(serializableType)
                                }))),
                BaseSpec.Abstract =>
                    type.WithBaseList(
                        BaseList(
                            SeparatedList<BaseTypeSyntax>(
                                new SyntaxNodeOrToken[]{
                                    SimpleBaseType(abstractBaseType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(thisEquatableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(thisComparableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(comparableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(serializableType)
                                }))),
                _ =>
                    type.WithBaseList(
                        BaseList(
                            SeparatedList<BaseTypeSyntax>(
                                new SyntaxNodeOrToken[]{
                                    SimpleBaseType(thisEquatableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(thisComparableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(comparableType),
                                    Token(SyntaxKind.CommaToken),
                                    SimpleBaseType(serializableType)
                                })))
            };

            type = AddWith(context, type, thisType, caseParams);
            type = AddLenses(type, thisType, caseParams);

            return type;
        }

        static MemberDeclarationSyntax MakeConstructor(
            string ctorName, 
            List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> fields
            )
        {
            // Make the parameters start with an upper case letter and have the out modifier
            var parameters = fields.Select(f => Parameter(Identifier(MakeFirstCharUpper(f.Identifier.Text)))
                                                    .WithType(f.Type))
                                   .ToArray();

            var assignments = parameters.Select(p =>
                ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(MakeFirstCharUpper(p.Identifier.Text))),
                        IdentifierName(p.Identifier.Text))));


            return ConstructorDeclaration(Identifier(ctorName))
                        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                        .WithParameterList(ParameterList(SeparatedList(parameters)))
                        .WithBody(Block(List(assignments)));
        }


        static MethodDeclarationSyntax MakeDeconstructor(
            List<(SyntaxToken Identifier, TypeSyntax Type, SyntaxTokenList Modifiers)> fields
            )
        {
            var assignments = fields.Select(p =>
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName(MakeFirstCharUpper(p.Identifier.Text)),
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName(MakeFirstCharUpper(p.Identifier.Text)))
                                                )))
                                    .ToArray();

            // Make the parameters start with an upper case letter and have the out modifier
            var parameters = fields.Select(f => Parameter(Identifier(MakeFirstCharUpper(f.Identifier.Text)))
                                                    .WithType(f.Type)
                                                    .WithModifiers(TokenList(Token(SyntaxKind.OutKeyword))))
                                   .ToArray();

            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Deconstruct"))
                       .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                       .WithParameterList(ParameterList(SeparatedList(parameters)))
                       .WithBody(Block(assignments));
        }

        static MemberDeclarationSyntax MakeField(string name, TypeSyntax type)
        {
            var fieldName = MakeFirstCharUpper(name);

            var field = FieldDeclaration(
                            VariableDeclaration(type)
                                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(fieldName)))))
                            .WithModifiers(
                                TokenList(
                                    new[]{
                                        Token(SyntaxKind.PublicKeyword),
                                        Token(SyntaxKind.ReadOnlyKeyword)}))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)); ;

            return field;
        }

        static MemberDeclarationSyntax MakeExplicitInterfaceImpl(
            TypeSyntax interfaceType, 
            TypeSyntax resultType, 
            SyntaxToken identifier, 
            ParameterListSyntax caseParams, 
            TypeParameterListSyntax caseTypeParams)
        {

            var method = MethodDeclaration(interfaceType, identifier)
                            .WithExplicitInterfaceSpecifier(
                                ExplicitInterfaceSpecifier(ParseName(interfaceType.ToString())))
                            .WithParameterList(caseParams)
                            .WithExpressionBody(
                                ArrowExpressionClause(
                                    ThrowExpression(
                                        ObjectCreationExpression(
                                            QualifiedName(
                                                IdentifierName("System"),
                                                IdentifierName("NotSupportedException")))
                                        .WithArgumentList(ArgumentList()))))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            if (caseTypeParams != null)
            {
                method = method.WithTypeParameterList(caseTypeParams);
            }
            return method;
        }
    }
}
