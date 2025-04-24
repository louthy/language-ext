using BlazorApp.Components;
using BlazorApp.Effects;
using BlazorApp.Effects.Impl;

// Set a default runtime for the app
AppRuntime.Current = new Runtime(
    LanguageExt.Sys.Live.Implementations.TimeIO.Default,
    RndImpl.Default
    );  

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
       .AddRazorComponents()
       .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
