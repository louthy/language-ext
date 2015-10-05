using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Executes an action post-subscription.  This is useful when the action is 
    /// going to publish to the observable.  A kind of request/response.
    /// Use the IObservable extension method: PostSubscribe(() => ...)
    /// </summary>
    public class ActionObservable<T> : IObservable<T>
    {
        readonly Action postSubscribeAction;
        readonly IObservable<T> switchTo;

        public ActionObservable(
            Action postSubscribeAction,
            IObservable<T> switchTo
            )
        {
            this.postSubscribeAction = postSubscribeAction;
            this.switchTo = switchTo;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subs = switchTo.Subscribe(observer);
            postSubscribeAction();
            return subs;
        }
    }
}
