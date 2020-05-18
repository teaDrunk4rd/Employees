using System.Windows;
using System.Windows.Input;
using Employees.Classes;

namespace Employees.ViewModels.Classes
{
    public abstract class LookupViewModel : SearchableViewModel
    {
        private WindowMode _mode;
        
        public WindowMode Mode
        {
            get => _mode;
            set
            {
                if (Equals(_mode, value)) return;
                _mode = value;
                RaisePropertiesChanged(nameof(Mode), nameof(OkCommand), nameof(FormName), nameof(FormVisibility));
            }
        }
        
        public abstract ICommand AddCommand { get; }
        
        public abstract ICommand EditCommand { get; }
        
        public abstract ICommand DeleteCommand { get; }
        
        public ICommand OnUpdateCollection { get; set; }

        public ICommand OnSelection { get; set; }

        // TODO: <fix this>
        public string FormName => Mode == WindowMode.Add ? "Добавление" : "Редактирование";
        
        public ICommand OkCommand => Mode == WindowMode.Add ? AddCommand : EditCommand;

        public Visibility FormVisibility => Mode == WindowMode.Read ? Visibility.Collapsed : Visibility.Visible;
        // </fix this>
    }
}