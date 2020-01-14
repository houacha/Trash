using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrashCollector.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required]
        public string Address { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
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
        [Display(Name = "Confirm Pick-ups")]
        public bool PickupConfirmed { get; set; }
        [HiddenInput]
        public string Lat { get; set; }
        [HiddenInput]
        public string Lng { get; set; }
        [NotMapped]
        [HiddenInput]
        public string CanSee { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}