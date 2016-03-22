using LanguageExt.UnitsOfMeasure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt
{
    internal static class EnumerableAttrExt
    {
        internal static Attr GetAttr(this IEnumerable<Attr> attrs, string name)
        {
            var a = attrs.Where(n => n.Name == name).FirstOrDefault();
            if (a == null)
            {
                throw new Exception($"Expected '{name}' attribute");
            }
            return a;
        }

        internal static int GetNumericAttr(this IEnumerable<Attr> attrs, string name) =>
            (attrs.GetAttr(name) as NumericAttr).Value;

        internal static Time GetTimeAttr(this IEnumerable<Attr> attrs, string name) =>
            (attrs.GetAttr(name) as TimeAttr).Value;

        internal static string GEtStringAttr(this IEnumerable<Attr> attrs, string name) =>
            (attrs.GetAttr(name) as StringAttr).Value;

        internal static MessageDirective GetMsgDirAttr(this IEnumerable<Attr> attrs, string name) =>
            (attrs.GetAttr(name) as MessageDirectiveAttr).Value;

        internal static Directive dirAttr(this IEnumerable<Attr> attrs, string name) =>
            (attrs.GetAttr(name) as DirectiveAttr).Value;
    }
}
