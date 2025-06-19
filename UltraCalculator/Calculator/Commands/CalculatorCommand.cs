using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.Commands
{
    public class CalculatorCommand : ICommand
    {
        private readonly Action<object> _exec;
        private readonly Func<object, bool> _canExec;

        public event EventHandler CanExecuteChanged;

        public CalculatorCommand(Action<object> exec, Func<object, bool> canExec = null)
        {
            _exec = exec ?? throw new ArgumentNullException(nameof(exec));
            _canExec = canExec;
        }

        public bool CanExecute(object parameter) => _canExec == null || _canExec(parameter);

        public void Execute(object parameter) => _exec(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
