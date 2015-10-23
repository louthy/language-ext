using System;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public partial class Prelude
    {
        /// <summary>
        /// Millimetre
        /// </summary>
        /// <example>
        ///     Length x = 10*mm;
        /// </example>
        public static readonly Length mm = 1.Millimetres();

        /// <summary>
        /// Millimetre
        /// </summary>
        /// <example>
        ///     Length x = 10*mm;
        /// </example>
        public static readonly Length millimetre = 1.Millimetres();

        /// <summary>
        /// Millimetre
        /// </summary>
        /// <example>
        ///     Length x = 10*mm;
        /// </example>
        public static readonly Length millimetres = 1.Millimetres();

        /// <summary>
        /// Centimetre
        /// </summary>
        /// <example>
        ///     Length x = 100*cm;
        /// </example>
        public static readonly Length cm = 1.Centimetres();

        /// <summary>
        /// Centimetre
        /// </summary>
        /// <example>
        ///     Length x = 100*cm;
        /// </example>
        public static readonly Length centimetre = 1.Centimetres();

        /// <summary>
        /// Centimetre
        /// </summary>
        /// <example>
        ///     Length x = 100*cm;
        /// </example>
        public static readonly Length centimetres = 1.Centimetres();

        /// <summary>
        /// Metre
        /// </summary>
        /// <example>
        ///     Length x = 10*m;
        /// </example>
        public static readonly Length m = 1.Metres();

        /// <summary>
        /// Metre
        /// </summary>
        /// <example>
        ///     Length x = 10*m;
        /// </example>
        public static readonly Length metre = 1.Metres();

        /// <summary>
        /// Metre
        /// </summary>
        /// <example>
        ///     Length x = 10*m;
        /// </example>
        public static readonly Length metres = 1.Metres();

        /// <summary>
        /// Kilometre
        /// </summary>
        /// <example>
        ///     Length x = 7*km;
        /// </example>
        public static readonly Length km = 1.Kilometres();

        /// <summary>
        /// Kilometre
        /// </summary>
        /// <example>
        ///     Length x = 7*km;
        /// </example>
        public static readonly Length kilometre = 1.Kilometres();

        /// <summary>
        /// Kilometre
        /// </summary>
        /// <example>
        ///     Length x = 7*km;
        /// </example>
        public static readonly Length kilometres = 1.Kilometres();

        /// <summary>
        /// Inch
        /// </summary>
        /// <example>
        ///     Length x = 7*inch;
        /// </example>
        public static readonly Length inch = 1.Inches();

        /// <summary>
        /// Inch
        /// </summary>
        /// <example>
        ///     Length x = 7*inch;
        /// </example>
        public static readonly Length inches = 1.Inches();

        /// <summary>
        /// Feet
        /// </summary>
        /// <example>
        ///     Length x = 7*ft;
        /// </example>
        public static readonly Length ft = 1.Feet();

        /// <summary>
        /// Feet
        /// </summary>
        /// <example>
        ///     Length x = 7*ft;
        /// </example>
        public static readonly Length foot = 1.Feet();

        /// <summary>
        /// Feet
        /// </summary>
        /// <example>
        ///     Length x = 7*ft;
        /// </example>
        public static readonly Length feet = 1.Feet();

        /// <summary>
        /// Yard
        /// </summary>
        /// <example>
        ///     Length x = 7*yd;
        /// </example>
        public static readonly Length yd = 1.Yards();

        /// <summary>
        /// Yard
        /// </summary>
        /// <example>
        ///     Length x = 7*yd;
        /// </example>
        public static readonly Length yard = 1.Yards();

        /// <summary>
        /// Yard
        /// </summary>
        /// <example>
        ///     Length x = 7*yd;
        /// </example>
        public static readonly Length yards = 1.Yards();

        /// <summary>
        /// Mile
        /// </summary>
        /// <example>
        ///     Length x = 7*mile;
        /// </example>
        public static readonly Length mile = 1.Miles();

        /// <summary>
        /// Mile
        /// </summary>
        /// <example>
        ///     Length x = 7*mile;
        /// </example>
        public static readonly Length miles = 1.Miles();

        /// <summary>
        /// Millimetre squared
        /// </summary>
        /// <example>
        ///     Area x = 10*mm;
        /// </example>
        public static readonly Area mm2 = 1.SqMillimetres();

        /// <summary>
        /// Millimetre squared
        /// </summary>
        /// <example>
        ///     Area x = 10*mm;
        /// </example>
        public static readonly Area millimetre2 = 1.SqMillimetres();

        /// <summary>
        /// Centimetre squared
        /// </summary>
        /// <example>
        ///     Area x = 100*cm;
        /// </example>
        public static readonly Area cm2 = 1.SqCentimetres();

        /// <summary>
        /// Centimetre squared
        /// </summary>
        /// <example>
        ///     Area x = 100*cm;
        /// </example>
        public static readonly Area centimetre2 = 1.SqCentimetres();

        /// <summary>
        /// Metre squared
        /// </summary>
        /// <example>
        ///     Area x = 10*m;
        /// </example>
        public static readonly Area m2 = 1.SqMetres();

        /// <summary>
        /// Metre squared
        /// </summary>
        /// <example>
        ///     Area x = 10*m;
        /// </example>
        public static readonly Area metre2 = 1.SqMetres();

        /// <summary>
        /// Kilometre squared
        /// </summary>
        /// <example>
        ///     Area x = 7*km;
        /// </example>
        public static readonly Area km2 = 1.SqKilometres();

        /// <summary>
        /// Kilometre squared
        /// </summary>
        /// <example>
        ///     Area x = 7*km;
        /// </example>
        public static readonly Area kilometre2 = 1.SqKilometres();

        /// <summary>
        /// Second
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*s;
        /// </example>
        public static readonly Time s = 1.Seconds();

        /// <summary>
        /// Second
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*sec;
        /// </example>
        public static readonly Time sec = 1.Seconds();

        /// <summary>
        /// Second
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*sec;
        /// </example>
        public static readonly Time second = 1.Seconds();

        /// <summary>
        /// Second
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*sec;
        /// </example>
        public static readonly Time seconds = 1.Seconds();

        /// <summary>
        /// Minute
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*mins;
        /// </example>
        public static readonly Time min = 1.Minutes();

        /// <summary>
        /// Minute
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*mins;
        /// </example>
        public static readonly Time mins = 1.Minutes();

        /// <summary>
        /// Minute
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*mins;
        /// </example>
        public static readonly Time minute = 1.Minutes();

        /// <summary>
        /// Minute
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*mins;
        /// </example>
        public static readonly Time minutes = 1.Minutes();

        /// <summary>
        /// Hour
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*hr;
        /// </example>
        public static readonly Time hr = 1.Hours();

        /// <summary>
        /// Hour
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*hr;
        /// </example>
        public static readonly Time hrs = 1.Hours();

        /// <summary>
        /// Hour
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*hr;
        /// </example>
        public static readonly Time hour = 1.Hours();

        /// <summary>
        /// Hour
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*hr;
        /// </example>
        public static readonly Time hours = 1.Hours();

        /// <summary>
        /// Day
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*day;
        /// </example>
        public static readonly Time day = 1.Days();

        /// <summary>
        /// Day
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*day;
        /// </example>
        public static readonly Time days = 1.Days();

        /// <summary>
        /// Millisecond
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*ms;
        /// </example>
        public static readonly Time ms = 1.Milliseconds();

        /// <summary>
        /// Millisecond
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*ms;
        /// </example>
        public static readonly Time millisecond = 1.Milliseconds();

        /// <summary>
        /// Millisecond
        /// </summary>
        /// <example>
        ///     TimeSpan x = 7*ms;
        /// </example>
        public static readonly Time milliseconds = 1.Milliseconds();
    }
}
