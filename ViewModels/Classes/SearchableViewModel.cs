using DevExpress.Mvvm;
using Employees.Classes;

namespace Employees.ViewModels.Classes
{
    public abstract class SearchableViewModel : ViewModelBase
    {
        private string _search;
        
        public string Search
        {
            get => _search;
            set
            {
                if (Equals(_search, value)) return;
                _search = value;
                RaisePropertyChanged(nameof(Search));
                RaiseSearchChanged();
            }
        }

        protected abstract void RaiseSearchChanged();
    }
}