using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    public struct TDecimal : Ord<decimal>, Floating<decimal>
    {
        public bool Equals(decimal x, decimal y) => x == y;

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// 
        /// if x less than y    : -1
        /// 
        /// if x equals y       : 0
        /// </returns>
        public int Compare(decimal x, decimal y) =>
            x.CompareTo(y);

        public decimal Add(decimal x, decimal y) => x + y;
        public decimal Difference(decimal x, decimal y) => x - y;
        public decimal Product(decimal x, decimal y) => x * y;

        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="x">left hand side of the division operation</param>
        /// <param name="y">right hand side of the division operation</param>
        /// <returns>x / y</returns>
        public decimal Divide(decimal x, decimal y) =>
            x / y;

        public decimal Abs(decimal x) => Math.Abs(x);
        public decimal Signum(decimal x) => Math.Sign(x);
        public decimal FromInteger(int x) => (decimal)x;
        public decimal FromDecimal(decimal x) => (decimal)x;
        public decimal FromFloat(float x) => (decimal)x;
        public decimal FromDouble(double x) => (decimal)x;
        public decimal Div(decimal x, decimal y) => x / y;
        public decimal FromRational(Ratio<int> x) => x.Numerator / x.Denominator;
        public decimal Pi() => (decimal)Math.PI;
        public decimal Exp(decimal x) => (decimal)Math.Exp((double)x);
        public decimal Sqrt(decimal x) => (decimal)Math.Sqrt((double)x);
        public decimal Log(decimal x) => (decimal)Math.Log((double)x);
        public decimal Pow(decimal x, decimal y) => (decimal)Math.Pow((double)x, (double)y);
        public decimal LogBase(decimal b, decimal x) => (decimal)Math.Log((double)x, (double)b);
        public decimal Sin(decimal x) => (decimal)Math.Sin((double)x);
        public decimal Cos(decimal x) => (decimal)Math.Cos((double)x);
        public decimal Tan(decimal x) => (decimal)Math.Tan((double)x);
        public decimal Asin(decimal x) => (decimal)Math.Asin((double)x);
        public decimal Acos(decimal x) => (decimal)Math.Acos((double)x);
        public decimal Atan(decimal x) => (decimal)Math.Atan((double)x);
        public decimal Sinh(decimal x) => (decimal)Math.Sinh((double)x);
        public decimal Cosh(decimal x) => (decimal)Math.Cosh((double)x);
        public decimal Tanh(decimal x) => (decimal)Math.Tanh((double)x);
        public decimal Asinh(decimal x) => Log(x + Sqrt((x * x) + 1m));
        public decimal Acosh(decimal x) => Log(x + Sqrt((x * x) - 1m));
        public decimal Atanh(decimal x) => 0.5m * Log((1m + x) / (1m - x));
        public decimal Append(decimal x, decimal y) => x + y;
    }
}
