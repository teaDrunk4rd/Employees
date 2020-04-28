using System.ComponentModel;

namespace Employees.Classes
{
    public enum WindowMode
    {
        [Description("")] 
        Read,
        [Description("Добавление")] 
        Add,
        [Description("Редактирование")] 
        Edit
    }
}