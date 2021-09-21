using System;
using System.Text;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Pretty
{
    public record LayoutOptions(PageWidth PageWidth)
    {
        public static readonly LayoutOptions Default = new LayoutOptions(PageWidth.Default);
    }
}
