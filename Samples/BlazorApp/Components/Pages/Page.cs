using BlazorApp.Effects;
using LanguageExt;
using Microsoft.AspNetCore.Components;
using static LanguageExt.Prelude;

namespace BlazorApp.Components.Pages;

public class Page : ComponentBase
{
    protected override async Task OnInitializedAsync() =>
        (await OnInitialized().RunAsync(AppRuntime.Current!)).SafeError();

    protected override async Task OnAfterRenderAsync(bool firstRender) => 
        (await OnAfterRender(firstRender).RunAsync(AppRuntime.Current!)).SafeError();

    protected override async Task OnParametersSetAsync() => 
        (await OnParametersSet().RunAsync(AppRuntime.Current!)).SafeError();

    public override async Task SetParametersAsync(ParameterView parameters) => 
        (await SetParameters(parameters).RunAsync(AppRuntime.Current!)).SafeError();
    
    protected new virtual Eff<Runtime, Unit> OnInitialized() =>
        liftEff<Runtime, Unit>(async _ =>
                               {
                                   await base.OnInitializedAsync();
                                   return unit;
                               });
    
    protected new virtual Eff<Runtime, Unit> OnAfterRender(bool firstRender) =>
        liftEff<Runtime, Unit>(async _ =>
                               {
                                   await base.OnAfterRenderAsync(firstRender);
                                   return unit;
                               });

    protected new virtual Eff<Runtime, Unit> OnParametersSet() =>
        liftEff<Runtime, Unit>(async _ =>
                               {
                                   await base.OnParametersSetAsync();
                                   return unit;
                               });
    
    protected new virtual Eff<Runtime, Unit> SetParameters(ParameterView parameters) =>
        liftEff<Runtime, Unit>(async _ =>
                               {
                                   await base.SetParametersAsync(parameters);
                                   return unit;
                               });
}
