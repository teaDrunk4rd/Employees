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

        public override ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Position = new Position();
        }, () =>  Mode == WindowMode.Read);

        public override ICommand ShowEditForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Edit;
            Position = (Position) SelectedPosition.Clone();
        }, 
        () => Mode == WindowMode.Read && SelectedPosition != null);

        public override ICommand AddCommand => new DelegateCommand(() =>
        {
            var id = DBModel.Context.InsertWithInt64Identity(Position);
            ClearWithUpdate();
            SelectedPosition = FilteredPositions.First(d => d.Id == id);
            OnSelection?.Execute(this);
        }, () => CanExecuteUpsertCommand(Position));

        public override ICommand EditCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Update(Position);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(Position));

        public override ICommand DeleteCommand => new DelegateCommand(() =>
        {
            if (Extensions.ShowConfirmationModal() != MessageBoxResult.Yes) 
                return;
            DBModel.EmployeesDB.Delete(SelectedPosition);
            Positions.Remove(SelectedPosition);
            FilteredPositions.Remove(SelectedPosition);
            SelectedPosition = null;
            Clear();
            OnUpdateCollection?.Execute(null);
        }, () =>  Mode == WindowMode.Read && SelectedPosition != null);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        private void Clear()
        {
            Position = null;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            Positions = DBModel.PositionsTable.ToList();
            UpdateCollection();
            OnUpdateCollection?.Execute(null);
        }

        public override void UpdateCollection()
        {
            var selectedId = SelectedPosition?.Id;
            FilteredPositions = 
                new ObservableCollection<Position>(Positions.Where(d => d.Search(Search)).OrderBy(d => d.Name));
            if (selectedId != null)
                SelectedPosition = FilteredPositions.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Position position)
            => position != null && !position.Name.IsEmpty() &&
               !Positions.Any(d => d.Name == position.Name && d.Id != position.Id);
    }
}
