using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.UnitsOfMeasure
{
    public static class __UnitsTimeExt
    {
        public static TimeSpan Milliseconds(this int self) =>
            TimeSpan.FromMilliseconds(self);

        public static TimeSpan Milliseconds(this float self) =>
            TimeSpan.FromMilliseconds(self);

        public static TimeSpan Milliseconds(this double self) =>
            TimeSpan.FromMilliseconds(self);

        public static TimeSpan Seconds(this int self) =>
            TimeSpan.FromSeconds(self);

        public static TimeSpan Seconds(this float self) =>
            TimeSpan.FromSeconds(self);

        public static TimeSpan Seconds(this double self) =>
            TimeSpan.FromSeconds(self);

        public static TimeSpan Minutes(this int self) =>
            TimeSpan.FromMinutes(self);

        public static TimeSpan Minutes(this float self) =>
            TimeSpan.FromMinutes(self);

        public static TimeSpan Minutes(this double self) =>
            TimeSpan.FromMinutes(self);

        public static TimeSpan Hours(this int self) =>
            TimeSpan.FromHours(self);

        public static TimeSpan Hours(this float self) =>
            TimeSpan.FromHours(self);

        public static TimeSpan Hours(this double self) =>
            TimeSpan.FromHours(self);

        public static TimeSpan Days(this int self) =>
            TimeSpan.FromDays(self);

        public static TimeSpan Days(this float self) =>
            TimeSpan.FromDays(self);

        public static TimeSpan Days(this double self) =>
            TimeSpan.FromDays(self);
    }
}


