﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DigiDou.Web.Models;
using Twilio;

namespace DigiDou.Web.Controllers
{
    public class ContactsMvcController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContactsMvc
        public ActionResult Index()
        {
            //var currentUser = db.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            //Temporarily using seeded user for testing
            var currentUser = db.Users.FirstOrDefault();
            return View(db.Contacts.Where(x => x.User.Id == currentUser.Id).ToList());
        }

       // GET: DueDateCountdown
       public ActionResult Countdown()
        {
            var dayCount = db.Users.FirstOrDefault().DaysTilDue;
            return View((object)dayCount);

        }

        // GET: ContactsMvc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: ContactsMvc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactsMvc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Phone")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                //var currentUser = db.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                //Temporarily using seeded user for testing
                var currentUser = db.Users.FirstOrDefault();
                contact.User = currentUser;
                db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: ContactsMvc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: ContactsMvc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Phone")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: ContactsMvc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: ContactsMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
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
