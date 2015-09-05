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
using System.Configuration;

namespace DigiDou.Web.Controllers
{
    public class BaseController : Controller
    {

        public void Success(string msg)
        {
            TempData["success"] = msg;
        }


        public void Error(string msg)
        {
            TempData["error"] = msg;
        }
        protected ApplicationUser CurrentUser { get; set; }
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
         //   if (Request.IsAuthenticated)
            {
                //CurrentUser= db.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                CurrentUser = db.Users.FirstOrDefault();

            }
            return base.BeginExecuteCore(callback, state);
        }

    }
    public class SMSMvcController : BaseController
    {
        
        // GET: SMSMvc
        public ActionResult Index()
        {
            return View(db.Messages.Where(x => x.User.Id == CurrentUser.Id).ToList());
        }

        // GET: SMSMvc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMS sMS = CurrentUser.Messages.Find(m => m.Id == id);
            if (sMS == null)
            {
                return HttpNotFound();
            }
            return View(sMS);
        }

        // GET: SMSMvc/Create
        public ActionResult Create(int? recipientId)
        {
            var model = new SMSCreateVM();
            model.MyContacts = new SelectList(CurrentUser.Contacts, "Id", "FullName", recipientId.GetValueOrDefault());
            
            return View(model);
        }

        // POST: SMSMvc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SMSCreateVM sMS)
        {
            if (ModelState.IsValid)
            {
                SMS newMessage = new SMS { Recipient = db.Contacts.Find(sMS.SelectContactId) , Body = sMS.Body, User = CurrentUser};
                db.Messages.Add(newMessage);
                db.SaveChanges();
                Success($"SMS ready to be sent to {newMessage.Recipient.FullName}");
                return RedirectToAction("Index");
            }

            sMS.MyContacts = new SelectList(CurrentUser.Contacts, "Id", "FullName", sMS.SelectContactId);
            db.SaveChanges();
            return View(sMS);
        }

        // GET: SMSMvc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMS sMS = db.Messages.Find(id);
            if (sMS == null)
            {
                return HttpNotFound();
            }
            return View(sMS);
        }

        // POST: SMSMvc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,Recipient")] SMS sMS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sMS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sMS);
        }

        // GET: SMSMvc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMS sMS = db.Messages.Find(id);
            if (sMS == null)
            {
                return HttpNotFound();
            }
            return View(sMS);
        }

        // POST: SMSMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SMS sMS = db.Messages.Find(id);
            db.Messages.Remove(sMS);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Send(string number, string body, int id)
        {
            SendText(number, body);
            var message = db.Messages.FirstOrDefault(m => m.Id == id);
            message.IsSent = true;
            db.SaveChanges();
            return Content("Ok");
        }

        //SendText method
        public static void SendText(string number, string body)
        {
            string AccountSid = ConfigurationManager.AppSettings["acctSid"];
            string AuthToken = ConfigurationManager.AppSettings["authToken"];
            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            var message = twilio.SendMessage(ConfigurationManager.AppSettings["twilioNumber"], number, body);
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
