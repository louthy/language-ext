namespace LanguageExt.Megaparsec;

public abstract record ErrorFancy<E> : IComparable<ErrorFancy<E>>
{
    public record Fail(string Value) : ErrorFancy<E>
    {
        public override int CompareTo(ErrorFancy<E>? other) =>
            other is Fail(var rhs)
                ? string.Compare(Value, rhs, StringComparison.Ordinal)
                : 1;
    }

    public record Indentation(int Ordering, int Reference, int Actual) : ErrorFancy<E>
    {
        public override int CompareTo(ErrorFancy<E>? other) =>
            other switch
            {
                Fail => 0,
                Indentation(var rhsOrdering, var rhsReference, var rhsActual) =>
                    Ordering.CompareTo(rhsOrdering) switch
                    {
                        0 => Reference.CompareTo(rhsReference) switch
                             {
                                 0     => Actual.CompareTo(rhsActual),
                                 var n => n
                             },
                        var n => n
                    },
                Custom => 1,
                _      => throw new NotSupportedException()
            };
    }

    public record Custom(E Value) : ErrorFancy<E>
    {
        public override int CompareTo(ErrorFancy<E>? other) =>
            other is Custom(var rhs)
                ? Comparer<E>.Default.Compare(Value, rhs)
                : 0;
    }

    public abstract int CompareTo(ErrorFancy<E>? other);
}
