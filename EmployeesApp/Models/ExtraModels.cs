using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesApp.Models
{
    public class SearchModel
    {
        public string SearchValue { get; set; }

        public SearchModel() { }

        public SearchModel(string sVal)
        {
            SearchValue = sVal;
        }
    }
}
