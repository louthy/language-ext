using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.Parsec
{
    public static class ___StringExt
    {
        public static PString ToPString(this string value) =>
            PString.Zero.SetValue(value);
    }
}
