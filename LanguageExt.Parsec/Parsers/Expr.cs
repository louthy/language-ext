using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;

namespace LanguageExt.Parsec
{
    public static class Expr
    {
        /// <summary>
        ///  Convert an OperatorTable and basic term parser into a fully fledged 
        ///  expression parser
        /// 
        ///  buildExpressionParser(table,term) builds an expression parser for
        ///  terms term with operators from table, taking the associativity
        ///  and precedence specified in table into account.  Prefix and postfix
        ///  operators of the same precedence can only occur once (i.e. --2 is
        ///  not allowed if '-' is prefix negate).  Prefix and postfix operators
        ///  of the same precedence associate to the left  (i.e. if ++ is
        ///  postfix increment, than -2++ equals -1, not -3).
        /// 
        ///  The buildExpressionParser function takes care of all the complexity
        ///  involved in building expression parser.
        ///  
        ///  See remarks.
        ///  </summary>
        ///  <example>
        ///  This is an example of an expression parser that handles prefix signs, 
        ///  postfix increment and basic arithmetic.
        /// 
        ///    Parser<int> expr = null;
        /// 
        ///    var binary = fun((string name, Func<int,int,int> f, Assoc assoc) =>
        ///         Operator.Infix<int>( assoc,
        ///                              from x in reservedOp(name)
        ///                              select fun );
        ///                              
        ///    var prefix = fun((string name, Func<int,int> f) =>
        ///         Operator.Prefix<int>(from x in reservedOp(name)
        ///                              select fun );
        /// 
        ///    var postfix = fun((string name, Func<int,int> f) =>
        ///         Operator.Postfix<int>(from x in reservedOp(name)
        ///                               select fun );
        ///         
        ///    Operator<int>[][] table = { { prefix("-",negate), prefix("+",id) }
        ///                              , { postfix("++", incr) }
        ///                              , { binary("*", mult) Assoc.Left), binary("/", div, Assoc.Left) }
        ///                              , { binary("+", add, Assoc.Left), binary("-", subtr, Assoc.Left) } };
        ///              ]
        ///    var term              = either(parens(expr),natural).label("simple expression")
        ///            
        ///    expr                  = buildExpressionParser(table,term).label("expression")
        ///    
        ///    var res = parse(expr, "(50 + 20) / 2");
        /// </example>
        public static Parser<T> buildExpressionParser<T>(
            Operator<T>[][] operators,
            Parser<T> simpleExpr
            )
        {
            return operators.FoldBack(
                simpleExpr, 
                (term, ops) => makeParser(ops, term)
                );
        }

        static Parser<T> makeParser<T>(
            Operator<T>[] ops,
            Parser<T> term
            )
        {
            var e3 = Seq.empty<Parser<Func<T,T,T>>>();
            var e2 = Seq.empty<Parser<Func<T,T>>>();

            return ops.Fold((e3, e3, e3, e2, e2), (state, op) => op.SplitOp(state))
               .Map((rassoc, lassoc, nassoc, prefix, postfix) =>
               {
                   var rassocOp = choice(rassoc);
                   var lassocOp = choice(lassoc);
                   var nassocOp = choice(nassoc);
                   var prefixOp = choice(prefix).label("");
                   var postfixOp = choice(postfix).label("");

                   var ambigious = fun((string assoc, Parser<Func<T, T, T>> op) =>
                        attempt(
                            from x in op
                            from y in failure<T>($"ambiguous use of a {assoc} associative operator")
                            select y));

                   var ambigiousRight = ambigious("right", rassocOp);
                   var ambigiousLeft = ambigious("left", lassocOp);
                   var ambigiousNon = ambigious("non", nassocOp);

                   var postfixP = either(postfixOp, result<Func<T, T>>(x => x));

                   var prefixP = either(prefixOp, result<Func<T,T>>(x => x));

                   var termP = from pre in prefixP
                               from x in term
                               from post in postfixP
                               select post(pre(x));

                   Func<T, Parser<T>> rassocP = null;
                   Func<T, Parser<T>> rassocP1 = null;

                   rassocP1 = fun((T x) => either(rassocP(x), result(x)));

                   rassocP = fun((T x) =>
                       choice(
                           from f in rassocOp
                           from y in (from z in termP
                                      from z1 in rassocP1(z)
                                      select z1)
                           select f(x, y),
                           ambigiousLeft,
                           ambigiousNon));

                   Func<T, Parser<T>> lassocP = null;
                   Func<T, Parser<T>> lassocP1 = null;

                   lassocP1 = fun((T x) => either(lassocP(x), result(x)));

                   lassocP = fun((T x) =>
                       choice(
                           from f in lassocOp
                           from y in termP
                           from r in lassocP1(f(x, y))
                           select r,
                           ambigiousRight,
                           ambigiousNon));

                   var nassocP = fun((T x) =>
                        from f in nassocOp
                        from y in termP
                        from r in choice(ambigiousRight, ambigiousLeft, ambigiousNon, result(f(x, y)))
                        select r);

                   return from x in termP
                          from r in choice(rassocP(x), lassocP(x), nassocP(x), result(x)).label("operator")
                          select r;
               });
        }
    }
}
