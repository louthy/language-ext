using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.DataTypes.Conditional
{
    public static class ConditionalExtensions
    {
        // X, X, X
        public static Conditional<A, B> Cond<A, B>(this A val, Func<A, bool> condition, Func<A, B> returns) =>
            condition(val)
                ? new Conditional<A, B>(val, returns(val))
                : new Conditional<A, B>(val);

        // Task<X>, X, X
        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<A> val, Func<A, bool> condition, Func<A, B> returns) =>
            Cond(await val, condition, returns);

        // X, Task<X>, X
        public static async Task<Conditional<A, B>> Cond<A, B>(this A val, Func<A, Task<bool>> condition, Func<A, B> returns) =>
            await condition(val)
                ? new Conditional<A, B>(val, returns(val))
                : new Conditional<A, B>(val);

        // Task<X>, Task<X>, X
        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<A> val, Func<A, Task<bool>> condition, Func<A, B> returns) =>
            await Cond(await val, condition, returns);

        // X, X, Task<X>
        public static async Task<Conditional<A, B>> Cond<A, B>(this A val, Func<A, bool> condition, Func<A, Task<B>> returns) =>
            condition(val)
                ? new Conditional<A, B>(val, await returns(val))
                : new Conditional<A, B>(val);

        // Task<X>, X, Task<X>
        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<A> val, Func<A, bool> condition, Func<A, Task<B>> returns) =>
            await Cond(await val, condition, returns);

        // X, Task<X>, Task<X>
        public static async Task<Conditional<A, B>> Cond<A, B>(this A val, Func<A, Task<bool>> condition, Func<A, Task<B>> returns) =>
            await condition(val)
                ? new Conditional<A, B>(val, await returns(val))
                : new Conditional<A, B>(val);

        // Task<X>, Task<X>, Task<X>
        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<A> val, Func<A, Task<bool>> condition, Func<A, Task<B>> returns) =>
            await Cond(await val, condition, returns);

        
        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<Conditional<A, B>> continuation, Func<A, bool> condition, Func<A, B> returns) =>
            (await continuation).Cond(condition, returns);


        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<Conditional<A, B>> continuation, Func<A, Task<bool>> condition, Func<A, B> returns) =>
            await (await continuation).Cond(condition, returns);

        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<Conditional<A, B>> continuation, Func<A, bool> condition, Func<A, Task<B>> returns) =>
            await (await continuation).Cond(condition, returns);

        public static async Task<Conditional<A, B>> Cond<A, B>(this Task<Conditional<A, B>> continuation, Func<A, Task<bool>> condition, Func<A, Task<B>> returns) =>
            await (await continuation).Cond(condition, returns);

        public static async Task<B> Else<A, B>(this Task<Conditional<A, B>> continuation, Func<A, B> returns) =>
            (await continuation).Else(returns);

        public static async Task<B> Else<A, B>(this Task<Conditional<A, B>> continuation, Func<A, Task<B>> returns) =>
            await (await continuation).Else(returns);
    }
}
