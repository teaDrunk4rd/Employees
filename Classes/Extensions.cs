namespace Employees.Classes
{
    public static class Extensions
    {
        public static bool IsEmpty(this string source) => source == string.Empty || source == default;
    }
}