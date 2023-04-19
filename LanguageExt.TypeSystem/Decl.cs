using System;
using System.Collections.Generic;

namespace LanguageExt.TypeSystem;

/// <summary>
/// Type declaration 
/// </summary>
public abstract record Decl<Ann>
{
    public static Decl<Ann> Nil => 
        NilDecl<Ann>.Default; 
    
    public static Decl<Ann> Record(IEnumerable<Decl<Ann>> fields, IEnumerable<Ty<Ann>> @params, IEnumerable<Deriving> derivings) =>
        new RecordDecl<Ann>(fields, @params, derivings);
    
    public static Decl<Ann> Union(IEnumerable<Decl<Ann>> cases, IEnumerable<Ty<Ann>> @params, IEnumerable<Deriving> derivings) =>
        new UnionDecl<Ann>(cases, @params, derivings);
    
    public static Decl<Ann> Label(string name, Ty<Ann> type) =>
        new LabelDecl<Ann>(name, type);
    
    public static Decl<Ann> Annotate(Ann annotation, Decl<Ann> decl) =>
        new AnnotateDecl<Ann>(annotation, decl);
    
    public static Func<Decl<Ann>, Decl<Ann>> Annotate(Ann annotation) =>
        decl => Annotate(annotation, decl);
    
}

public record NilDecl<Ann> : Decl<Ann>
{
    public static readonly Decl<Ann> Default = new NilDecl<Ann>();
}

public record RecordDecl<Ann>(IEnumerable<Decl<Ann>> Fields, IEnumerable<Ty<Ann>> Params, IEnumerable<Deriving> Derivings) : Decl<Ann>; 
public record UnionDecl<Ann>(IEnumerable<Decl<Ann>> Cases, IEnumerable<Ty<Ann>> Params, IEnumerable<Deriving> Derivings) : Decl<Ann>;
public record LabelDecl<Ann>(string Name, Ty<Ann> Type) : Decl<Ann>;
public record AnnotateDecl<Ann>(Ann Annotation, Decl<Ann> Decl) : Decl<Ann>;
