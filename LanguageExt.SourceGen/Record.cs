using LanguageExt.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.SourceGen;

public static class Record
{
    public static IEnumerable<Decl<SyntaxNode>> Make(
        TypeDeclarationSyntax type,
        string name,
        IEnumerable<MemberDeclarationSyntax> members,
        IEnumerable<TypeParameterSyntax> @params,
        IEnumerable<Deriving> derivings)
    {
        var fields = Record.GetFields(members)
            .Select(MakeField)
            .ToArray();

        var ps = @params
            .Select(p => p.Identifier.ToString())
            .Select(Ty<SyntaxNode>.Var)
            .ToArray();

        return new[] {Decl<SyntaxNode>.Annotate(type, Decl<SyntaxNode>.Record(fields, ps, derivings))};
    }

    public static Decl<SyntaxNode> MakeField(MemberDeclarationSyntax m) =>
        Decl<SyntaxNode>.Annotate(m, Decl<SyntaxNode>.Label(Member.Name(m), Type.Convert(Member.Type(m))));    
    
    /// <summary>
    /// Finds the fields and properties that are usable in a record
    /// </summary>
    /// <remarks>
    ///
    ///     Fields must be:
    ///         * public
    ///         * readonly
    ///         * instance (not static)    
    ///         * Be upper-case
    ///
    ///     Properties must be:
    ///         * public
    ///         * Be upper-case
    ///         * Have implicit getter only (i.e. be field-backed) 
    ///         * instance (not static)
    ///  
    /// </remarks>
    public static IEnumerable<MemberDeclarationSyntax> GetFields(TypeDeclarationSyntax type) =>
        GetFields(type.Members);
        
    /// <summary>
    /// Finds the fields and properties that are usable in a record
    /// </summary>
    /// <remarks>
    ///
    ///     Fields must be:
    ///         * public
    ///         * readonly
    ///         * instance (not static)    
    ///         * Be upper-case
    ///
    ///     Properties must be:
    ///         * public
    ///         * Be upper-case
    ///         * Have implicit getter only (i.e. be field-backed) 
    ///         * instance (not static)
    ///  
    /// </remarks>
    public static IEnumerable<MemberDeclarationSyntax> GetFields(IEnumerable<MemberDeclarationSyntax> members)
    {
        // Provide an index for each member
        var indexedMembers = members.Select((m, i) => (m, i)).ToList();

        var fields = indexedMembers
            .Where(m => m.m is FieldDeclarationSyntax f)
            .Select(m => (f: m.m as FieldDeclarationSyntax ?? throw new InvalidCastException(), m.i))
            .Where(m => m.f.Declaration.Variables.Count > 0 &&
                        Text.FirstCharIsUpper(m.f.Declaration.Variables[0].Identifier.ToString()) &&
                        m.f.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                        m.f.Modifiers.Any(SyntaxKind.ReadOnlyKeyword) &&
                        !m.f.Modifiers.Any(SyntaxKind.StaticKeyword))
            .Select(m => (m: (MemberDeclarationSyntax)m.f, i: m.i))
            .ToList();

        var properties = indexedMembers
            .Where(m => m.m is PropertyDeclarationSyntax)
            .Select(m => (p: m.m as PropertyDeclarationSyntax ?? throw new InvalidCastException(), m.i))
            .Where(m => Text.FirstCharIsUpper(m.p.Identifier.ToString()))
            .Where(m => m.p.Modifiers.Any(SyntaxKind.PublicKeyword))
            .Where(m => !m.p.Modifiers.Any(SyntaxKind.StaticKeyword))
            .Where(m => m.p.AccessorList is not null &&
                        m.p.AccessorList.Accessors.Count == 1 &&
                        m.p.AccessorList.Accessors[0].Kind() == SyntaxKind.GetAccessorDeclaration &&
                        m.p.AccessorList.Accessors[0].ExpressionBody == null &&
                        m.p.AccessorList.Accessors[0].Body == null &&
                        m.p.Initializer == null)
            .Select(m => (m: (MemberDeclarationSyntax)m.p, i: m.i))
            .ToList();

        return fields
            .Concat(properties)
            .OrderBy(m => m.i) // Preserve the order between properties and fields.
            .Select(m => m.m)
            .ToList();
    }
}
