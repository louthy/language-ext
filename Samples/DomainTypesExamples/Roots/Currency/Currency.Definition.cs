namespace DomainTypesExamples.Roots;

public sealed class CLP : Currency
{
    public string Code => "CLP";
    public string Name => "Chilean Peso";
    public string Symbol => "$";
    public int Decimals => 0;
}

public sealed class UF : Currency
{
    public string Code => "UF";
    public string Name => "Unidad de Fomento";
    public string Symbol => "";
    public int Decimals => 2;

}
