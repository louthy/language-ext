using LanguageExt;

namespace BlazorApp.Data;

/// <summary>
/// Weather forecast data
/// </summary>
/// <param name="Date">Date</param>
/// <param name="Temperature">Temperature for the day</param>
/// <param name="Summary">Summary text</param>
public record WeatherForecast(
    DateOnly Date, 
    Temperature Temperature, 
    string Summary);
