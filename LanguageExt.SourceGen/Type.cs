using LanguageExt.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.SourceGen;

public static class Type
{
    static readonly string[] derivingNames =
    {
        "Eq", 
        "Ord",
        "Show",
        "Read",
        "Generic",
        "Lens",
        "Functor",
        "Applicative",
        "Monad",
        "Record",
        "Union"
    };
    
    public static bool IsPartial(TypeDeclarationSyntax type) =>
        type.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

    public static bool IsInstance(TypeDeclarationSyntax type) =>
        !IsStatic(type);

    public static bool IsStatic(TypeDeclarationSyntax type) =>
        type.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));

    public static bool HasTypeNameOf(TypeSyntax type, params string[] names) =>
        type switch
        {
            SimpleNameSyntax t => names.Contains(t.Identifier.ToString()),
            QualifiedNameSyntax t when HasTypeNameOf(t.Right, names) => t.Left.ToString() == "LanguageExt",
            _ => false
        };

    public static IEnumerable<TypeSyntax> BaseTypes(TypeDeclarationSyntax type) =>
        type switch
        {
            ClassDeclarationSyntax t => t.BaseList?.Types.Select(t1 => t1.Type) ?? Array.Empty<TypeSyntax>(),
            InterfaceDeclarationSyntax t => t.BaseList?.Types.Select(t1 => t1.Type) ?? Array.Empty<TypeSyntax>(),
            StructDeclarationSyntax t => t.BaseList?.Types.Select(t1 => t1.Type) ?? Array.Empty<TypeSyntax>(),
            RecordDeclarationSyntax t => t.BaseList?.Types.Select(t1 => t1.Type) ?? Array.Empty<TypeSyntax>(),
            _ => Array.Empty<TypeSyntax>()
        };

    public static bool IsDerivingType(TypeDeclarationSyntax type) =>
        BaseTypes(type).Any(IsDerivingType);
    
    public static bool IsDerivingType(TypeSyntax type) =>
        HasTypeNameOf(type, "Deriving");
    
    public static bool IsKnownDeriving(TypeSyntax type) =>
        HasTypeNameOf(type, derivingNames);

    public static Ty<SyntaxNode> Convert(TypeSyntax type) =>
        throw new NotImplementedException();
        //Decl<CSharpSyntaxNode>.Annotate(type, throw new NotImplementedException());
}
