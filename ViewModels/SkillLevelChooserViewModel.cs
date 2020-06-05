using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.ViewModels.Classes;

namespace Employees.ViewModels
{
    public class SkillLevelChooserViewModel : ModalViewModel
    {
        private short _level;

        public string SkillName { get; set; }

        public short Level
        {
            get => _level;
            set
            {
                if (Equals(_level, value)) return;
                _level = value;
                RaisePropertyChanged(nameof(Level));
            }
        }

        public ICommand CancelCommand =>
            new DelegateCommand(() => AbortCommand?.Execute(null));

        public ICommand OkCommand =>
            new DelegateCommand(() => ApplyCommand?.Execute(null),
                () => Level > 0 && Level <= 5);
    }
}