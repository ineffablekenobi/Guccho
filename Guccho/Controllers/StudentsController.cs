using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Guccho.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace Guccho.Controllers
{
    public class StudentsController : Controller
    {
        private GucchoEntities db = new GucchoEntities();


        // GET: Students
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fullName,password,phoneNumber,interests,email,sName")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fullName,password,phoneNumber,interests,email,sName")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
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
        public ActionResult SignIn([Bind(Include = "sName,password")] Student student)
        {
            Student result = db.Students.SingleOrDefault(user => user.sName == student.sName);
            if (result != null)
            {
                if (result.password.Equals(student.password))
                {

                    HttpCookie cookie = new HttpCookie("name");
                    cookie.Values["name"] = result.sName;

                    HttpCookie role = new HttpCookie("access");
                    role.Values["access"] = "student";

                    cookie.Expires = DateTime.Now.AddMinutes(10000);// update this later
                    role.Expires = DateTime.Now.AddMinutes(10000);
                    Response.Cookies.Add(cookie);
                    Response.Cookies.Add(role);
                    return RedirectToAction("Dashboard/" + result.sName);
                }

            }
            return View(result);
        }



        public ActionResult Dashboard(String id)
        {
            // many to many
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);

            if (!grantStudentAccess(student))
            {
                return HttpNotFound();
            }

            if (student == null)
            {
                return HttpNotFound();
            }


            string connStr = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            string sql = "SELECT * from Students";
            SqlCommand cmd = new SqlCommand(sql, conn);

            SqlDataReader reader = cmd.ExecuteReader();
            String str = "";
            while (reader.Read())
            {
                //This "for" iterates through each column of the current row!
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    str = str + reader.GetValue(i).ToString() + "\n";
                }
            }


            ViewBag.details = str;

            conn.Close();


            return View(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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

        public bool grantStudentAccess(Student student)
        {
            if (student == null)
            {
                return false;
            }
            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";

            if (name.Equals("undefined"))
            {
                return false;
            }

            HttpCookie access = Request.Cookies["access"];
            string role = access != null ? access.Value.Split('=')[1] : "undefined";

            if (!role.Equals("student") )
            {
                return false;
            }

            return true;

        }


    }
}
