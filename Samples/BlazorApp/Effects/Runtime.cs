using BlazorApp.Effects.Interfaces;
using LanguageExt;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace BlazorApp.Effects;

/// <summary>
/// Concrete runtime for this project
/// </summary>
public record Runtime(RuntimeInterfaces Interfaces) : 
    Has<Eff<Runtime>, TimeIO>,
    Has<Eff<Runtime>, RndIO>
{
    public Runtime(TimeIO timeIO, RndIO rndIO) :
        this(new RuntimeInterfaces(timeIO, rndIO)){}

    static K<Eff<Runtime>, TimeIO> Has<Eff<Runtime>, TimeIO>.Ask { get; } =
        liftEff<Runtime, TimeIO>(rt => rt.Interfaces.TimeIO);
        
    static K<Eff<Runtime>, RndIO> Has<Eff<Runtime>, RndIO>.Ask { get; } =
        liftEff<Runtime, RndIO>(rt => rt.Interfaces.RndIO);
}

public record RuntimeInterfaces(TimeIO TimeIO, RndIO RndIO);
