using LanguageExt.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static LanguageExt.SourceGen.Type;

namespace LanguageExt.SourceGen;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<Unit> p = context.SyntaxProvider.CreateSyntaxProvider(
            
            );
        context.RegisterImplementationSourceOutput(p);
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var types = MapTypes(FindTypes(context.Compilation.SyntaxTrees)).ToArray();
        Gen.walk(types).Run(new GenEnv<SyntaxNode>(context, Stack<SyntaxNode>.Empty));
        context.AddSource("testing123.generated.cs", "using System.Bah");
    }

    static IEnumerable<TypeDeclarationSyntax> FindTypes(IEnumerable<SyntaxTree> trees) =>
        trees.SelectMany(FindTypes);

    static IEnumerable<TypeDeclarationSyntax> FindTypes(SyntaxTree tree) =>
        tree.GetRoot()
            .DescendantNodes()
            .SelectMany(FindTypes);

    static IEnumerable<TypeDeclarationSyntax> FindTypes(SyntaxNode node) =>
        node switch
        {
            RecordDeclarationSyntax @record when IsPartial(@record) => FindTypesAndSubTypes(@record),
            ClassDeclarationSyntax @class when IsPartial(@class)    => FindTypesAndSubTypes(@class),
            StructDeclarationSyntax @struct when IsPartial(@struct) => FindTypes(@struct),
            InterfaceDeclarationSyntax @interface                   => FindTypes(@interface),
            NamespaceDeclarationSyntax @namespace                   => @namespace.Members.SelectMany(FindTypes), 
            _                                                       => Array.Empty<TypeDeclarationSyntax>()
        };

    static IEnumerable<TypeDeclarationSyntax> FindTypesAndSubTypes(TypeDeclarationSyntax node) =>
        IsPartial(node) && IsInstance(node) && IsDerivingType(node)
            ? new[] {node}.Concat(node.DescendantNodes().SelectMany(FindTypes))
            : node.DescendantNodes().SelectMany(FindTypes);

    static IEnumerable<TypeDeclarationSyntax> FindTypes(TypeDeclarationSyntax node) =>
        IsPartial(node) && IsInstance(node) && IsDerivingType(node)
            ? new [] { node }
            : Array.Empty<TypeDeclarationSyntax>();

    static IEnumerable<Decl<SyntaxNode>> MapTypes(IEnumerable<TypeDeclarationSyntax> decls) =>
        decls.SelectMany(MapTypes);
    
    static IEnumerable<Decl<SyntaxNode>> MapTypes(TypeDeclarationSyntax decl) =>
        decl switch
        {
            ClassDeclarationSyntax @class         => MapTypes(@class),
            /*
            RecordDeclarationSyntax @record       => MapTypes(@record),
            InterfaceDeclarationSyntax @interface => FindTypes(@interface),
            StructDeclarationSyntax @struct       => FindTypes(@struct),
            */
            _                                     => Array.Empty<Decl<SyntaxNode>>()
        };

    static IEnumerable<Decl<SyntaxNode>> MapTypes(ClassDeclarationSyntax @class) =>
        Record.Make(
            @class,
            @class.Identifier.ToString(), 
            @class.Members.ToArray(), 
            @class.TypeParameterList?.Parameters.ToArray() ?? Array.Empty<TypeParameterSyntax>(),
            FindDerivings(@class.BaseList?.Types.Select(t => t.Type).ToArray() ?? Array.Empty<TypeSyntax>()));

    static IEnumerable<string> TypeIdent(TypeSyntax type) =>
        type switch
        {
            GenericNameSyntax gn => new[] {gn.Identifier.ToString()},
            QualifiedNameSyntax qn => TypeIdent(qn.Right),
            _ => Array.Empty<string>()
        };

    static IEnumerable<TypeSyntax> TypeArguments(TypeSyntax type) =>
        type switch
        {
            GenericNameSyntax gn => gn.TypeArgumentList.Arguments,
            QualifiedNameSyntax qn => TypeArguments(qn.Right),
            _ => Array.Empty<TypeSyntax>()
        };

    static IEnumerable<Deriving> FindDerivings(IEnumerable<TypeSyntax> types) =>
        types
            .Where(IsDerivingType)
            .SelectMany(TypeArguments)
            .Where(IsKnownDeriving)
            .SelectMany(TypeIdent)
            .Select(Deriving.FromString);
}
