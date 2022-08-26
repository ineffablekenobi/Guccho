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
    public class BranchesController : Controller
    {
        private GucchoEntities db = new GucchoEntities();

        // GET: Branches
        public ActionResult Index()
        {
            var branches = db.Branches.Include(b => b.Organization);
            return View(branches.ToList());
        }

        // GET: Branches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
        public ActionResult Create()
        {
            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";
            ViewBag.fk_oID = new SelectList(db.Organizations.Where(x => x.fk_userName.Equals(name)), "oID", "name");
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "phoneNumber,email,bID,name,address,fk_oID")] Branch branch)
        {
            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";
            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Dashboard","Admins",new { id = name});
            }
            
            ViewBag.fk_oID = new SelectList(db.Organizations.Where(x => x.fk_userName.Equals(name)), "oID", "name");
            return View(branch);
        }

        // GET: Branches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";
            ViewBag.fk_oID = new SelectList(db.Organizations.Where(x => x.fk_userName.Equals(name)), "oID", "name");

            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "phoneNumber,email,bID,name,address,fk_oID")] Branch branch)
        {
            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";

            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Admins", new { id = name });
            }
            
            ViewBag.fk_oID = new SelectList(db.Organizations.Where(x => x.fk_userName.Equals(name)), "oID", "name");
            return View(branch);
        }

        // GET: Branches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";

            Branch branch = db.Branches.Find(id);
            db.Branches.Remove(branch);
            db.SaveChanges();
            return RedirectToAction("Dashboard", "Admins", new { id = name });
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
