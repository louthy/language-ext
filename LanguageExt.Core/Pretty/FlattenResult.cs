using System;

namespace LanguageExt.Pretty
{
    public abstract record FlattenResult<A>
    {
        public abstract FlattenResult<B> Map<B>(Func<A, B> f);
        public FlattenResult<B> Select<B>(Func<A, B> f) => Map(f);
    }

    public record Flattened<A>(A Value) : FlattenResult<A>
    {
        public override FlattenResult<B> Map<B>(Func<A, B> f) =>
            new Flattened<B>(f(Value));
    }

    public record AlreadyFlat<A> : FlattenResult<A>
    {
        public static readonly FlattenResult<A> Default = new AlreadyFlat<A>();
        
        public override FlattenResult<B> Map<B>(Func<A, B> f) =>
            AlreadyFlat<B>.Default;
    }
        
    public record NeverFlat<A> : FlattenResult<A>
    {
        public static readonly FlattenResult<A> Default = new NeverFlat<A>();
        
        public override FlattenResult<B> Map<B>(Func<A, B> f) =>
            NeverFlat<B>.Default;
    }
}
