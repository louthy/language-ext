using System;

namespace LanguageExt;

/// <summary>
/// Executes an action post-subscription.  This is useful when the action is 
/// going to publish to the observable.  A kind of request/response.
/// Use the IObservable extension method: PostSubscribe(() => ...)
/// </summary>
public class ActionObservable<T>(
    Action PostSubscribeAction,
    IObservable<T> SwitchTo)
    : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        var subs = SwitchTo.Subscribe(observer);
        PostSubscribeAction();
        return subs;
    }
}
