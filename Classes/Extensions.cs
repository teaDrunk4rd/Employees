using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.ViewModels.Classes;

namespace Employees.Classes
{
    public static class Extensions
    {
        public static bool IsEmpty(this string source) => source == string.Empty || source == default;
        
        public static void OpenWindow<VM, W>(this VM viewModel, ICommand selectionCommand=default) // TODO: Исправить, нарушает MVVM
            where VM : LookupViewModel
            where W : Window, new()
        {
            viewModel.Mode = WindowMode.Read;
            viewModel.OnSelection = selectionCommand;
            var view = new W {DataContext = viewModel};
            view.Show();
        }

        public static void CloseWindow(this ViewModelBase viewModel) // TODO: Исправить, нарушает MVVM
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == viewModel) window.Close();
            }
        }
    }
}