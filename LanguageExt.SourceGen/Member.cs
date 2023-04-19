using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.SourceGen;

public static class Member
{
    public static string Name(SyntaxNode m) =>
        m switch
        {
            FieldDeclarationSyntax f => f.Declaration.Variables[0].Identifier.Text,
            PropertyDeclarationSyntax p => p.Identifier.Text,
            MethodDeclarationSyntax d => d.Identifier.Text,
            ClassDeclarationSyntax c => c.Identifier.Text,
            StructDeclarationSyntax s => s.Identifier.Text,
            InterfaceDeclarationSyntax i => i.Identifier.Text,
            RecordDeclarationSyntax r => r.Identifier.Text,
            EnumDeclarationSyntax e => e.Identifier.Text,
            NamespaceDeclarationSyntax n => n.Name.ToString(),
            _ => throw new NotSupportedException()
        };
    
    public static TypeSyntax Type(SyntaxNode m) =>
        m switch
        {
            FieldDeclarationSyntax f => f.Declaration.Type,
            PropertyDeclarationSyntax p => p.Type,
            MethodDeclarationSyntax d => d.ReturnType,
            _ => throw new NotSupportedException()
        };

    public static List<string> FullName(SyntaxNode m) =>
        FullName(m.Parent, Name(m).Cons(List.Nil<string>()));
    
    static List<string> FullName(SyntaxNode? m, List<string> ns) =>
        m switch
        {
            null => ns, 
            FieldDeclarationSyntax f => FullName(m.Parent, f.Declaration.Variables[0].Identifier.Text.Cons(ns)),
            PropertyDeclarationSyntax p => FullName(m.Parent, p.Identifier.Text.Cons(ns)),
            MethodDeclarationSyntax d => FullName(m.Parent, d.Identifier.Text.Cons(ns)),
            ClassDeclarationSyntax c => FullName(m.Parent, c.Identifier.Text.Cons(ns)),
            StructDeclarationSyntax s => FullName(m.Parent, s.Identifier.Text.Cons(ns)),
            InterfaceDeclarationSyntax i => FullName(m.Parent, i.Identifier.Text.Cons(ns)),
            RecordDeclarationSyntax r => FullName(m.Parent, r.Identifier.Text.Cons(ns)),
            EnumDeclarationSyntax e => FullName(m.Parent, e.Identifier.Text.Cons(ns)),
            NamespaceDeclarationSyntax n => FullName(m.Parent, FullName(n.Name, ns)),
            _ => ns
        };        
    
    static List<string> FullName(NameSyntax name, List<string> ns) =>
        name switch
        {
            QualifiedNameSyntax qn => FullName(qn.Left, FullName(qn.Right, ns)),
            SimpleNameSyntax sn => sn.ToString().Cons(ns),
            _ => throw new NotSupportedException()
        };
}
