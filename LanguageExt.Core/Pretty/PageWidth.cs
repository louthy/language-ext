using System;
using System.Text;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Pretty
{
    public abstract record PageWidth
    {
        public static readonly PageWidth Default = new AvailablePerLine();
    }

    /// <summary>
    ///  - The 'Int' is the number of characters, including whitespace, that
    ///    fit in a line. A typical value is 80.
    ///
    ///  - The 'Double' is the ribbon with, i.e. the fraction of the total
    ///    page width that can be printed on. This allows limiting the length
    ///    of printable text per line. Values must be between 0 and 1, and
    ///    0.4 to 1 is typical.
    /// </summary>
    public record AvailablePerLine(int LineLength = 120, double RibbonFraction = 0.4) : PageWidth;
    public record Unbounded() : PageWidth;
}
