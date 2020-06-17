using System;
using System.Collections.ObjectModel;

namespace Employees.ViewModels.Classes
{
    public abstract class FilteredLookupViewModel<T> : LookupViewModel
    {
        private Func<ObservableCollection<T>, ObservableCollection<T>> _filter;
        
        public Func<ObservableCollection<T>, ObservableCollection<T>> Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                RaisePropertyChanged(nameof(Filter));
                UpdateCollection();
            }
        }
        public void SetFilter(Func<ObservableCollection<T>, ObservableCollection<T>> func) => Filter = func;
        
        public void RemoveFilter() => Filter = null;
    }
}