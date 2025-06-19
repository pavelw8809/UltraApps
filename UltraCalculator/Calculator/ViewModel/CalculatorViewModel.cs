using Calculator.Commands;
using Calculator.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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
            get => _expression;
            set { 
                _expression = value; 
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

        public CalculatorViewModel()
        {
            NumberCommand = new CalculatorCommand(ExecuteNumber);
            OperationCommand = new CalculatorCommand(ExecuteOperation);
            ConfirmationCommand = new CalculatorCommand(ExecuteConfirmation, CanExecuteConfirmation);
            ClearCommand = new CalculatorCommand(ExecuteClear);
        }

        private void ExecuteNumber(object parameter)
        {
            _expression += parameter.ToString();
            Display = _expression;
            RaiseCanExecuteChanged();
        }

        private void ExecuteOperation(object parameter)
        {
            string oper = parameter.ToString();

            if (string.IsNullOrEmpty(oper))
            {
                return;
            }

            if ("+-x:".Contains(_expression.Last()))
            {
                return;
            }

            _expression += oper;
            Display = _expression;
        }

        private void ExecuteConfirmation(object parameter)
        {
            try
            {
                //string fixedExpression = _expression.Replace("x", "*").Replace(":", "/");
                //var result = new System.Data.DataTable().Compute(fixedExpression, null);
                _lastExpression = _expression;
                _expression = _calculator.ExecuteExpression(_expression);
                LastExpression = _lastExpression;
                Display = _expression;
            }
            catch (Exception ex)
            {
                _expression = "";
                Display = _expression;
                LastExpression = $"Error: {ex.Message}";
            }
        }

        private void ExecuteClear(object parameter)
        {
            Display = "0";
            _expression = "";
            _lastExpression = "";
            RaiseCanExecuteChanged();
        }

        private bool CanExecuteConfirmation(object parameter)
        {
            return !string.IsNullOrEmpty(_expression);
        }

        private void RaiseCanExecuteChanged() => ((CalculatorCommand)ConfirmationCommand).RaiseCanExecuteChanged();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
