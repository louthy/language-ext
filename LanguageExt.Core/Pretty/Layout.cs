#nullable enable
using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Pretty
{
    public static class Layout
    {
        public static DocStream<A> pretty<A>(LayoutOptions options, Doc<A> doc)
        {
            return layout(new FittingPredicate<A>(static (p, m, w, d) => go(w, d)), options, doc);

            static bool go(Option<int> mx, DocStream<A> doc)
            {
                while (true)
                {
                    switch ((mx.Case, doc))
                    {
                        case (null, _):
                            return true;

                        case (int w, _) when w < 0:
                            return false;

                        case (_, SFail<A>):
                            return false;

                        case (_, SEmpty<A>):
                            return false;

                        case (int w, SChar<A> (_, var x)):
                            mx = Some(w - 1);
                            doc = x;
                            break;
                        
                        case (int w, SText<A> (var t, var x)):
                            mx  = Some(w - t.Length);
                            doc = x;
                            break;
                            
                        case (_, SLine<A>):
                            return true;
                            
                        case (int w, SAnnPush<A> (_, var x)):
                            mx = Some(w);
                            doc = x;
                            break;
                        
                        case (int w, SAnnPop<A> (var x)):
                            mx  = Some(w);
                            doc = x;
                            break;
                        
                        default: throw new NotSupportedException();
                    }
                }
            }
        }
        
        public static DocStream<A> smart<A>(LayoutOptions options, Doc<A> doc)
        {
            return layout(new FittingPredicate<A>(go), options, doc);

            static bool go(PageWidth pageWidth, int minNest, Option<int> mx, DocStream<A> doc)
            {
                while (true)
                {
                    switch ((pageWidth, minNest, mx.Case, doc))
                    {
                        case (_, _, null, _):
                            return false;

                        case (_, _, int w, _) when w < 0:
                            return false;

                        case (_, _, _, SFail<A>):
                            return false;

                        case (_, _, _, SEmpty<A>):
                            return true;

                        case (var pw, var m, int w, SChar<A> (_, var x)):
                            mx  = Some(w - 1);
                            doc = x;
                            break;
                        
                        case (var pw, var m, int w, SText<A> (var t, var x)):
                            mx  = Some(w - t.Length);
                            doc = x;
                            break;
                            
                        case (AvailablePerLine (var cpl, _), var m, _, SLine<A>(var i, var x)) when m < i :
                            mx  = Some(cpl - i);
                            doc = x;
                            break;
                            
                        case (_, _, _, SLine<A>):
                            return true;
                            
                        case (var pw, var m, int w, SAnnPush<A> (_, var x)):
                            mx  = Some(w);
                            doc = x;
                            break;
                        
                        case (var pw, var m, int w, SAnnPop<A> (var x)):
                            mx  = Some(w);
                            doc = x;
                            break;
                        
                        default: throw new NotSupportedException();
                    }
                }
            }
        }
        
        public static DocStream<A> layout<A>(
            FittingPredicate<A> fittingPredicate,
            LayoutOptions options,
            Doc<A> doc)
        {
            return best(0, 0, new Cons<A>(0, doc, Nil<A>.Default));

            DocStream<A> best(int nest, int col, LayoutPipeline<A> pipeline) =>
                (nest, col, pipeline) switch
                {
                    (_, _, Nil<A>)                        => SEmpty<A>.Default,
                    (var nl, var cc, UndoAnn<A> (var ds)) => new SAnnPop<A>(best(nl, cc, ds)),
                    (var nl, var cc, Cons<A> (var i, var d, var ds)) =>
                        d switch
                        {
                            DocFail<A>                     => SFail<A>.Default,
                            DocEmpty<A>                    => best(nl, cc, ds),
                            DocChar<A> (var c)             => new SChar<A>(c, best(nl, cc + 1, ds)),
                            DocText<A> (var t)             => new SText<A>(t, best(nl, cc + t.Length, ds)),
                            DocLine<A>                     => new SLine<A>(i, best(i, i, ds)),
                            DocFlatAlt<A>(var x, _)        => best(nl, cc, new Cons<A>(i, x, ds)),
                            DocCat<A>(var x, var y)        => best(nl, cc, new Cons<A>(i, x, new Cons<A>(i, y, ds))),
                            DocNest<A>(var j, var x)       => best(nl, cc, new Cons<A>(i + j, x, ds)),
                            DocUnion<A>(var x, var y)      => selectNicer(fittingPredicate, nl, cc, best(nl, cc, new Cons<A>(i, x, ds)), best(nl, cc, new Cons<A>(i, y, ds))),
                            DocColumn<A>(var f)            => best(nl, cc, new Cons<A>(i, f(cc), ds)),
                            DocPageWidth<A>(var f)         => best(nl, cc, new Cons<A>(i, f(options.PageWidth), ds)),
                            DocNesting<A>(var f)           => best(nl, cc, new Cons<A>(i, f(i), ds)),
                            DocAnnotate<A>(var ann, var x) => new SAnnPush<A>(ann, best(nl, cc, new Cons<A>(i, x, new UndoAnn<A>(ds)))),
                            _                              => throw new NotSupportedException()
                        },
                    _ => throw new NotSupportedException()
                };

            DocStream<A> selectNicer(FittingPredicate<A> fits, int lineIndent, int currentColumn, DocStream<A> x, DocStream<A> y)
            {
                var minNestingLevel = Math.Min(lineIndent, currentColumn);
                var ribbonWidth = options.PageWidth switch
                                  {
                                      AvailablePerLine (var lineLength, var ribbonFraction) =>
                                          Some(Math.Max(0, Math.Min(lineLength, (int) (((double) lineLength) * ribbonFraction)))),
                                      Unbounded => None,
                                      _         => throw new NotSupportedException()
                                  };
                var availableWidth = from columnsLeftInLine in options.PageWidth switch
                                                               {
                                                                   AvailablePerLine(var cpl, var ribbonFrac) => Some(cpl - currentColumn),
                                                                   Unbounded                                 => None, _ => throw new NotSupportedException()
                                                               }
                                     from columnsLeftInRibbon in from li in Some(lineIndent)
                                                                 from rw in ribbonWidth
                                                                 from cc in Some(currentColumn)
                                                                 select li + rw - cc
                                     select Math.Min(columnsLeftInLine, columnsLeftInRibbon);

                return fits.FP(options.PageWidth, minNestingLevel, availableWidth, x)
                           ? x
                           : y;
            }
        }
    }
}
#nullable disable
