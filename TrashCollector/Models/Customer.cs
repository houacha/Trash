using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TrashCollector.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Zip { get; set; }
        public string Address { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public double Balance { get; set; }
        [Display(Name = "Pick-up Day")]
        public string PickupDay { get; set; }
        [Display(Name = "Extra Pick-up Date")]
        public string ExtraPickupDate { get; set; }
        [Display(Name = "Suspended Start")]
        public string SuspendedStart { get; set; }
        [Display(Name = "Suspended End")]
        public string SuspendedEnd { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}