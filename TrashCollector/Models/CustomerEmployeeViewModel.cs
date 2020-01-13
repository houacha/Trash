using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace TrashCollector.Models
{
    public class CustomerEmployeeViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<Employee> Employees { get; set; }
        [Display(Name = "Current Position")]
        public List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> Roles { get; set; }
    }
}