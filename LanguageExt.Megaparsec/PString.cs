using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

/// <summary>
/// Type that wraps up a string and its start and length.  This allows us to implement
/// the low-level streaming trait and get a high-performance parser.
/// </summary>
/// <param name="value">Text</param>
/// <param name="start">Start of text</param>
/// <param name="length">Length of text</param>
[CollectionBuilder(typeof(PString), nameof(PString.From))]
public readonly struct PString(Arr<char> value, long start, long length) :
    TokenStream<PString, char>,
    IEquatable<PString>,
    IComparable<PString>
{
    /// <summary>
    /// Backing string
    /// </summary>
    Arr<char> Value => value;
    
    /// <summary>
    /// Start position in the backing string
    /// </summary>
    long Start => start;
    
    /// <summary>
    /// Number of characters in the backing string
    /// </summary>
    long Length => length;
    
    /// <summary>
    /// Empty parser string
    /// </summary>
    public static readonly PString Empty = 
        new(Arr<char>.Empty, 0, 0);

    public PString(ReadOnlySpan<char> value) : this(toArr(value), 0, value.Length)
    {
    }

    public PString(ReadOnlySpan<char> value, long start, long length) : this(toArr(value), start, length)
    {
    }

    public PString(string Value) : this(toArr(Value), 0, Value.Length)
    {
    }
    
    public static PString From(string value) =>
        new (value);

    public char this[long ix] =>
        ix < 0 || ix >= Length 
            ? throw new IndexOutOfRangeException() 
            : Value[Start + ix];

    public char this[Index ix] =>
        ix.Value < 0 || ix.Value >= Length
            ? throw new IndexOutOfRangeException()
            : ix.IsFromEnd
                ? Value[Start + Length - ix.Value]
                : Value[Start          + ix.Value];
    
    public PString Slice(int offset, int amount)
    {
        if (amount < 0) return Empty;
        return amount - offset < Length
            ? new PString(Value, Start + offset, amount)
            : this;
    }    
    
    public PString Slice(int amount)
    {
        if (amount < 0) return Empty;
        return amount < Length
            ? new PString(Value, Start, amount)
            : this;
    }

    public override string ToString() => 
        Start + Length > int.MaxValue
            ? "..."
            : new (Value.AsSpan((int)Start, (int)Length));

    public PStringEnum GetEnumerator() =>
        new (this);

    static bool TokenStream<PString, char>.IsTab(char token) =>
        token == '\t';

    static bool TokenStream<PString, char>.IsNewline(char token) => 
        token == '\n';

    static ReadOnlySpan<char> TokenStream<PString, char>.TokenToString(char token) => 
        new ([token]);

    static PString TokenStream<PString, char>.TokenToChunk(in char token) => 
        new (Arr.singleton(token), 0, 1);

    static PString TokenStream<PString, char>.TokensToChunk(in ReadOnlySpan<char> token) => 
        new (new string(token), 0, token.Length);

    static ReadOnlySpan<char> TokenStream<PString, char>.ChunkToTokens(in PString tokens) => 
        tokens.Value.AsSpan((int)tokens.Start, (int)tokens.Length);

    static long TokenStream<PString, char>.ChunkLength(in PString tokens) => 
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

    static bool TokenStream<PString, char>.Take(long amount, in PString stream, out PString head, out PString tail)
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

    static void TokenStream<PString, char>.TakeWhile(
        Func<char, bool> predicate, 
        in PString stream,
        out PString head,
        out PString tail)
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
        head = new PString(value, start, count);
        tail = new PString(value, end, length - count);
    }

    public bool Equals(PString other)
    {
        if(Length != other.Length) return false;
        var spanA = Value.AsSpan((int)Start, (int)Length);
        var spanB = other.Value.AsSpan((int)other.Start, (int)other.Length);
        return spanA.Equals(spanB, StringComparison.Ordinal);
    }
    
    public int CompareTo(PString other)
    {
        var spanA = Value.AsSpan((int)Start, (int)Length);
        var spanB = other.Value.AsSpan((int)other.Start, (int)other.Length);
        return spanA.CompareTo(spanB, StringComparison.Ordinal);
    }
    
    public struct PStringEnum
    {
        readonly long start;
        readonly Arr<char> target;
        readonly long end;
        long current;

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
            target[(int)current];
    }
}
