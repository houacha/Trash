using Google.Maps;
using Google.Maps.Geocoding;
using Google.Maps.StaticMaps;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;
using TrashCollector.Models;

namespace TrashCollector.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers
        public ActionResult Index()
        {
            FilterViewModel model = new FilterViewModel();
            model.DayWeek = new SelectList(new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" });
            model.Customers = db.Customers.Include(e => e.ApplicationUser).ToList();
            return View(model);
        }
        public ActionResult Balance()
        {
            var Id = User.Identity.GetUserId();
            var person = db.Customers.Include(c => c.ApplicationUser).Where(c => c.ApplicationId == Id).Select(c => c).SingleOrDefault();
            return View(person);
        }
        public FilterViewModel PeopleFilter(string day)
        {
            FilterViewModel model = new FilterViewModel();
            model.DayWeek = new SelectList(new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" });
            var Id = User.Identity.GetUserId();
            var user = db.Employees.Where(e => e.ApplicationId == Id).Select(e => e).SingleOrDefault();
            model.Day = day;
            model.Customers = db.Customers.Include(c => c.ApplicationUser).Where(c => c.PickupDay == day && c.Zip == user.Zip).Select(c => c).ToList(); // add extra pickup date
            return model;
        }
        public string Map(List<Customer> customers)
        {
            if (customers != null)
            {
                if (customers.Count > 0)
                {
                    List<string> addresses = new List<string>();
                    string markers = "";
                    var zip = customers[0].Zip;
                    foreach (var item in customers)
                    {
                        addresses.Add(item.Address);
                    }
                    foreach (var location in addresses)
                    {
                        string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(location), "AIzaSyCGKMApsiQJT - YV_KWIV63HXtTMITjhPiw");
                        WebRequest request = WebRequest.Create(requestUri);
                        WebResponse response = request.GetResponse();
                        XDocument xdoc = XDocument.Load(response.GetResponseStream());
                        XElement result = xdoc.Element("GeocodeResponse").Element("result");

                        if (result != null)
                        {
                            XElement locationElement = result.Element("geometry").Element("location");
                            XElement lat = locationElement.Element("lat");
                            XElement lng = locationElement.Element("lng");
                            var x = lat.Value;
                            var y = lng.Value;
                            markers += "markers=color:red%7Clabel:C%7C" + x + "," + y + "&";
                        }
                    }
                    string mapUri = string.Format("https://maps.googleapis.com/maps/api/staticmap?center={0}&zoom=13&size=600x300&maptype=roadmap&{2}key={1}", Uri.EscapeDataString(zip), "AIzaSyCGKMApsiQJT - YV_KWIV63HXtTMITjhPiw", markers);
                    return mapUri;
                }
            }
            return "";
        }

        public ActionResult Filter()
        {
            var today = Convert.ToString(DateTime.Now.DayOfWeek);
            var model = PeopleFilter(today);
            var mapUri = Map(model.Customers);
            ViewBag.StaticMapUri = mapUri;
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(FilterViewModel model)
        {
            var filterModel = PeopleFilter(model.Day);
            var mapUri = Map(filterModel.Customers);
            ViewBag.StaticMapUri = mapUri;
            return View("Index", filterModel);
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id, Messages? messages, string value)
        {
            Customer customer;
            List<Customer> customers = new List<Customer>();
            ViewBag.Message =
                messages == Messages.RegisterPickup ? "Your order was placed! Thank you for choosing 'Super Garbo-Men' to be your designated trash collectors."
                : messages == Messages.RegisterOtherPickup ? "Your order was placed! We will come pick-up on those desire days."
                : "";
            if (User.IsInRole("Customer") == true)
            {
                var currentId = User.Identity.GetUserId();
                customer = db.Customers.Include(c => c.ApplicationUser).Where(e => e.ApplicationId == currentId).Select(e => e).SingleOrDefault();
                customer.CanSee = value;
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                customer = db.Customers.Include(c => c.ApplicationUser).Where(c => c.Id == id).SingleOrDefault();
                if (customer == null)
                {
                    return HttpNotFound();
                }
            }
            customers.Add(customer);
            var mapUri = Map(customers);
            ViewBag.StaticMapUri = mapUri;
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Zip,Address,FirstName,LastName,State,City,Balance,PickupDay,ExtraPickupDate,SuspendedStart,SuspendedEnd,ApplicationId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var Id = User.Identity.GetUserId();
                customer.ApplicationId = Id;
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("LogOut", "Account");
            }
            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email", customer.ApplicationId);
            return View(customer);
        }

        public ActionResult Register()
        {
            var Id = User.Identity.GetUserId();
            var person = db.Customers.Where(c => c.ApplicationId == Id).Select(c => c).SingleOrDefault();
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer customer)
        {
            try
            {
                var personId = User.Identity.GetUserId();
                var personToEdit = db.Customers.Where(c => c.ApplicationId == personId).Select(c => c).SingleOrDefault();
                var tempHolder = personToEdit.PickupDay;
                personToEdit.PickupDay = customer.PickupDay;
                personToEdit.ExtraPickupDate = customer.ExtraPickupDate;
                personToEdit.SuspendedStart = customer.SuspendedStart;
                personToEdit.SuspendedEnd = customer.SuspendedEnd;
                if (customer.PickupDay != null && customer.PickupDay != tempHolder)
                {
                    db.SaveChanges();
                    return RedirectToAction("Details", new { value = "false", message = Messages.RegisterPickup });
                }
                else
                {
                    personToEdit.PickupDay = tempHolder;
                }
                if (customer.ExtraPickupDate != null || (customer.SuspendedEnd != null && customer.SuspendedStart != null))
                {
                    db.SaveChanges();
                    return RedirectToAction("Details", new { value = "false", message = Messages.RegisterOtherPickup });
                }
                return RedirectToAction("Details", new { value = "false" });
            }
            catch (Exception)
            {
                return View(customer);
            }
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Include(c => c.ApplicationUser).Where(c => c.Id == id).Select(c => c).SingleOrDefault();
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email", customer.ApplicationId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, Customer customer)
        {
            if (ModelState.IsValid)
            {
                var person = db.Customers.Where(c => c.Id == id).Select(c => c).SingleOrDefault();
                person.FirstName = customer.FirstName;
                person.LastName = customer.LastName;
                person.Address = customer.Address;
                person.City = customer.City;
                person.State = customer.State;
                person.Zip = customer.Zip;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email", customer.ApplicationId);
            return View(customer);
        }

        public ActionResult ConfirmPick(int? id)
        {
            var customer = db.Customers.Where(c => c.Id == id).Select(c => c).Single();
            if (customer.PickupConfirmed != true)
            {
                customer.PickupConfirmed = true;
                customer.Balance = 30;
            }
            db.SaveChanges();
            var model = PeopleFilter(customer.PickupDay);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult ConfirmPick(FilterViewModel model)
        {
            var viewModel = PeopleFilter(model.Day);
            return View("Index", viewModel);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            Customer customer;
            if (User.IsInRole("Customer") == true)
            {
                var Id = User.Identity.GetUserId();
                customer = db.Customers.Where(c => c.ApplicationId == Id).SingleOrDefault();
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                customer = db.Customers.Find(id);
            }
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            var user = db.Users.Where(u => u.Id == customer.ApplicationId).SingleOrDefault();
            db.Users.Remove(user);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return View("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Helpers
        public enum Messages
        {
            RegisterPickup,
            RegisterOtherPickup
        }
        #endregion
    }
}
