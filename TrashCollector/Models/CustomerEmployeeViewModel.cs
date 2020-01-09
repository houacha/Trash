using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrashCollector.Models
{
    public class CustomerEmployeeViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<Employee> Employees { get; set; }
    }
}