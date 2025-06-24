using Calculator.Commands;
using Calculator.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.ViewModel
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _calculator = new();
        private string _display = "0";

        public string Display
        {
            get => _display;
            set { 
                _display = value; 
                OnPropertyChanged(nameof(Display)); 
            }
        }

        public string LastExpression
        {
            get => _lastExpression;
            set
            {
                _lastExpression = value;
                OnPropertyChanged(nameof(LastExpression));
            }
        }

        private string _expression = "";
        private string _lastExpression = "";


        public ICommand NumberCommand { get; }
        public ICommand OperationCommand { get; }
        public ICommand ConfirmationCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand RemoveCommand { get; }

        public CalculatorViewModel()
        {
            NumberCommand = new CalculatorCommand(ExecuteNumber);
            OperationCommand = new CalculatorCommand(ExecuteOperation);
            ConfirmationCommand = new CalculatorCommand(ExecuteConfirmation, CanExecuteConfirmation);
            ClearCommand = new CalculatorCommand(_ => ExecuteClear());
            RemoveCommand = new CalculatorCommand(_ => ExecuteRemove());
        }

        private void ExecuteNumber(object parameter)
        {
            string parsedParameter = parameter.ToString();

            if (_expression == "0")
            {
                if (parsedParameter == "0")
                {
                    return;
                }
                else
                {
                    _expression = parsedParameter;
                }
            }
            else
            {
                if (parsedParameter == "^")
                {
                    if (string.IsNullOrWhiteSpace(_expression) || Regex.IsMatch(_expression, @"^0+$") || "+-x:^".Contains(_expression.Last()))
                    {
                        return;
                    }
                    else
                    {
                        _expression += parsedParameter;
                    }
                }
                else
                {
                    _expression += parsedParameter;
                }
            }
                
            Display = FormatDisplayContent(_expression);
            RaiseCanExecuteChanged();
        }

        private void ExecuteOperation(object parameter)
        {
            string oper = parameter.ToString();

            if (oper == "√")
            {
                if (!string.IsNullOrEmpty(_expression) && char.IsDigit(_expression.Last())) {
                    ApplySquareRoot();
                }
                else
                {
                    _expression += $"{oper}(";
                    Display = FormatDisplayContent(_expression);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_expression) || "+-x:".Contains(_expression.Last()))
                {
                    return;
                }
                else
                {
                    _expression += oper;
                }
            }
            
            Display = FormatDisplayContent(_expression);
        }

        private void ExecuteConfirmation(object parameter)
        {
            string info = "";
            try
            {
                int openBrackets = _expression.Count(c => c == '(');
                int closeBrackets = _expression.Count(c => c == ')');

                while (closeBrackets < openBrackets)
                {
                    _expression += ")";
                    closeBrackets++;
                }

                info = _expression;
                _lastExpression = _expression;
                _expression = _calculator.ExecuteExpression(_expression);

                if (_expression.StartsWith("ERROR:"))
                {
                    LastExpression = _expression;
                    _expression = "0";
                    Display = FormatDisplayContent(_expression);
                    return;
                }

                LastExpression = FormatDisplayContent(_lastExpression);
                Display = _expression;
            }
            catch (Exception ex)
            {
                _expression = "";
                Display = FormatDisplayContent(_expression);
                LastExpression = $"ERROR: {ex.Message}";
            }
        }

        private void ExecuteClear()
        {
            Display = "0";
            _expression = "";
            _lastExpression = "";
            RaiseCanExecuteChanged();
        }

        private void ExecuteRemove()
        {
            if (CanExecuteRemove())
            {
                _expression = _expression.Substring(0, _expression.Length - 1);
                Display = FormatDisplayContent(_expression);
                RaiseCanExecuteChanged();
            }
        }

        private string FormatDisplayContent(string expression)
        {
            expression = expression
                .Replace("^0", "⁰")
                .Replace("^1", "¹")
                .Replace("^2", "²")
                .Replace("^3", "³")
                .Replace("^4", "⁴")
                .Replace("^5", "⁵")
                .Replace("^6", "⁶")
                .Replace("^7", "⁷")
                .Replace("^8", "⁸")
                .Replace("^9", "⁹");

            return expression;
        }

        private void ApplySquareRoot()
        {
            if (string.IsNullOrWhiteSpace(_expression)) return;

            var match = Regex.Match(_expression, @"(\d+(\.\d+)?)(?!.*\d)");

            if (match.Success)
            {
                int startIndex = match.Index;
                int matchLength = match.Length;
                string matchVal = match.Value;

                _expression = _expression.Remove(startIndex, matchLength);
                _expression = _expression.Insert(startIndex, $"√({matchVal})");
            }
        }

        private bool CanExecuteRemove()
        {
            return !string.IsNullOrEmpty(_expression);
        }

        private bool CanExecuteConfirmation(object parameter)
        {
            return !string.IsNullOrEmpty(_expression);
        }

        public void SetScannedExpression(string expression)
        {
            _expression = expression;
            Display = FormatDisplayContent(_expression);
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged() => ((CalculatorCommand)ConfirmationCommand).RaiseCanExecuteChanged();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
