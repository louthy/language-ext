using LanguageExt.TypeSystem;
using Microsoft.CodeAnalysis;

namespace LanguageExt.SourceGen;

/// <summary>
/// Generator environment
/// </summary>
public record GenEnv<Ann>(GeneratorExecutionContext ExecContext, Stack<Ann> Annotations);

/// <summary>
/// Generator state
/// </summary>
public record GenState<Ann, A>(GenEnv<Ann> Env, A Value)
{
    public GenState<Ann, B> Select<B>(Func<A, B> f) =>
        Map(f);
    
    public GenState<Ann, B> Map<B>(Func<A, B> f) =>
        new (Env, f(Value));
}

public record Gen<Ann, A>(Func<GenEnv<Ann>, GenState<Ann, A>> F)
{
    public Gen<Ann, B> Select<B>(Func<A, B> f) =>
        Map(f);
    
    public Gen<Ann, B> Map<B>(Func<A, B> f) =>
        new(env => Run(env).Map(f));
    
    public Gen<Ann, B> SelectMany<B>(Func<A, Gen<Ann, B>> f) =>
        Bind(f);

    public Gen<Ann, C> SelectMany<B, C>(Func<A, Gen<Ann, B>> bind, Func<A, B, C> proj) =>
        new(env =>
        {
            var ra = Run(env);
            var mb = bind(ra.Value);
            var rb = mb.Run(ra.Env);
            return new (rb.Env, proj(ra.Value, rb.Value));
        });

    public Gen<Ann, B> Bind<B>(Func<A, Gen<Ann, B>> f) =>
        new(env =>
        {
            var ra = Run(env);
            var mb = f(ra.Value);
            return mb.Run(ra.Env);
        });

    public GenState<Ann, A> Run(GenEnv<Ann> env) =>
        F(env);
}



public static class Gen
{
    public static Gen<Ann, A> Pure<Ann, A>(A value) =>
        new(env => new GenState<Ann, A>(env, value));
    
    public static Gen<SyntaxNode, A> Pure<A>(A value) =>
        new(env => new GenState<SyntaxNode, A>(env, value));
    
    public static Gen<Ann, A> lift<Ann, A>(Func<A> f) =>
        new(env => new GenState<Ann, A>(env, f()));
    
    public static Gen<SyntaxNode, A> lift<A>(Func<A> f) =>
        new(env => new GenState<SyntaxNode, A>(env, f()));
    
    public static Gen<Ann, Unit> liftVoid<Ann>(Action f) =>
        new(env =>
        {
            f();
            return new GenState<Ann, Unit>(env, default);
        });
    
    public static Gen<SyntaxNode, Unit> liftVoid(Action f) =>
        new(env =>
        {
            f();
            return new GenState<SyntaxNode, Unit>(env, default);
        });
    
    public static Gen<Ann, IEnumerable<B>> Sequence<Ann, A, B>(this IEnumerable<A> ma, Func<A, Gen<Ann, B>> f) =>
        new(env =>
        {
            var xs = Go().ToArray();
            return new GenState<Ann, IEnumerable<B>>(env, xs);

            IEnumerable<B> Go()
            {
                foreach (var a in ma)
                {
                    var r = f(a).F(env);
                    env = r.Env;
                    yield return r.Value;
                }
            }
        });

    public static Gen<Ann, IEnumerable<A>> Sequence<Ann, A>(this IEnumerable<Gen<Ann, A>> ma) =>
        ma.Sequence(static x => x);

    public static Gen<SyntaxNode, Unit> walk(IEnumerable<Decl<SyntaxNode>> decls) =>
        decls.Sequence(walk).Map(_ => default(Unit));

    public static Gen<SyntaxNode, Unit> walk(Decl<SyntaxNode> decl) =>
        decl switch
        {
            AnnotateDecl<SyntaxNode> d => annotate(d.Annotation, walk(d.Decl)),
            RecordDecl<SyntaxNode>   d => record(d),
            _                          => throw new NotSupportedException()
        };

    public static Gen<SyntaxNode, SyntaxNode> annotation =>
        new(env => new GenState<SyntaxNode, SyntaxNode>(env, env.Annotations.Peek()));
        
    public static Gen<SyntaxNode, Stack<SyntaxNode>> annotations =>
        new(env => new GenState<SyntaxNode, Stack<SyntaxNode>>(env, env.Annotations));
        
    public static Gen<SyntaxNode, GeneratorExecutionContext> execContext =>
        new(env => new GenState<SyntaxNode, GeneratorExecutionContext>(env, env.ExecContext));
    
    public static Gen<SyntaxNode, A> annotate<A>(SyntaxNode annotation, Gen<SyntaxNode, A> ma) =>
        new(env =>
        {
            var state = ma.Run(env with {Annotations = env.Annotations.Push(annotation)});
            return state with {Env = state.Env with {Annotations = env.Annotations}};
        });

    public static Gen<SyntaxNode, Unit> record(Decl<SyntaxNode> decl) =>
        from n in makeName(decl)
        from r in makeRecord(n, decl)
        from _ in addSource(n, r)
        select default(Unit);

    public static Gen<SyntaxNode, FullName> makeName(Decl<SyntaxNode> decl) =>
        from a in annotation
        select Member.FullName(a);

    public static Gen<SyntaxNode, string> makeRecord(FullName name, Decl<SyntaxNode> decl) =>
        Pure(name.AsString());

    public static Gen<SyntaxNode, Unit> addSource(FullName hintName, string sourceText) =>
        from c in execContext
        from _ in liftVoid(() => c.AddSource($"{hintName.AsString()}.generated.cs", sourceText))
        select default(Unit);
}
