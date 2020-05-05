using DevExpress.Mvvm;
using Employees.Classes;

namespace Employees.ViewModels.Classes
{
    public abstract class SearchableViewModel : ViewModelBase
    {
        private WindowMode _mode;
        private string _search;

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