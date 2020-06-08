using Employees.Classes;
using Employees.Models;

namespace DataModels
{
    public partial class Project : ISearchable
    {
        public object Clone() => MemberwiseClone();

        public bool Search(string search) => 
            search.IsEmpty() || Name.Search(search) || StartDate.Search(search) || FinishDate.Search(search);
    }
}