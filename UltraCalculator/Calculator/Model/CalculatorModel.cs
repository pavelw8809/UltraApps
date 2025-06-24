using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NCalc;

namespace Calculator.Model
{
    public class CalculatorModel
    {
        public string ExecuteExpression(string expression)
        {
            try
            {
                string processedExpression = Preprocess(expression);

                var runExpression = new Expression(processedExpression);

                if (processedExpression.Contains("Pow"))
                {
                    runExpression.EvaluateFunction += (name, args) =>
                    {
                        if (name.Equals("Pow", StringComparison.OrdinalIgnoreCase) && args.Parameters.Length == 2)
                        {
                            var baseValue = Convert.ToDouble(args.Parameters[0].Evaluate());
                            var exp = Convert.ToDouble(args.Parameters[1].Evaluate());
                            args.Result = Math.Pow(baseValue, exp);
                        }
                    };
                }

                var result = runExpression.Evaluate();

                return result.ToString().Replace(",", ".");
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message} - {expression}";
            }
        }

        private string Preprocess(string expression)
        {
            expression = expression.Replace(",", ".");
            expression = ReplaceBasics(expression);
            expression = ReplaceRoots(expression);
            expression = ReplaceExponents(expression);

            return expression;
        }

        private string ReplaceBasics(string expression)
        {
            return expression.Replace("x", "*").Replace(":", "/");
        }

        private string ReplaceRoots(string expression)
        {
            expression = Regex.Replace(
                expression, @"√\(([^()]+)\)", x => $"Sqrt({x.Groups[1].Value})"
            );

            expression = Regex.Replace(
                expression, @"√(\d+(\.\d+)?)", m => $"Sqrt({m.Groups[1].Value})"
            );

            return expression;
        }

        private string ReplaceExponents(string expression)
        {
            expression = Regex.Replace(
                expression, @"(\d+(\.\d+)?)\^(\d+(\.\d+)?)", x => $"Pow({x.Groups[1].Value}, {x.Groups[3].Value})"
            );

            return expression;
        }
    }
}
