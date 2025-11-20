// See https://aka.ms/new-console-template for more information

using DomainTypesExamples;
using LanguageExt;
using static LanguageExt.Prelude;

var v1 = Vector<D3, double>.From([100, 200, 300]).ThrowIfFail();
var v2 = Vector<D3, double>.From([100, 200, 300]).ThrowIfFail();
var v3 = v1 + v2;

Console.WriteLine($"{v3}");

var w1 = Vector<D128, int>.From(Range(0, 128).ToArr()).ThrowIfFail();
var w2 = Vector<D128, int>.From(Range(0, 128).ToArr()).ThrowIfFail();
var w3 = w1 + w2;

Console.WriteLine($"{w3}");

var x1 = Vector<D128, int>.From(Range(0, 128).ToArr()).ThrowIfFail();
var x2 = Vector<D128, int>.From(Range(0, 128).ToArr()).ThrowIfFail();
var x3 = x1 * x2;

Console.WriteLine($"{x3}");
