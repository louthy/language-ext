using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class ___StringExt
    {
        public static PString ToPString(this string value) =>
            new PString(value, Pos.Zero, Pos.Zero, Sidedness.Onside);
    }
}
