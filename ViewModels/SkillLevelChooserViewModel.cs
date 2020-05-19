using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.ViewModels.Classes;

namespace Employees.ViewModels
{
    public class SkillLevelChooserViewModel : ModalViewModel
    {
        public string SkillName { get; set; }

        public string Level { get; set; }

        public ICommand CancelCommand =>
            new DelegateCommand(() => AbortCommand?.Execute(null));

        public ICommand OkCommand =>
            new DelegateCommand(() => ApplyCommand?.Execute(null),
                () => int.TryParse(Level, out var level) && level > 0 && level <= 5);
    }
}