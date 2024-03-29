﻿using System;
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
    public class OrganizationsController : Controller
    {
        private GucchoEntities db = new GucchoEntities();

        // GET: Organizations
        public ActionResult Index()
        {
            var organizations = db.Organizations.Include(o => o.Admin);
            return View(organizations.ToList());
        }

        // GET: Organizations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);

            // security
            if(organization == null || organization.fk_userName != getCurrentlyLoggedInUser())
            {
                return HttpNotFound();
            }

            if (organization == null)
            {
                return HttpNotFound();
            }

            return View(organization);
        }

        // GET: Organizations/Create
        public ActionResult Create()
        {
            ViewBag.fk_userName = new SelectList(db.Admins, "username", "name");
            return View();
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "oID,name,address,phoneNumber,email")] Organization organization)
        {
            organization.fk_userName = getCurrentlyLoggedInUser();

            if (ModelState.IsValid)
            {
                db.Organizations.Add(organization);
                db.SaveChanges();
                HttpCookie access = Request.Cookies["access"];
                string role = access != null ? access.Value.Split('=')[1] : "undefined";

                if (!role.Equals("undefined"))
                {
                    if (role.Equals("admin"))
                    {
                        return RedirectToAction("SignIn", "Admins");
                    }
                    else
                    {
                        return RedirectToAction("SignIn", "Students");
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }

            ViewBag.fk_userName = new SelectList(db.Admins, "username", "name", organization.fk_userName);
            return View(organization);
        }

        // GET: Organizations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            //security
            if (organization == null || organization.fk_userName != getCurrentlyLoggedInUser())
            {
                return HttpNotFound();
            }

            if (organization == null)
            {
                return HttpNotFound();
            }
            SelectList selectListItems = new SelectList(db.Admins, "username", "name", organization.fk_userName);
            
            ViewBag.fk_userName = selectListItems;
            return View(organization);
        }

        // POST: Organizations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "oID,name,address,phoneNumber,email")] Organization organization)
        {
            organization.fk_userName = getCurrentlyLoggedInUser();
            if (ModelState.IsValid)
            {
                db.Entry(organization).State = EntityState.Modified;
                db.SaveChanges();
                HttpCookie access = Request.Cookies["access"];
                string role = access != null ? access.Value.Split('=')[1] : "undefined";

                if (!role.Equals("undefined"))
                {
                    if (role.Equals("admin"))
                    {
                        return RedirectToAction("SignIn", "Admins");
                    }
                    else
                    {
                        return RedirectToAction("SignIn", "Students");
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }
            ViewBag.fk_userName = new SelectList(db.Admins, "username", "name", organization.fk_userName);
            return View(organization);
        }

        // GET: Organizations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);

            // security
            if (organization == null || organization.fk_userName != getCurrentlyLoggedInUser())
            {
                return HttpNotFound();
            }

            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Organization organization = db.Organizations.Find(id);
            db.Organizations.Remove(organization);
            db.SaveChanges();
            HttpCookie access = Request.Cookies["access"];
            string role = access != null ? access.Value.Split('=')[1] : "undefined";

            if (!role.Equals("undefined"))
            {
                if (role.Equals("admin"))
                {
                    return RedirectToAction("SignIn", "Admins");
                }
                else
                {
                    return RedirectToAction("SignIn", "Students");
                }
            }
            else
            {
                return HttpNotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //utility here
        public String getCurrentlyLoggedInUser()
        {
            // returns the username of currently logged in user

            HttpCookie username = Request.Cookies["name"];
            string name = username != null ? username.Value.Split('=')[1] : "undefined";
            return name;
        }

    }
}
