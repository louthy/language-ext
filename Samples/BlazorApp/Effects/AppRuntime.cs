namespace BlazorApp.Effects;

public static class AppRuntime<RT>
{
    /// <summary>
    /// This will always point to the same runtime in this example, but it should be relatively
    /// obvious that switching this to a test-runtime, or any alternative with different effects
    /// implementations is how you would do mocking or DI.
    ///
    /// There may well be a more elegant way to make the runtime available to the Razor pages, but
    /// I have zero experience with Razer/Blazor, so I haven't figured it out yet.  If you know,
    /// please let me know in the repo Discussions.  Thanks! 
    /// </summary>
    public static RT? Current;
}
