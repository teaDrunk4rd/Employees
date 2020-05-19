using System.Windows.Input;
using DevExpress.Mvvm;

namespace Employees.ViewModels.Classes
{
    public class ModalViewModel : ViewModelBase
    {
        public ICommand ApplyCommand;
        
        public ICommand AbortCommand;
    }
}