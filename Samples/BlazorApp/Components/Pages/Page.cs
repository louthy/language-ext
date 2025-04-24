using BlazorApp.Effects;
using LanguageExt;
using Microsoft.AspNetCore.Components;
using static LanguageExt.Prelude;

namespace BlazorApp.Components.Pages;

public class Page<RT> : ComponentBase
{
    protected override async Task OnInitializedAsync() =>
        (await OnInitialized().RunAsync(AppRuntime<RT>.Current!)).SafeError();

    protected override async Task OnAfterRenderAsync(bool firstRender) => 
        (await OnAfterRender(firstRender).RunAsync(AppRuntime<RT>.Current!)).SafeError();

    protected override async Task OnParametersSetAsync() => 
        (await OnParametersSet().RunAsync(AppRuntime<RT>.Current!)).SafeError();

    public override async Task SetParametersAsync(ParameterView parameters) => 
        (await SetParameters(parameters).RunAsync(AppRuntime<RT>.Current!)).SafeError();
    
    protected new virtual Eff<RT, Unit> OnInitialized() =>
        liftEff<RT, Unit>(async _ =>
                          {
                              await base.OnInitializedAsync();
                              return unit;
                          });
    
    protected new virtual Eff<RT, Unit> OnAfterRender(bool firstRender) =>
        liftEff<RT, Unit>(async _ =>
                          {
                              await base.OnAfterRenderAsync(firstRender);
                              return unit;
                          });

    protected new virtual Eff<RT, Unit> OnParametersSet() =>
        liftEff<RT, Unit>(async _ =>
                          {
                              await base.OnParametersSetAsync();
                              return unit;
                          });
    
    protected new virtual Eff<RT, Unit> SetParameters(ParameterView parameters) =>
        liftEff<RT, Unit>(async _ =>
                          {
                              await base.SetParametersAsync(parameters);
                              return unit;
                          });
}
