using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using System.Windows.Input;
using DataModels;
using Employees.Classes;
using Employees.Models;
using LinqToDB;

namespace Employees.ViewModels
{
    class PositionViewModel : ViewModelBase
    {
        private WindowMode _mode;
        private Position _selectedPosition;
        private Position _appendablePosition;
        private ObservableCollection<Position> _positions
            = new ObservableCollection<Position>(DBModel.PositionsTable.ToList().OrderBy(d => d.Name));

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

        public Position SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                if (Equals(_selectedPosition, value)) return;
                _selectedPosition = value;
                RaisePropertyChanged(nameof(SelectedPosition));
            }
        }

        public Position AppendablePosition
        {
            get => _appendablePosition;
            set
            {
                if (Equals(_appendablePosition, value)) return;
                _appendablePosition = value;
                RaisePropertyChanged(nameof(AppendablePosition));
            }
        }

        public ObservableCollection<Position> Positions
        {
            get => _positions;
            set
            {
                if (Equals(_positions, value)) return;
                _positions = value;
                RaisePropertyChanged(nameof(Positions));
            }
        }

        public ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            AppendablePosition = new Position();
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() => Mode = WindowMode.Edit, 
            () => Mode == WindowMode.Read && SelectedPosition != default);

        public ICommand AddCommand => new DelegateCommand(() =>
        {
            DBModel.Context.Insert(AppendablePosition);
            Clear();
            SelectedPosition = Positions.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2);
        }, () => CanExecuteUpsertCommand(AppendablePosition));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Update(SelectedPosition);
            Clear();
        }, () => CanExecuteUpsertCommand(SelectedPosition));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Delete(SelectedPosition);
            Clear();
            SelectedPosition = default;
        }, () =>  Mode == WindowMode.Read && SelectedPosition != default);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        private void Clear()
        {
            UpdatePositions();
            Mode = WindowMode.Read;
        }

        private void UpdatePositions()
        {
            var selectedId = SelectedPosition?.Id;
            Positions = new ObservableCollection<Position>(DBModel.PositionsTable.ToList().OrderBy(d => d.Name));
            if (selectedId != default)
                SelectedPosition = Positions.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Position position)
            => position != null && !position.Name.IsEmpty() &&
               !Positions.Any(d => d.Name == position.Name && d.Id != position.Id);
    }
}
