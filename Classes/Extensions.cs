using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.ViewModels.Classes;

namespace Employees.Classes
{
    public static class Extensions
    {
        public static bool IsEmpty(this string source) => source == string.Empty || source == default;

        public static bool Search(this DateTime sourceDateTime, string stringDateTime)
        {
            DateTime.TryParse(stringDateTime, out var dateTime);
            return sourceDateTime.ToString("dd.MM.yyyy").Contains(dateTime.ToString("dd.MM.yyyy"))
                || sourceDateTime.ToString("dd.MM.yyyy").Contains(stringDateTime);
        }

        public static bool Search(this string source, string word)
            => source.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0;
        
        public static void OpenWindow<VM, W>(this VM viewModel, ICommand selectionCommand=default, ICommand onOpenCommand=default) // TODO: Исправить, нарушает MVVM
            where VM : LookupViewModel
            where W : Window, new()
        {
            viewModel.Mode = WindowMode.Read;
            viewModel.OnSelection = selectionCommand;
            var view = new W {DataContext = viewModel};
            view.Show();
            onOpenCommand?.Execute(viewModel);
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