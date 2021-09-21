namespace LanguageExt.Pretty
{
    /// <summary>
    /// List of nesting level/document pairs yet to be laid out.
    /// </summary>
    public abstract record LayoutPipeline<A>;

    public record Nil<A> : LayoutPipeline<A>
    {
        public static readonly LayoutPipeline<A> Default = new Nil<A>();
    }

    public record Cons<A>(int Value, Doc<A> Doc, LayoutPipeline<A> Pipeline): LayoutPipeline<A>;
    public record UndoAnn<A>(LayoutPipeline<A> Pipeline): LayoutPipeline<A>;
}
