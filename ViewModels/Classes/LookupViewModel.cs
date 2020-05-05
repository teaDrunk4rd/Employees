using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.Classes;

namespace Employees.ViewModels.Classes
{
    public class LookupViewModel : ViewModelBase
    {
        private WindowMode _mode;

        public WindowMode Mode
        {
            get => _mode;
            set
            {
                if (Equals(_mode, value)) return;
                _mode = value;
                RaisePropertyChanged(nameof(Mode));
            }
        }

        public ICommand OnUpdateCollection { get; set; }

        public ICommand OnSelection { get; set; }
    }
}