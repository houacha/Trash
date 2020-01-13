using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrashCollector.Models
{
    public class FilterViewModel
    {
        [Display(Name = "Filter Customers By Day")]
        public string Day { get; set; }
        public SelectList DayWeek { get; set; }
        public List<Customer> Customers { get; set; }
    }
}