using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Represents a parser source position
    /// </summary>
    public class Pos
    {
        public readonly int Line;
        public readonly int Column;

        public Pos(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public static readonly Pos Zero = new Pos(0, 0);
    }
}
