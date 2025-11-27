using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

internal static class Reach<S, T>
    where S : TokenStream<S, T>
{
    // Buffer size for collecting a line of text
    const int lineBufferSize = 256;
    const int lineBufferSizeMinus3 = lineBufferSize - 3;
    
    /// <summary>
    /// TODO 
    /// </summary>
    /// <param name="o">Offset to reach</param>
    /// <param name="posState">Initial `PosState` to use</param>
    /// <returns></returns>
    public static (string Line, PosState<S> UpdatedState) offset(int o, PosState<S> posState) 
    {
        // Split the input stream at the current offset position
        // TODO: Consider if the maths here are correct
        // TODO: Probably TokenStream should support Split
        S.Take(posState.Offset, posState.Input, out _, out var input);
        S.Take(o, input, out var pre, out var post); 
        //var (pre, post) = //splitAt(o - posState.Offset, posState.Input); -- OLD VERSION OF ABOVE LINE
        
        // Walk the input stream and collect the position and line-text
        Span<char> ns    = stackalloc char[lineBufferSize];
        var        spos  = posState.SourcePos;
        var        c     = spos.Column.Value;
        var        l     = spos.Line.Value;
        var        w     = posState.TabWidth;
        var        ix    = 0;
        
        while(S.Take1(pre, out var tok, out pre)) 
        {
            if (S.IsNewline(tok))
            {
                // Newlines add 1 to the line number, reset the column number to 1, and reset the line text
                l += 1;
                c = 1;
                ix = 0;
            }
            else if (S.IsTab(tok))
            {
                // Tabs add 0-<tab width> spaces to align with the tab stop 
                var tw = w - (c - 1) % w;
                c += tw;
                for (var i = 0; i < tw; i++)
                {
                    if(ix < lineBufferSizeMinus3 ) ns[ix] = ' ';
                    ix++;
                }
            }
            else
            {
                // Regular tokens should be converted to char strings and added to the line text
                var tchars = S.TokenToString(tok);
                foreach (var tch in tchars)
                {
                    if (ix < lineBufferSizeMinus3) ns[ix] = tch;
                    c++;
                    ix++;
                }
            }

            // Truncate long lines
            if (ix == lineBufferSize)
            {
                ns[lineBufferSize - 1] = '.';
                ns[lineBufferSize - 2] = '.';
                ns[lineBufferSize - 3] = '.';
            }
        }
        spos = spos with { Column = c, Line = l };
        var sameLine = spos.Line == posState.SourcePos.Line;
        var line = sameLine
                       ? posState.LinePrefix + (ix == 0
                                                    ? "<empty line>"
                                                    : new string(ns[..Math.Min(ix, lineBufferSize)]))
                       : ix == 0
                           ? "<empty line>"
                           : new string(ns[..Math.Min(ix, lineBufferSize)]); 
        
        var pstate = new PosState<S>(
            post, 
            Math.Max(o, posState.Offset), 
            spos, 
            posState.TabWidth, 
            posState.LinePrefix);
        
        return (line, pstate);
    }
    
    /// <summary>
    /// TODO 
    /// </summary>
    /// <param name="o">Offset to reach</param>
    /// <param name="posState">Initial `PosState` to use</param>
    /// <returns></returns>
    public static PosState<S> offsetNoLine(int o, PosState<S> posState)                    
    {
        // Split the input stream at the current offset position
        // TODO: Consider if the maths here are correct
        // TODO: Probably TokenStream should support Split
        S.Take(posState.Offset, posState.Input, out _, out var input);
        S.Take(o, input, out var pre, out var post); 
        //var (pre, post) = //splitAt(o - posState.Offset, posState.Input); -- OLD VERSION OF ABOVE LINE
        
        // Walk the input stream and collect the position and line-text
        var spos = posState.SourcePos;
        var c    = spos.Column.Value;
        var l    = spos.Line.Value;
        var w    = posState.TabWidth;
        
        while(S.Take1(pre, out var tok, out pre)) 
        {
            if (S.IsNewline(tok))
            {
                // Newlines add 1 to the line number, reset the column number to 1, and reset the line text
                l += 1;
                c = 1;
            }
            else if (S.IsTab(tok))
            {
                // Tabs add 0-<tab width> spaces to align with the tab stop 
                var tw = w - (c - 1) % w;
                c += tw;
            }
            else
            {
                // Regular tokens should be converted to char strings and added to the line text
                var tchars = S.TokenToString(tok);
                c += tchars.Length;
            }
        }
        
        spos = spos with { Column = c, Line = l };
        
        var pstate = new PosState<S>(
            post, 
            Math.Max(o, posState.Offset), 
            spos, 
            posState.TabWidth, 
            posState.LinePrefix);

        return pstate;
    }
        
    
    /// <summary>
    /// Replace tab characters with the given number of spaces
    /// </summary>
    /// <param name="tabWidth">Tab width</param>
    /// <param name="str">String to expand</param>
    /// <returns></returns>
    public static string expandTab(Pos tabWidth, string str)
    {
        // Find out the new size of the string
        var tabSize = tabWidth.Value;
        var length  = 0;
        
        foreach (var c in str)
        {
            if (c == '\t')
            {
                length += tabSize - length % tabSize;
            }
            else
            {
                length++;
            }
        }

        if (str.Length == length)
        {
            // Bail early when there are no tabs to expand
            return str;
        }
        
        Span<char> ns    = stackalloc char[length];
        var        index = 0;
        foreach (var c in str)
        {
            if (c == '\t')
            {
                var tl = tabSize - index % tabSize;
                for (var i = 0; i < tl; i++)
                {
                    ns[index++] = ' ';
                }
            }
            else
            {
                ns[index++] = c;
            }
        }
        return new string(ns);
    }
}
