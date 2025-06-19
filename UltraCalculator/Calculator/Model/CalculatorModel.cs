using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Model
{
    public class CalculatorModel
    {
        public string ExecuteExpression(string expression)
        {
            string fixedExpression = expression.Replace("x", "*").Replace(":", "/");
            var result = new System.Data.DataTable().Compute(fixedExpression, null);

            return result.ToString();
        }
    }
}
