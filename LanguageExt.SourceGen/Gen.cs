using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.SourceGen;

public class Gen : ISourceGenerator
{
    static readonly string[] attrNames =
    {
        "Union", 
        "Record", 
        "Free", 
        "Reader", 
        "RWS"
    }; 
    
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var types = FindTypes(context.Compilation.SyntaxTrees).ToArray();
        
        // TODO: Use the types to gen code
    }

    static IEnumerable<MemberDeclarationSyntax> FindTypes(IEnumerable<SyntaxTree> trees) =>
        trees.SelectMany(FindTypes);

    static IEnumerable<MemberDeclarationSyntax> FindTypes(SyntaxTree tree) =>
        tree.GetRoot()
            .DescendantNodes()
            .SelectMany(FindTypes);

    static IEnumerable<MemberDeclarationSyntax> FindTypes(SyntaxNode node) =>
        node switch
        {
            RecordDeclarationSyntax @record       => FindTypesAndSubTypes(@record),
            ClassDeclarationSyntax @class         => FindTypesAndSubTypes(@class),
            InterfaceDeclarationSyntax @interface => FindTypes(@interface),
            StructDeclarationSyntax @struct       => FindTypes(@struct),
            NamespaceDeclarationSyntax @namespace => @namespace.Members.SelectMany(FindTypes), 
            _                                     => Array.Empty<MemberDeclarationSyntax>()
        };

    static IEnumerable<MemberDeclarationSyntax> FindTypesAndSubTypes(MemberDeclarationSyntax node) =>
        node.AttributeLists.Any(als => als.Attributes.Any(a => HasKnownName(a.Name)))
            ? new[] {node}.Concat(node.DescendantNodes().SelectMany(FindTypes))
            : node.DescendantNodes().SelectMany(FindTypes);

    static IEnumerable<MemberDeclarationSyntax> FindTypes(MemberDeclarationSyntax node) =>
        node.AttributeLists.Any(als => als.Attributes.Any(a => HasKnownName(a.Name)))
            ? new [] { node }
            : Array.Empty<MemberDeclarationSyntax>();

    static bool HasKnownName(NameSyntax name) =>
        name switch
        {
            SimpleNameSyntax n => attrNames.Contains(n.ToString()),
            QualifiedNameSyntax qn when HasKnownName(qn.Right) => qn.Left.ToString() == "LanguageExt",
            _ => false
        };
}
