using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

/// <summary>
/// Type that wraps up a string and its start and length.  This allows us to implement
/// the low-level streaming trait and get a high-performance parser.
/// </summary>
/// <param name="value">Text</param>
/// <param name="start">Start of text</param>
/// <param name="length">Length of text</param>
public readonly struct PString(string value, int start, int length) :
    TraversableTokenStream<PString, char>,
    IEquatable<PString>,
    IComparable<PString>
{
    readonly string Value = value;
    readonly int Start = start;
    readonly int Length = length;
    
    /// <summary>
    /// Empty parser string
    /// </summary>
    public static readonly PString Empty = 
        new("", 0, 0);

    public PString(string Value) : this(Value, 0, Value.Length)
    {
    }

    public char this[int ix] =>
        ix < 0 || ix >= Length 
            ? throw new IndexOutOfRangeException() 
            : Value[Start + ix];

    public char this[Index ix] =>
        ix.Value < 0 || ix.Value >= Length
            ? throw new IndexOutOfRangeException()
            : ix.IsFromEnd
                ? Value[Start + Length - ix.Value]
                : Value[Start          + ix.Value];
    
    public PString Substring(int amount)
    {
        if(amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (amount <= Length)
        {
            var value = Value;
            return new PString(value, Start, amount);
        }
        else
        {
            return this;
        }
    }

    public override string ToString() => 
        new (Value.AsSpan(Start, Length));

    public PStringEnum GetEnumerator() =>
        new (this);

    static PString TokenStream<PString, char>.TokenToChunk(in char token) => 
        new (token.ToString(), 0, 1);

    static PString TokenStream<PString, char>.TokensToChunk(in ReadOnlySpan<char> token) => 
        new (token.ToString(), 0, token.Length);

    static ReadOnlySpan<char> TokenStream<PString, char>.ChunkToTokens(in PString tokens) => 
        tokens.Value.AsSpan(tokens.Start, tokens.Length);

    static int TokenStream<PString, char>.ChunkLength(in PString tokens) => 
        tokens.Length;

    static bool TokenStream<PString, char>.Take1(in PString stream, out char head, out PString tail)
    {
        if (stream.Length > 0)
        {
            var start = stream.Start;
            var value = stream.Value;
            head = value[start];
            tail = new PString(value, start + 1, stream.Length - 1);
            return true;
        }
        else
        {
            head = '?';
            tail = stream;
            return false;
        }
    }

    static bool TokenStream<PString, char>.Take(int amount, in PString stream, out PString head, out PString tail)
    {
        // If the requested length `amount` is 0 (or less), `false` should
        // not be returned, instead `true` and `(out Empty, out stream)` should be returned.
        if (amount <= 0)
        {
            head = Empty;
            tail = stream;
            return true;
        }

        // If the requested length is greater than 0 and the stream is
        // empty, `false` should be returned indicating end-of-input.
        if (stream.Length <= 0)
        {
            head = Empty;
            tail = stream;
            return false;
        }
        
        // In other cases, take chunk of length `amount` (or shorter if the
        // stream is not long enough) from the input stream and return the
        // chunk along with the rest of the stream.
        amount = Math.Min(amount, stream.Length);
        var start = stream.Start;
        var value = stream.Value;
        head = new PString(value, start, amount);
        tail = new PString(value, start + amount, stream.Length - amount);
        return true;
    }

    static PString TokenStream<PString, char>.TakeWhile(
        Func<char, bool> predicate, 
        in PString stream,
        out ReadOnlySpan<char> head)
    {
        var value  = stream.Value;
        var start  = stream.Start;
        var end    = start;
        var length = stream.Length;
        var count  = 0;

        while (count < length && !predicate(value[end]))
        {
            end++;
            count++;
        }
        head = value.AsSpan(start, count);
        return new PString(value, end, length - count);
    }

    static (Option<LineText> Line, PosState<PString> Updated) TraversableTokenStream<PString, char>.ReachOffset(
        int offset,
        PosState<PString> pst)
    {
        var newpos = TraversableTokenStream.reachOffsetNoLine<PString, char>(offset, pst);
        var lineText = LineText.Lift(() => pst.SourcePos.Line == newpos.SourcePos.Line
                                               ? $"{pst.LinePrefix}{pst.Input.Substring(offset)}"
                                               : pst.Input.Substring(offset).ToString());

        return (lineText, newpos);
    }

    static PosState<PString> TraversableTokenStream<PString, char>.ReachOffsetNoLine(
        int offset,
        PosState<PString> pst)
    {
        offset -= pst.Offset;
        if (offset == 0) return pst;
        var max = pst.Input.Length - pst.Offset;
        offset = Math.Clamp(offset, 0, max);
        var current = pst.Offset;
        var srcpos  = pst.SourcePos;
        var tab     = pst.TabWidth;
        
        while (offset > 0)
        {
            offset--;
            current++;
            if (current >= max) break;
            
            switch (pst.Input[pst.Offset + current])
            {
                case '\n':
                    srcpos = srcpos.NextLine;
                    break;
                
                case '\t':
                    srcpos = srcpos.Next(tab);
                    break;
                
                default:
                    srcpos = srcpos.NextToken;
                    break;
            }
        }
        return pst with { Offset = current, SourcePos = srcpos };
    }

    public bool Equals(PString other)
    {
        if(Length != other.Length) return false;
        var spanA = Value.AsSpan(Start, Length);
        var spanB = other.Value.AsSpan(other.Start, other.Length);
        return spanA.Equals(spanB, StringComparison.Ordinal);
    }
    
    public int CompareTo(PString other)
    {
        var spanA = Value.AsSpan(Start, Length);
        var spanB = other.Value.AsSpan(other.Start, other.Length);
        return spanA.CompareTo(spanB, StringComparison.Ordinal);
    }
    
    public struct PStringEnum
    {
        readonly int start;
        readonly string target;
        readonly int end;
        int current;

        internal PStringEnum(PString ps)
        {
            target = ps.Value;
            start = current = ps.Start - 1;
            end = ps.Start + ps.Length;
        }
        
        public bool MoveNext()
        {
            current++;
            return current >= end; 
        }

        public void Reset() =>
            current = start;

        public char Current => 
            target[current];
    }
}
