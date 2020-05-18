using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using System.Windows.Input;
using DataModels;
using Employees.Classes;
using Employees.Models;
using Employees.ViewModels.Classes;
using LinqToDB;

namespace Employees.ViewModels
{
    public class PositionViewModel : LookupViewModel
    {
        private Position _selectedPosition;
        private Position _position;
        private List<Position> _positions;
        private ObservableCollection<Position> _filteredPositions;

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

        public List<Position> Positions
        {
            get => _positions;
            set
            {
                if (Equals(_positions, value)) return;
                _positions = value;
                RaisePropertyChanged(nameof(Positions));
            }
        }

        public ObservableCollection<Position> FilteredPositions
        {
            get => _filteredPositions;
            set
            {
                if (Equals(_filteredPositions, value)) return;
                _filteredPositions = value;
                RaisePropertyChanged(nameof(FilteredPositions));
            }
        }

        public PositionViewModel()
        {
            Positions = DBModel.PositionsTable.ToList();
            FilteredPositions =
                new ObservableCollection<Position>(Positions.Where(p => p.Search(Search)).OrderBy(p => p.Name));
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
            SelectedPosition = FilteredPositions.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2);
            OnSelection?.Execute(this);
        }, () => CanExecuteUpsertCommand(Position));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            SelectedPosition = (Position) Position.Clone();
            DBModel.EmployeesDB.Update(SelectedPosition);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(Position));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            if (Extensions.ShowConfirmationDialog() != MessageBoxResult.Yes) 
                return;
            DBModel.EmployeesDB.Delete(SelectedPosition);
            Positions.Remove(SelectedPosition);
            FilteredPositions.Remove(SelectedPosition);
            SelectedPosition = default;
            Clear();
            OnUpdateCollection?.Execute(null);
        }, () =>  Mode == WindowMode.Read && SelectedPosition != default);

        public ICommand ClearCommand => new DelegateCommand(() =>
        {
            Clear();
            if (OnSelection != default)
                OnSelection = default;
        });

        private void Clear()
        {
            Position = default;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            Positions = DBModel.PositionsTable.ToList();
            UpdatePositions();
            OnUpdateCollection?.Execute(null);
        }

        private void UpdatePositions()
        {
            var selectedId = SelectedPosition?.Id;
            FilteredPositions = 
                new ObservableCollection<Position>(Positions.Where(d => d.Search(Search)).OrderBy(d => d.Name));
            if (selectedId != default)
                SelectedPosition = FilteredPositions.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Position position)
            => position != null && !position.Name.IsEmpty() &&
               !Positions.Any(d => d.Name == position.Name && d.Id != position.Id);

        protected override void RaiseSearchChanged() => UpdatePositions();
    }
}
