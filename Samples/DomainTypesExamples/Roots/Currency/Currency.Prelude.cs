using DomainTypesExamples.Roots;

namespace DomainTypesExamples;

public static partial class SamplePrelude
{
    public static readonly Money<CLP> clp =
        Money<CLP>.One;

    public static readonly Money<UF> uf =
        Money<UF>.One;

    public static readonly ExchangeRate<UF, CLP> ufToClp =
        Unsafe<ExchangeRate<UF, CLP>>(40_000);
}
