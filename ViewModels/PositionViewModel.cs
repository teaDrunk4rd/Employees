using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using System.Windows.Input;
using DataModels;
using Employees.Classes;
using Employees.Models;
using Employees.ViewModels.Classes;
using LinqToDB;

namespace Employees.ViewModels
{
    class PositionViewModel : LookupViewModel
    {
        private Position _selectedPosition;
        private Position _position;
        private ObservableCollection<Position> _positions
            = new ObservableCollection<Position>(DBModel.PositionsTable.ToList().OrderBy(d => d.Name));

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

        public Position Position
        {
            get => _position;
            set
            {
                if (Equals(_position, value)) return;
                _position = value;
                RaisePropertyChanged(nameof(Position));
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
            Position = new Position();
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Edit;
            Position = (Position) SelectedPosition.Clone();
        }, 
        () => Mode == WindowMode.Read && SelectedPosition != default);

        public ICommand AddCommand => new DelegateCommand(() =>
        {
            DBModel.Context.Insert(Position);
            ClearWithUpdate();
            SelectedPosition = Positions.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2);
        }, () => CanExecuteUpsertCommand(Position));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            SelectedPosition = (Position) Position.Clone();
            DBModel.EmployeesDB.Update(SelectedPosition);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(SelectedPosition));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Delete(SelectedPosition);
            Positions.Remove(SelectedPosition);
            SelectedPosition = default;
            Mode = WindowMode.Read;
            OnUpdateCollection?.Execute(null);
        }, () =>  Mode == WindowMode.Read && SelectedPosition != default);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        private void Clear()
        {
            Position = default;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            var selectedId = SelectedPosition?.Id;
            Positions = new ObservableCollection<Position>(DBModel.PositionsTable.ToList().OrderBy(d => d.Name));
            if (selectedId != default)
                SelectedPosition = Positions.FirstOrDefault(d => d.Id == selectedId);
            OnUpdateCollection?.Execute(null);
        }

        private bool CanExecuteUpsertCommand(Position position)
            => position != null && !position.Name.IsEmpty() &&
               !Positions.Any(d => d.Name == position.Name && d.Id != position.Id);
    }
}
