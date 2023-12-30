using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project1
{
    internal static class Calculator
    {
        const string dgt = @"[0-9]";
        const string usnum = $"({dgt}+[.,]?{dgt}*)";
        const string snum = $"(\\(-{usnum}\\)|-?{usnum})";
        const string prp = $"(\\(-{usnum}?x\\)|-?{usnum}?x)";
        const string opd = $"({snum}|{prp})";
        const string opr = $" ?[+*/\\-^] ?";
        const string formulaPattern = $"^{opd}({opr}{opd})*$";
        static Regex reg = new Regex(formulaPattern, RegexOptions.Compiled);
        static Dictionary<int, string> opPriority = new()
        {
            {0,"^" },
            {1,"*/"},
            {2,"+~"},
        };
        static Dictionary<char, Func<Expression, Expression, Expression>> operations = new()
        {
            {'^',Expression.Power },
            {'*',Expression.Multiply },
            {'/',Expression.Divide },
            {'+',Expression.Add },
            {'~',Expression.Subtract },
        };
        public static bool isValid(string form)
        {
            return reg.IsMatch(form);
        }
        static ParameterExpression X { get; } = Expression.Parameter(typeof(double), "x");
        public static T GetDelegate<T>(string func) where T : Delegate
        {
            return Expression.Lambda<T>(GetExpression(func), new ParameterExpression[] { X }).Compile();
        }
        static Expression GetExpression(string func)
        {
            func = func.Replace(" ", "");
            func = func.Replace("(", "");
            func = func.Replace(")", "");
            func = func.Replace(".", ",");
            func = Regex.Replace(func, @"([^^\-+/*])-",@"$1~");
            if (Regex.IsMatch(func, $"^{opd}$"))
            {
                if (Regex.IsMatch(func, $"^{snum}$"))
                    return Expression.Constant(double.Parse(func));
                else
                {
                    if (Regex.IsMatch(func, @"^-?x$") )func=func.Replace("x", "1");
                    else func = func.Replace("x", "");
                    return Expression.Multiply(
                    Expression.Constant(double.Parse(func)),X);
                }
            }
            else
            {
                foreach (var opp in opPriority.Reverse())
                {
                    foreach (var c in opp.Value)
                    {
                        if (func.Contains(c))
                            return func.Split(c)
                                .Select(func => GetExpression(func))
                                .Aggregate(Expression.Empty() as Expression,
                                (a, b) =>
                                {
                                    if (a.GetType() == typeof(DefaultExpression) &&
                                        (a as DefaultExpression).Type == typeof(void))
                                        return b;
                                    else
                                        return operations[c](a, b);
                                });
                    }
                }
            }
            throw new ArgumentException();
        }
    }
}
