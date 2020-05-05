using System.Windows.Input;

namespace Employees.ViewModels.Classes
{
    public abstract class LookupViewModel : SearchableViewModel
    {
        public ICommand OnUpdateCollection { get; set; }

        public ICommand OnSelection { get; set; }
    }
}