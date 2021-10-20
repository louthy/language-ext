using System;
using System.Text;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Pretty
{
    /// <summary>
    /// Document building functions
    /// </summary>
    /// <remarks>
    /// Carries annotation, for unit only annotations, use `Doc`
    /// </remarks>
    public static class DocAnn
    {
        public static Doc<A> Annotate<A>(A annotation, Doc<A> doc) =>
            new DocAnnotate<A>(annotation, doc);
        
        public static Doc<A> Fail<A>() =>
            DocFail<A>.Default;

        public static Doc<A> Empty<A>() =>
            DocEmpty<A>.Default;
        
        public static Doc<A> Char<A>(char c) =>
            c == '\n'
                ? LineOrSpace<A>()
                : new DocChar<A>(c);

        public static Doc<A> Text<A>(string text) =>
            string.IsNullOrEmpty(text)
                ? DocEmpty<A>.Default
                : text.Length == 1
                    ? Char<A>(text[0])
                    : text.Contains("\n")
                        ? text.Split('\n')
                              .Map(Text<A>)
                              .Intersperse(DocLine<A>.Default)
                              .Reduce(Cat<A>)
                        : new DocText<A>(text);

        public static Doc<A> FlatAlt<A>(Doc<A> da, Doc<A> db) =>
            new DocFlatAlt<A>(da, db);
 
        public static Doc<A> Union<A>(Doc<A> da, Doc<A> db) =>
            new DocFlatAlt<A>(da, db);

        public static Doc<A> Cat<A>(Doc<A> da, Doc<A> db) =>
            (da, db) switch
            {
                (DocEmpty<A>, DocEmpty<A>) => da,
                (DocEmpty<A>, _)           => db,
                (_, DocEmpty<A>)           => da,
                _                          => new DocCat<A>(da, db)
            };

        public static Doc<A> Nest<A>(int indent, Doc<A> doc) =>
            indent == 0
                ? doc
                : new DocNest<A>(indent, doc);

        public static Doc<A> Column<A>(Func<int, Doc<A>> f) =>
            new DocColumn<A>(f);

        public static Doc<A> PageWidth<A>(Func<PageWidth, Doc<A>> f) =>
            new DocPageWidth<A>(f);

        public static Doc<A> Nesting<A>(Func<int, Doc<A>> f) =>
            new DocNesting<A>(f);
        
        /// <summary>
        /// A hardline is always laid out as a line break, even when 'grouped or
        /// when there is plenty of space. Note that it might still be simply discarded
        /// if it is part of a 'FlatAlt' inside a 'Group'.
        /// </summary>
        public static Doc<A> HardLine<A>() =>
            DocLine<A>.Default;

        /// <summary>
        /// LineOrSpace is a line-break, but behaves like space if the line break
        /// is undone by Group
        /// </summary>
        public static Doc<A> LineOrSpace<A>() =>
            FlatAlt(DocLine<A>.Default, Char<A>(' '));

        /// <summary>
        /// LineOrEmpty is a line-break, but behaves like Empty if the line break
        /// is undone by Group
        /// </summary>
        public static Doc<A> LineOrEmpty<A>() =>
            FlatAlt(DocLine<A>.Default, Empty<A>());

        /// <summary>
        /// softline behaves like space if the resulting output fits the page,
        /// otherwise like line
        /// </summary>
        public static Doc<A> SoftLineOrSpace<A>() =>
            Union(Char<A>(' '), DocLine<A>.Default);

        /// <summary>
        /// softline behaves like Empty if the resulting output fits the page,
        /// otherwise like line
        /// </summary>
        public static Doc<A> SoftLineOrEmpty<A>() =>
            Union(Empty<A>(), DocLine<A>.Default);

        /// <summary>
        /// Group tries laying out doc into a single line by removing the
        /// contained line breaks; if this does not fit the page, or when a 'hardline'
        /// within doc prevents it from being flattened, doc is laid out without any
        /// changes.
        /// 
        /// The 'group' function is key to layouts that adapt to available space nicely.
        /// </summary>
        public static Doc<A> Group<A>(Doc<A> doc) =>
            doc switch
            {
                DocUnion<A> _ => doc,
                DocFlatAlt<A> (var a, var b) => b.ChangesUponFlattening() switch
                                                {
                                                    Flattened<Doc<A>> (var b1) => DocAnn.Union<A>(b1, a),
                                                    AlreadyFlat<Doc<A>>        => DocAnn.Union<A>(b, a),
                                                    _                          => a
                                                },
                _ => doc.ChangesUponFlattening() switch
                     {
                         Flattened<Doc<A>> (var x1) => DocAnn.Union<A>(x1, doc),
                         _                          => doc
                     },
            };

        /// <summary>
        /// Align lays out the document with the nesting level set to the
        /// current column. It is used for example to implement 'hang'.
        ///
        /// As an example, we will put a document right above another one, regardless of
        /// the current nesting level. Without alignment, the second line is put simply
        /// below everything we've had so far,
        ///
        ///     Text("lorem") + VertSep(["ipsum", "dolor"])
        ///
        /// lorem ipsum
        /// dolor
        ///
        /// If we add an 'Align' to the mix, the VertSep's contents all start in the
        /// same column,
        ///
        /// >>> Text("lorem") + Align (VertSep(["ipsum", "dolor"]))
        /// lorem ipsum
        ///       dolor        
        /// </summary>
        public static Doc<A> Align<A>(Doc<A> doc) =>
            Column(k => Nesting(i => Nest(k - i, doc)));

        /// <summary>     
        /// Hang lays out the document with a nesting level set to the
        /// /current column/ plus offset. Negative values are allowed, and decrease the
        /// nesting level accordingly.
        ///
        /// >>> var doc = Reflow("Indenting these words with hang")
        /// >>> PutDocW(24, ("prefix" + Hang(4, doc)))
        /// prefix Indenting these
        ///            words with
        ///            hang
        ///
        /// This differs from Nest, which is based on the /current nesting level/ plus
        /// offset. When you're not sure, try the more efficient 'nest' first. In our
        /// example, this would yield
        ///
        /// >>> var doc = Reflow("Indenting these words with nest")
        /// >>> PutDocW(24, "prefix" + Nest(4, doc))
        /// prefix Indenting these
        ///     words with nest
        /// </summary>
        public static Doc<A> Hang<A>(int offset, Doc<A> doc) =>
            Align(Nest(offset, doc));

        /// <summary>
        /// Indents document `indent` columns, starting from the
        /// current cursor position.
        ///
        /// >>> var doc = Reflow("The indent function indents these words!")
        /// >>> PutDocW(24, ("prefix" + Indent(4, doc))
        /// prefix    The indent
        ///           function
        ///           indents these
        ///           words!
        ///
        /// </summary>
        public static Doc<A> Indent<A>(int indent, Doc<A> doc) =>
            Hang(indent, Spaces<A>(indent) | doc);

        /// <summary>
        /// Intersperse the documents with a separator
        /// </summary>
        public static Doc<A> Sep<A>(Doc<A> sep, Seq<Doc<A>> docs)
        {
            if (docs.IsEmpty) return DocAnn.Empty<A>();
            var d = docs.Head;
            foreach (var doc in docs.Tail)
            {
                d = d | sep | doc;
            }
            return d;
        }

        /// <summary>
        /// Delimit and intersperse the documents with a separator
        /// </summary>
        public static Doc<A> BetweenSep<A>(Doc<A> leftDelim, Doc<A> rightDelim, Doc<A> sep, Seq<Doc<A>> docs) =>
            leftDelim | Sep(sep, docs) | rightDelim;

        /// <summary>
        /// Delimit and intersperse the documents with a separator
        /// </summary>
        public static Doc<A> BetweenSep<A>(Doc<A> leftDelim, Doc<A> rightDelim, Doc<A> sep, params Doc<A>[] docs) =>
            BetweenSep(leftDelim, rightDelim, sep, toSeq(docs));

        /// <summary>
        /// Haskell-inspired array/list formatting
        /// </summary>
        public static Doc<A> List<A>(Seq<Doc<A>> docs) =>
            Group(BetweenSep<A>(
                      FlatAlt<A>("[ ", "["),
                      FlatAlt<A>(" ]", "]"),
                      ", ",
                      docs));

        /// <summary>
        /// Haskell-inspired tuple formatting
        /// </summary>
        public static Doc<A> Tuple<A>(Seq<Doc<A>> docs) =>
            Group(BetweenSep<A>(
                      FlatAlt<A>("( ", "("),
                      FlatAlt<A>(" )", ")"),
                      ", ",
                      docs));
        

        /// <summary>
        /// Haskell-inspired array/list formatting
        /// </summary>
        public static Doc<A> List<A>(params Doc<A>[] docs) =>
            Group(BetweenSep<A>(
                      FlatAlt<A>("[ ", "["),
                      FlatAlt<A>(" ]", "]"),
                      ", ",
                      docs));

        /// <summary>
        /// Haskell-inspired tuple formatting
        /// </summary>
        public static Doc<A> Tuple<A>(params Doc<A>[] docs) =>
            Group(BetweenSep<A>(
                      FlatAlt<A>("( ", "("),
                      FlatAlt<A>(" )", ")"),
                      ", ",
                      docs));        

        /// <summary>
        /// Insert a number of spaces. Negative values count as 0.
        /// </summary>
        public static Doc<A> Spaces<A>(int n)
        {
            return (n < 0 ? 0 : n) switch
                   {
                       0 => Empty<A>(),
                       1 => Char<A>(' '),
                       2 => Text<A>("  "),
                       3 => Text<A>("   "),
                       4 => Text<A>("    "),
                       5 => Text<A>("     "),
                       6 => Text<A>("      "),
                       7 => Text<A>("       "),
                       8 => Text<A>("        "),
                       _ => Text<A>(FastSpace(n - 8))
                   };
            
            string FastSpace(int x)
            {
                var sb = new StringBuilder();
                sb.Append("        ");
                for (var i = 0; i < x; i++)
                {
                    sb.Append(' ');
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// HorizSep concatenates all documents horizontally with `+`
        /// i.e. it puts a space between all entries.
        ///
        /// HorizSep does not introduce line breaks on its own, even when the page is too
        /// narrow:
        ///
        /// For automatic line breaks, consider using 'fillSep' instead.
        /// </summary>
        public static Doc<A> HorizSep<A>(Seq<Doc<A>> docs) =>
            docs.IsEmpty
                ? DocEmpty<A>.Default
                : docs.Tail.Fold(docs.Head, (s, d) => s + d);

        /// <summary>
        /// VertSep concatenates all documents above each other. If a
        /// Group undoes the line breaks inserted by VertSep, the documents are
        /// separated with a 'space' instead.
        ///
        /// Grouping a VertSep separates the documents with a 'space' if it fits the
        /// page (and does nothing otherwise). See the Sep convenience function for
        /// this use case.
        ///
        /// The Align function can be used to align the documents under their first
        /// element:
        ///
        /// Since Grouping a VertSep is rather common, Sep is a built-in for doing
        /// that.        
        /// </summary>
        public static Doc<A> VertSep<A>(Seq<Doc<A>> docs) =>
            docs.IsEmpty
                ? DocEmpty<A>.Default
                : docs.Tail.Fold(docs.Head, (s, d) => s | LineOrSpace<A>() | d);

        /// <summary>
        /// Sep tries laying out the documents separated with 'space's,
        /// and if this does not fit the page, separates them with newlines. This is what
        /// differentiates it from VerSep, which always lays out its contents beneath
        /// each other.
        /// </summary>
        public static Doc<A> Sep<A>(Seq<Doc<A>> docs) =>
            Group(VertSep(docs));

        /// <summary>
        /// FillSep concatenates the documents horizontally with `+` (inserting a space)
        /// as long as it fits the page, then inserts a Line and continues doing that
        /// for all documents in (Line means that if Grouped, the documents
        /// are separated with a Space instead of newlines.  Use 'FillCat' if you do not
        /// want a 'space'.)       
        /// </summary>
        public static Doc<A> FillSep<A>(Seq<Doc<A>> docs) =>
            docs.IsEmpty
                ? DocEmpty<A>.Default
                : docs.Tail.Fold(docs.Head, (s, d) => s | SoftLineOrSpace<A>() | d);

        /// <summary>
        /// Hard line separator
        /// </summary>
        public static Doc<A> HardSep<A>(Seq<Doc<A>> docs) =>
            docs.IsEmpty
                ? DocEmpty<A>.Default
                : docs.Tail.Fold(docs.Head, (s, d) => (s, d) switch
                                                      {
                                                          (DocLine<A>, DocLine<A>) => s,
                                                          (DocLine<A>, _)          => d,
                                                          (_, DocLine<A>)          => s,
                                                          _                        => s | HardLine<A>() | d
                                                      });
         
        /// <summary>
        /// HorizCat concatenates all documents horizontally with |
        /// (i.e. without any spacing).
        /// 
        /// It is provided only for consistency, since it is identical to 'Cat'.
        /// </summary>
        public static Doc<A> HorizCat<A>(Seq<Doc<A>> docs) =>
            docs.IsEmpty
                ? DocEmpty<A>.Default
                : docs.Tail.Fold(docs.Head, (s, d) => s | d);

        /// <summary>
        /// VertCat vertically concatenates the documents. If it is
        /// Grouped, the line breaks are removed.
        ///
        /// In other words VertCat is like VertSep, with newlines removed instead of
        /// replaced by spaces.
        /// </summary>
        public static Doc<A> VertCat<A>(Seq<Doc<A>> docs) =>
            docs.IsEmpty
                ? DocEmpty<A>.Default
                : docs.Tail.Fold(docs.Head, (s, d) => s | LineOrEmpty<A>() | d);

        /// <summary>
        /// Width lays out the document 'doc', and makes the column width
        /// of it available to a function.
        /// </summary>
        public static Doc<A> Width<A>(Doc<A> doc, Func<int, Doc<A>> f) =>
            Column<A>(colStart =>
                          doc | Column<A>(colEnd =>
                                              f(colEnd - colStart)));

        /// <summary>
        /// Fill lays out the document. It then appends spaces until
        /// the width is equal to `width`. If the width is already larger, nothing is
        /// appended.
        /// </summary>
        public static Doc<A> Fill<A>(int width, Doc<A> doc) =>
            Width<A>(doc, w => Spaces<A>(width - w));

        /// <summary>
        /// Enclose
        /// </summary>
        public static Doc<A> Between<A>(Doc<A> left, Doc<A> middle, Doc<A> right) =>
            left | middle | right;
    }
}
