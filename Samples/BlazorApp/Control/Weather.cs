using BlazorApp.Data;
using BlazorApp.Effects;
using BlazorApp.Effects.Interfaces;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace BlazorApp.Control;

public static class Weather<RT>
    where RT : 
        Has<Eff<RT>, TimeIO>, 
        Has<Eff<RT>, RndIO>
{
    /// <summary>
    /// Some example summaries
    /// </summary>
    static readonly Seq<string> summaries =
        ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    /// <summary>
    /// Get the next five-days weather forecast
    /// </summary>
    public static Eff<RT, Seq<WeatherForecast>> forecastNextFiveDays =>
        from d in dateNow
        from f in Range(1, 5).AsIterable()
                             .Map(d.AddDays)
                             .Traverse(forecast)
        select toSeq(f);          

    /// <summary>
    /// Gets a weather forecast for a given date
    /// </summary>
    public static Eff<RT, WeatherForecast> forecast(DateOnly date) =>
        from t in randomTemperature
        from s in randomSummary
        select new WeatherForecast(date, t, s);

    /// <summary>
    /// Get the date now
    /// </summary>
    static Eff<RT, DateOnly> dateNow =>
        Time<RT>.now.Map(DateOnly.FromDateTime).As();
    
    /// <summary>
    /// Generate a random temperature between -20 and 55 degrees celsius
    /// </summary>
    static Eff<RT, Temperature> randomTemperature =>
        Rnd<RT>.next(-20, 55).Map(c => c.Celsius()).As();

    /// <summary>
    /// Generate a random summary
    /// </summary>
    static Eff<RT, string> randomSummary =>
        Rnd<RT>.next(summaries.Length).Map(ix => summaries[ix]).As();
}
