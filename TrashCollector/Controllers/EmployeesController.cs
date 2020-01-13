using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrashCollector.Models;

namespace TrashCollector.Controllers
{
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employees
        public ActionResult Index()
        {
            CustomerEmployeeViewModel allUser = new CustomerEmployeeViewModel();
            allUser.Customers = db.Customers.Include(c=>c.ApplicationUser).Select(c => c).ToList();
            allUser.Employees = db.Employees.Include(e=>e.ApplicationUser).Select(e=>e).ToList();
            var users = db.Users.Select(u => u.Roles.FirstOrDefault()).ToList();
            List <Microsoft.AspNet.Identity.EntityFramework.IdentityRole> allRoles = new List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>();
            var roles = db.Roles.Select(r => r).ToList();
            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    if (user.RoleId == role.Id)
                    {
                        allRoles.Add(role);
                        break;
                    }
                }
            }
            allUser.Roles = allRoles;
            return View(allUser);
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            Employee employee;
            string currentId = User.Identity.GetUserId();
            var currentUser = db.Employees.Include(e=>e.ApplicationUser).Where(e => e.ApplicationId == currentId).Select(e => e).SingleOrDefault();
            if (User.IsInRole("Employee") == true)
            {
                employee = currentUser;
            }
            else
            {
                if (id == null)
                {
                    if (User.IsInRole("Manager") == true || User.IsInRole("Admin") == true)
                    {
                        employee = currentUser;
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    employee = db.Employees.Include(e => e.ApplicationUser).Where(e => e.Id == id).Select(e => e).SingleOrDefault();
                }
                if (employee == null)
                {
                    return HttpNotFound();
                }
            }
            ViewBag.Id = currentUser.Id;
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Zip,ApplicationId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var id =  User.Identity.GetUserId();
                employee.ApplicationId = id;
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("LogOut", "Account");
            }

            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email", employee.ApplicationId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = db.Employees.Include(e => e.ApplicationUser).Where(e => e.Id == id).Select(e => e).SingleOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email", employee.ApplicationId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, Employee employee)
        {
            if (ModelState.IsValid)
            {
                var person = db.Employees.Where(e => e.Id == id).Select(e => e).SingleOrDefault();
                person.FirstName = employee.FirstName;
                person.LastName = employee.LastName;
                person.Zip = employee.Zip;
                person.ApplicationUser.Email = employee.ApplicationUser.Email;
                db.SaveChanges();
                ViewBag.Id = person.Id;
                return View("Details", person);
            }
            ViewBag.ApplicationId = new SelectList(db.Users, "Id", "Email", employee.ApplicationId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = db.Employees.Include(e => e.ApplicationUser).Where(e => e.Id == id).Select(e => e).SingleOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            var user = db.Users.Where(u => u.Id == employee.ApplicationId).Select(u => u).SingleOrDefault();
            db.Users.Remove(user);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
