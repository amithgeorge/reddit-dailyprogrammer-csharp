using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RedditDailyProgrammer.Answers._206Easy
{
    /// <summary>
    /// Please ensure that the type T is a numeric one. There is no way to enforce this at compile time. 
    /// Using a non number type will likely result in a runtime exception.
    /// </summary>
    /// <typeparam name="T">A number type - int, double, float etc</typeparam>
    public class RecurrenceRelation<T>
    {
        private readonly T _termZero;
        private readonly Func<T, T> _reccurrenceFormula;

        private static readonly Dictionary<string, Func<T, T, T>> BinaryOps =
            new Dictionary<string, Func<T, T, T>>();

        private readonly Func<string, T> _parse; 

        public RecurrenceRelation(string recurrenceFormula, T termZero)
        {
            _termZero = termZero;

            var methodInfo = typeof(T).GetMethod("Parse", new[]{typeof(string)});
            if (methodInfo == null)
            {
                throw new ArgumentException("Parse method not found for type " + typeof(T));
            }

            _parse = str => (T) methodInfo.Invoke(null, new object[] {str});

            _reccurrenceFormula = ComputeRecurrenceFormula(recurrenceFormula);
        }

        public List<T> GetNTerms(int n)
        {
            return
                Enumerable.Range(1, n)
                          .Aggregate(new List<T>() {_termZero},
                                     (acc, _) =>
                                     {
                                         acc.Add(_reccurrenceFormula(acc.Last()));
                                         return acc;
                                     });
        }

        private Func<T, T> ComputeRecurrenceFormula(string opsString)
        {
            var operations =
                opsString.Split(new[] {' '})
                         .Select(term =>
                                 {
                                     var opStr = term[0].ToString();
                                     if (BinaryOps.ContainsKey(opStr) == false)
                                     {
                                         throw new InvalidDataException("Unrecognized operator: " + opStr);
                                     }

                                     T operand2;
                                     try
                                     {
                                         operand2 = _parse(term.Substring(1));
                                     }
                                     catch (Exception exception)
                                     {
                                         exception.Data["Message"] = "Couldn't parse operand2 " + term.Substring(1);
                                         throw;
                                     }

                                     return new
                                            {
                                                Operation = BinaryOps[opStr],
                                                Operand2 = operand2
                                            };
                                 });

            Func<T, T> recurrenceRelation =
                x => operations.Aggregate(x, (acc, i) => i.Operation(acc, i.Operand2));

            return recurrenceRelation;
        }

        static RecurrenceRelation()
        {
            var x = Expression.Parameter(typeof(T));
            var y = Expression.Parameter(typeof(T));

            BinaryOps["+"] = (Func<T, T, T>)Expression.Lambda(Expression.Add(x, y), x, y).Compile();
            BinaryOps["-"] = (Func<T, T, T>)Expression.Lambda(Expression.Subtract(x, y), x, y).Compile();
            BinaryOps["*"] = (Func<T, T, T>)Expression.Lambda(Expression.Multiply(x, y), x, y).Compile();
            BinaryOps["/"] = (Func<T, T, T>)Expression.Lambda(Expression.Divide(x, y), x, y).Compile();
        }
    }
}