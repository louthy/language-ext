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
    /// No annotation (unit only), use `DocAnn` for annotated documents
    /// </remarks>
    public static class Doc
    {
        public static readonly Doc<Unit> Fail =
            DocAnn.Fail<Unit>();
        
        public static readonly Doc<Unit> Empty =
            DocAnn.Empty<Unit>();
        
        public static Doc<Unit> Char(char c) =>
            DocAnn.Char<Unit>(c);

        public static Doc<Unit> Text(string text) =>
            DocAnn.Text<Unit>(text);

        public static Doc<Unit> FlatAlt(Doc<Unit> da, Doc<Unit> db) =>
            DocAnn.FlatAlt(da, db);
 
        public static Doc<Unit> Union(Doc<Unit> da, Doc<Unit> db) =>
            DocAnn.Union(da, db);
        
        public static Doc<Unit> Cat(Doc<Unit> da, Doc<Unit> db) =>
            DocAnn.Cat(da, db);

        public static Doc<Unit> Nest(int indent, Doc<Unit> doc) =>
            DocAnn.Nest(indent, doc);

        public static Doc<Unit> Column(Func<int, Doc<Unit>> f) =>
            DocAnn.Column(f);

        public static Doc<Unit> PageWidth(Func<PageWidth, Doc<Unit>> f) =>
            DocAnn.PageWidth(f);

        public static Doc<Unit> Nesting(Func<int, Doc<Unit>> f) =>
            DocAnn.Nesting(f);
        
        /// <summary>
        /// A hardline is always laid out as a line break, even when 'grouped or
        /// when there is plenty of space. Note that it might still be simply discarded
        /// if it is part of a 'FlatAlt' inside a 'Group'.
        /// </summary>
        public static readonly Doc<Unit> HardLine =
            DocLine<Unit>.Default;

        /// <summary>
        /// LineOrSpace is a line-break, but behaves like Char(' ') if the line break
        /// is undone by Group
        /// </summary>
        public static readonly Doc<Unit> LineOrSpace =
            DocAnn.LineOrSpace<Unit>();

        /// <summary>
        /// LineOrEmpty is a line-break, but behaves like Empty if the line break
        /// is undone by Group
        /// </summary>
        public static readonly Doc<Unit> LineOrEmpty =
            DocAnn.LineOrEmpty<Unit>();

        /// <summary>
        /// softline behaves like space if the resulting output fits the page,
        /// otherwise like line
        /// </summary>
        public static readonly Doc<Unit> SoftLineOrSpace =
            DocAnn.SoftLineOrSpace<Unit>();

        /// <summary>
        /// softline behaves like Empty if the resulting output fits the page,
        /// otherwise like line
        /// </summary>
        public static readonly Doc<Unit> SoftLineOrEmpty =
            DocAnn.SoftLineOrEmpty<Unit>();

        /// <summary>
        /// Group tries laying out doc into a single line by removing the
        /// contained line breaks; if this does not fit the page, or when a 'hardline'
        /// within doc prevents it from being flattened, doc is laid out without any
        /// changes.
        /// 
        /// The 'group' function is key to layouts that adapt to available space nicely.
        /// </summary>
        public static Doc<Unit> Group(Doc<Unit> doc) =>
            DocAnn.Group(doc);

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
        /// If we add an Align to the mix, the VertSep contents all start in the
        /// same column,
        ///
        /// >>> Text("lorem") + Align (VertSep(["ipsum", "dolor"]))
        /// lorem ipsum
        ///       dolor        
        /// </summary>
        public static Doc<Unit> Align(Doc<Unit> doc) =>
            DocAnn.Align(doc);

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
        public static Doc<Unit> Hang(int offset, Doc<Unit> doc) =>
            DocAnn.Hang(offset, doc);

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
        public static Doc<Unit> Indent(int indent, Doc<Unit> doc) =>
            DocAnn.Indent(indent, doc);

        /// <summary>
        /// Delimit and intersperse the documents with a separator
        /// </summary>
        public static Doc<Unit> EncloseSep(Doc<Unit> leftDelim, Doc<Unit> rightDelim, Doc<Unit> sep, Seq<Doc<Unit>> docs) =>
            DocAnn.BetweenSep(leftDelim, rightDelim, sep, docs);

        /// <summary>
        /// Delimit and intersperse the documents with a separator
        /// </summary>
        public static Doc<Unit> EncloseSep(Doc<Unit> leftDelim, Doc<Unit> rightDelim, Doc<Unit> sep, params Doc<Unit>[] docs) =>
            DocAnn.BetweenSep(leftDelim, rightDelim, sep, docs);

        /// <summary>
        /// Haskell-inspired array/list formatting
        /// </summary>
        public static Doc<Unit> List(Seq<Doc<Unit>> docs) =>
            DocAnn.List(docs);

        /// <summary>
        /// Haskell-inspired tuple formatting
        /// </summary>
        public static Doc<Unit> Tuple(Seq<Doc<Unit>> docs) =>
            DocAnn.Tuple(docs);

        /// <summary>
        /// Haskell-inspired array/list formatting
        /// </summary>
        public static Doc<Unit> List(params Doc<Unit>[] docs) =>
            DocAnn.List(docs);

        /// <summary>
        /// Haskell-inspired tuple formatting
        /// </summary>
        public static Doc<Unit> Tuple(params Doc<Unit>[] docs) =>
            DocAnn.Tuple(docs);
        
        /// <summary>
        /// Insert a number of spaces. Negative values count as 0.
        /// </summary>
        public static Doc<Unit> Spaces(int n) =>
            DocAnn.Spaces<Unit>(n);
        
        /// <summary>
        /// HorizSep concatenates all documents horizontally with `+`
        /// i.e. it puts a space between all entries.
        ///
        /// HorizSep does not introduce line breaks on its own, even when the page is too
        /// narrow:
        ///
        /// For automatic line breaks, consider using 'fillSep' instead.
        /// </summary>
        public static Doc<Unit> HorizSep(Seq<Doc<Unit>> docs) =>
            DocAnn.HorizSep(docs);

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
        public static Doc<Unit> VertSep(Seq<Doc<Unit>> docs) =>
            DocAnn.VertSep(docs);

        /// <summary>
        /// Intersperse the documents with a separator
        /// </summary>
        public static Doc<Unit> Sep(Doc<Unit> sep, Seq<Doc<Unit>> docs) =>
            DocAnn.Sep(sep, docs);
 
        /// <summary>
        /// Sep tries laying out the documents separated with 'space's,
        /// and if this does not fit the page, separates them with newlines. This is what
        /// differentiates it from VerSep, which always lays out its contents beneath
        /// each other.
        /// </summary>
        public static Doc<Unit> Sep(Seq<Doc<Unit>> docs) =>
            DocAnn.Sep(docs);

        /// <summary>
        /// FillSep concatenates the documents horizontally with `+` (inserting a space)
        /// as long as it fits the page, then inserts a Line and continues doing that
        /// for all documents in (Line means that if Grouped, the documents
        /// are separated with a Space instead of newlines.  Use 'FillCat' if you do not
        /// want a 'space'.)       
        /// </summary>
        public static Doc<Unit> FillSep(Seq<Doc<Unit>> docs) =>
            DocAnn.FillSep(docs);

        /// <summary>
        /// Hard line separator
        /// </summary>
        public static Doc<Unit> HardSep(Seq<Doc<Unit>> docs) =>
            DocAnn.HardSep(docs);

        /// <summary>
        /// HorizCat concatenates all documents horizontally with |
        /// (i.e. without any spacing).
        /// 
        /// It is provided only for consistency, since it is identical to 'Cat'.
        /// </summary>
        public static Doc<Unit> HorizCat(Seq<Doc<Unit>> docs) =>
            DocAnn.HorizSep(docs);

        /// <summary>
        /// VertCat vertically concatenates the documents. If it is
        /// Grouped, the line breaks are removed.
        ///
        /// In other words VertCat is like VertSep, with newlines removed instead of
        /// replaced by spaces.
        /// </summary>
        public static Doc<Unit> VertCat(Seq<Doc<Unit>> docs) =>
            DocAnn.VertSep(docs);
        
        /// <summary>
        /// Width lays out the document 'doc', and makes the column width
        /// of it available to a function.
        /// </summary>
        public static Doc<Unit> Width(Doc<Unit> doc, Func<int, Doc<Unit>> f) =>
            DocAnn.Width(doc, f);

        /// <summary>
        /// Fill lays out the document. It then appends spaces until
        /// the width is equal to `width`. If the width is already larger, nothing is
        /// appended.
        /// </summary>
        public static Doc<Unit> Fill(int width, Doc<Unit> doc) =>
            DocAnn.Fill(width, doc);

        /// <summary>
        /// Enclose
        /// </summary>
        public static Doc<Unit> Between(Doc<Unit> left, Doc<Unit> right, Doc<Unit> middle) =>
            DocAnn.Between(left, middle, right);
    }
}
