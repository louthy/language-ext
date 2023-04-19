namespace LanguageExt.TypeSystem;

/// <summary>
/// Type 
/// </summary>
public abstract record Ty<Ann>
{
    public static Ty<Ann> Var(string name) =>
        new TyVar<Ann>(name);
    
    public static Ty<Ann> Id(string name) =>
        new TyId<Ann>(name);
    
    public static Ty<Ann> App(Ty<Ann> f, Ty<Ann> x) =>
        new TyApp<Ann>(f, x);
    
    public static Ty<Ann> Arr(Ty<Ann> f, Ty<Ann> x) =>
        new TyArr<Ann>(f, x);
    
    public static Ty<Ann> Abs(string name, Ty<Ann> body) =>
        new TyAbs<Ann>(name, body);
    
    public static Ty<Ann> Nullable(Ty<Ann> type) =>
        new TyNullable<Ann>(type);
    
    public static Ty<Ann> Array(Ty<Ann> type) =>
        new TyArray<Ann>(type);
    
    public static Ty<Ann> Annotate(Ann annotation, Ty<Ann> type) =>
        new TyAnnotate<Ann>(annotation, type);
}

public record TyVar<Ann>(string Name) : Ty<Ann>;
public record TyId<Ann>(string Name) : Ty<Ann>;
public record TyApp<Ann>(Ty<Ann> F, Ty<Ann> X) : Ty<Ann>;
public record TyArr<Ann>(Ty<Ann> F, Ty<Ann> X) : Ty<Ann>;
public record TyAbs<Ann>(string Name, Ty<Ann> Body) : Ty<Ann>;
public record TyNullable<Ann>(Ty<Ann> Type) : Ty<Ann>;
public record TyArray<Ann>(Ty<Ann> Type) : Ty<Ann>;
public record TyAnnotate<Ann>(Ann Annotation, Ty<Ann> Type) : Ty<Ann>;
