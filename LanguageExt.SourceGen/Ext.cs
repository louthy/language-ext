using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LanguageExt.SourceGen;

public static class LinqExt
{
    public static IEnumerable<B> Cast<A, B>(this IEnumerable<A> ma) where B : A =>
        ma.Where(a => a is B)
          .Select(a => (B?)a ?? throw new InvalidCastException());

    public static IEnumerable<A> CastMember<A>(this IEnumerable<MemberDeclarationSyntax> ma)
        where A : MemberDeclarationSyntax =>
        ma.Where(a => a is A).Select(a => (A?)a ?? throw new InvalidCastException());

    public static string AsString(this FullName fn) =>
        string.Join(".", fn);
}
