using System.Windows;
using System.Windows.Input;
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
    }
}