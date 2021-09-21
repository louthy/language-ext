using System.Text;

namespace LanguageExt.Pretty
{
    internal class FastSpace
    {
        public static string Show(int n) =>
            (n < 0 ? 0 : n) switch
            {
                0 => "",
                1 => " ",
                2 => "  ",
                3 => "   ",
                4 => "    ",
                5 => "     ",
                6 => "      ",
                7 => "       ",
                8 => "        ",
                _ => FastSpaceInternal(n - 8)
            };

        static string FastSpaceInternal(int x)
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
}
