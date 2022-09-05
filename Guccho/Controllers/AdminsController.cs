using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Guccho.Models;

namespace Guccho.Controllers
{
    public class AdminsController : Controller
    {
        private GucchoEntities db = new GucchoEntities();

        // GET: Admins
        public ActionResult Index()
        {
            return View(db.Admins.ToList());
        }

        // GET: Admins/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,username,phonenumber,password,Address")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("SignIn");
            }

            return View(admin);
        }

        // GET: Admins/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "name,username,phonenumber,password,Address")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SignIn");
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Admin admin = db.Admins.Find(id);
            db.Admins.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("SignIn");
        }

        public ActionResult SignIn()
        {

            if (isLoggedIn())
            {
                HttpCookie username = Request.Cookies["name"];
                string name = username != null ? username.Value.Split('=')[1] : "undefined";
                return RedirectToAction("Dashboard/" + name);
            }
            
            return View();
        }

        public ActionResult SignOut()
        {

            HttpCookie username = Request.Cookies["name"];
            username.Expires = DateTime.Now.AddDays(-1);
            HttpCookie role = Request.Cookies["access"];
            role.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(username);
            Response.Cookies.Add(role);

            return RedirectToAction("SignIn");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "username,password")] Admin admin)
        {
            Admin result = db.Admins.SingleOrDefault(user => user.username == admin.username);
            if(result != null)
            {
                if (result.password.Equals(admin.password))
                {
                    
                    HttpCookie cookie = new HttpCookie("name");
                    cookie.Values["name"] = result.username;

                    HttpCookie role = new HttpCookie("access");
                    role.Values["access"] = "admin";

                    cookie.Expires = DateTime.Now.AddMinutes(10000);// update this later
                    role.Expires = DateTime.Now.AddMinutes(10000);
                    Response.Cookies.Add(cookie);
                    Response.Cookies.Add(role);
                    return RedirectToAction("Dashboard/" + result.username);
                }
                
            }
            return View(result);
        }


        [OutputCache(Duration =0)]
        public ActionResult Dashboard(String id)
        {
            if (!isLoggedIn())
            {
                return HttpNotFound();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);

            if (!grantAdminAccess(admin))
            {
                return HttpNotFound();
            }
            
            var lst = db.Organizations.Where(val => val.fk_userName.Contains(admin.username)).ToList<Organization>();
            
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        // utility classes
        public bool grantAdminAccess(Admin admin)
        {
            if (admin == null)
            {
                return false;
            }
            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";

            if (name.Equals("undefined"))
            {
                return false;
            }

            if (!name.Equals(admin.username))
            {
                return false; 
            }

            HttpCookie access = Request.Cookies["access"];
            string role = access != null ? access.Value.Split('=')[1] : "undefined";

            if (!role.Equals("admin"))
            {
                return false;
            }

            

            return true;

        }

        public bool isLoggedIn()
        {
            HttpCookie access = Request.Cookies["access"];
            string role = access != null ? access.Value.Split('=')[1] : "undefined";

            if (role.Equals("undefined"))
            {
                return false;
            }

            return true;

        }


    }
}
