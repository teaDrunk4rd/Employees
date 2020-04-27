using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using System.Windows.Input;
using DataModels;
using Employees.Models;

namespace Employees.ViewModels
{
    class PositionViewModel : ViewModelBase
    {
        private bool _editMode;

        public bool EditMode
        {
            get => _editMode;
            set
            {
                _editMode = value;
                RaisePropertyChanged(nameof(EditMode));
            }
        }

        public ObservableCollection<Position> Positions { get; } = new ObservableCollection<Position>(DBModel.PositionsTable.ToList());

        public ICommand AddCommand => new DelegateCommand(() => EditMode = !EditMode);
    }
}
