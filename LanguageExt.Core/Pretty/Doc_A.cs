using System;
using System.Text;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Pretty
{
    /// <summary>
    /// Base document record
    /// </summary>
    /// <remarks>
    /// Construct documents using the Doc and DocAnn static types
    /// </remarks>
    public abstract record Doc<A>
    {
        public virtual Doc<A> Append(Doc<A> d) =>
            DocAnn.Cat<A>(this, d);

        public readonly Doc<A> Empty =
            DocEmpty<A>.Default;

        public static Doc<A> operator +(Doc<A> x, Doc<A> y) =>
            x | DocAnn.Char<A>(' ') | y;

        public static Doc<A> operator +(string x, Doc<A> y) =>
            x | DocAnn.Char<A>(' ') | y;

        public static Doc<A> operator +(Doc<A> x, string y) =>
            x | DocAnn.Char<A>(' ') | y;

        public static Doc<A> operator |(Doc<A> x, Doc<A> y) =>
            x.Append(y);

        public static Doc<A> operator |(string x, Doc<A> y) =>
            DocAnn.Text<A>(x).Append(y);

        public static Doc<A> operator |(Doc<A> x, string y) =>
            x.Append(DocAnn.Text<A>(y));

        public Doc<B> Select<B>(Func<A, B> f) =>
            ReAnnotate(f);

        public Doc<B> Map<B>(Func<A, B> f) =>
            ReAnnotate(f);

        public abstract Doc<B> ReAnnotate<B>(Func<A, B> f);
        public abstract FlattenResult<Doc<A>> ChangesUponFlattening();
        public abstract Doc<A> Flatten();

        public static implicit operator Doc<A>(string x) =>
            DocAnn.Text<A>(x);

        public string Show() =>
            Layout.smart(LayoutOptions.Default, this).Show();
    }

    public record DocFail<A> : Doc<A>
    {
        public readonly static Doc<A> Default = new DocFail<A>();

        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocFail<B>.Default;

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            NeverFlat<Doc<A>>.Default;

        public override Doc<A> Flatten() =>
            this;
    }
        
    public record DocEmpty<A> : Doc<A>
    {
        public readonly static Doc<A> Default = new DocEmpty<A>();

        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocEmpty<B>.Default;

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            AlreadyFlat<Doc<A>>.Default;

        public override Doc<A> Flatten() =>
            this;

        public override Doc<A> Append(Doc<A> d) =>
            d switch
            {
                DocText<A>(var ts) => d,
                DocChar<A>(var c)  => d,
                DocEmpty<A>        => this,
                _                  => base.Append(d)
            };
    }

    /// <summary>
    /// Invariant: not '\n'
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public record DocChar<A>(char Value) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Char<B>(Value);

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            AlreadyFlat<Doc<A>>.Default;

        public override Doc<A> Flatten() =>
            this;
    }

    /// <summary>
    /// At least two characters long, does not contain '\n'. For
    /// empty documents, there is `DocEmpty`; for singleton documents, there is
    /// `DocChar`; newlines should be replaced by e.g. `DocLine`.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public record DocText<A>(string Text) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Text<B>(Text);

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            AlreadyFlat<Doc<A>>.Default;

        public override Doc<A> Flatten() =>
            this;

        public int Length =>
            Text.Length;
    }

    /// <summary>
    /// Hard line break
    /// </summary>
    public record DocLine<A> : Doc<A>
    {
        public readonly static Doc<A> Default = new DocLine<A>();

        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocLine<B>.Default;

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            NeverFlat<Doc<A>>.Default;

        public override Doc<A> Flatten() =>
            DocFail<A>.Default;
    }

    /// <summary>
    /// Lay out the first 'Doc', but when flattened (via 'group'), prefer
    /// the second.
    /// 
    /// The layout algorithms work under the assumption that the first
    /// alternative is less wide than the flattened second alternative.
    /// </summary>
    public record DocFlatAlt<A>(Doc<A> Primary, Doc<A> Alt) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.FlatAlt<B>(Primary.ReAnnotate(f), Alt.ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            new Flattened<Doc<A>>(Alt.Flatten());

        public override Doc<A> Flatten() =>
            Alt.Flatten();
    }

    /// <summary>
    /// The first lines of first document should be longer than the
    /// first lines of the second one, so the layout algorithm can pick the one
    /// that fits best. Used to implement layout alternatives for 'group'.
    /// </summary>
    public record DocUnion<A>(Doc<A> DocA, Doc<A> DocB) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Union<B>(DocA.ReAnnotate(f), DocB.ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            new Flattened<Doc<A>>(DocA.Flatten());

        public override Doc<A> Flatten() =>
            DocA.Flatten();
    }

    /// <summary>
    /// Concat two documents
    /// </summary>
    public record DocCat<A>(Doc<A> DocA, Doc<A> DocB) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Cat(DocA.ReAnnotate(f), DocB.ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            (DocA.ChangesUponFlattening(), DocB.ChangesUponFlattening()) switch
            {
                (NeverFlat<Doc<A>>, _)                                   => NeverFlat<Doc<A>>.Default,
                (_, NeverFlat<Doc<A>>)                                   => NeverFlat<Doc<A>>.Default,
                (Flattened<Doc<A>> (var x1), Flattened<Doc<A>> (var y1)) => new Flattened<Doc<A>>(DocAnn.Cat<A>(x1, y1)),
                (Flattened<Doc<A>> (var x1), AlreadyFlat<Doc<A>>)        => new Flattened<Doc<A>>(DocAnn.Cat<A>(x1, DocB)),
                (AlreadyFlat<Doc<A>>, Flattened<Doc<A>> (var y1))        => new Flattened<Doc<A>>(DocAnn.Cat<A>(DocA, y1)),
                _                                                        => AlreadyFlat<Doc<A>>.Default
            };

        public override Doc<A> Flatten() =>
            DocAnn.Cat(DocA.Flatten(), DocB.Flatten());
    }
    
    /// <summary>
    /// Document indented by a number of columns
    /// </summary>
    public record DocNest<A>(int Indent, Doc<A> Doc) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Nest<B>(Indent, Doc.ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            Doc.ChangesUponFlattening().Map(d => DocAnn.Nest<A>(Indent, d) as Doc<A>);

        public override Doc<A> Flatten() =>
            DocAnn.Nest<A>(Indent, Doc.Flatten());

        public override string ToString() =>
            $"(nest {Indent} {Doc})";
    }
    
    /// <summary>
    /// React on the current cursor position,
    /// </summary>
    public record DocColumn<A>(Func<int, Doc<A>> React) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Column<B>(a => React(a).ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            new Flattened<Doc<A>>(DocAnn.Column<A>(x => React(x).Flatten()));

        public override Doc<A> Flatten() =>
            DocAnn.Column<A>(x => React(x).Flatten());
    }
    
    /// <summary>
    /// React on the document's page width
    /// </summary>
    public record DocPageWidth<A>(Func<PageWidth, Doc<A>> React) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.PageWidth<B>(a => React(a).ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            new Flattened<Doc<A>>(DocAnn.PageWidth<A>(x => React(x).Flatten()));

        public override Doc<A> Flatten() =>
            DocAnn.PageWidth<A>(x => React(x).Flatten());
    }
    
    /// <summary>
    /// React on the current nesting level
    /// </summary>
    public record DocNesting<A>(Func<int, Doc<A>> React) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Nesting<B>(a => React(a).ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            new Flattened<Doc<A>>(DocAnn.Nesting<A>(x => React(x).Flatten()));

        public override Doc<A> Flatten() =>
            DocAnn.Nesting<A>(x => React(x).Flatten());
    }
    
    /// <summary>
    /// dd an annotation to the enclosed 'Doc'. Can be used for example to add
    /// styling directives or alt texts that can then be used by the renderer.
    /// </summary>
    public record DocAnnotate<A>(A Annotation, Doc<A> Doc) : Doc<A>
    {
        public override Doc<B> ReAnnotate<B>(Func<A, B> f) =>
            DocAnn.Annotate<B>(f(Annotation), Doc.ReAnnotate(f));

        public override FlattenResult<Doc<A>> ChangesUponFlattening() =>
            Doc.ChangesUponFlattening().Map(d => DocAnn.Annotate<A>(Annotation, Doc) as Doc<A>);

        public override Doc<A> Flatten() =>
            DocAnn.Annotate<A>(Annotation, Doc.Flatten());
    }
}
