using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project8.Models;

namespace Project8.Controllers
{
    [Authorize(Roles = "Admin")]

    public class semestersController : Controller
    {
        private Project8Entities db = new Project8Entities();

        // GET: semesters
        public ActionResult Index()
        {
            ViewBag.x = "semesters";

            return View(db.semesters.ToList());
        }
        [HttpPost]
        public ActionResult Index(string search5)
        {
            if (search5 != null)
            {
                var abumahmood = db.semesters.Where(x => x.name.Contains(search5)).ToList();
                return View(abumahmood);
            }

            return View(db.semesters.ToList());
        }
        // GET: semesters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            semester semester = db.semesters.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            ViewBag.x = "semesters";

            return View(semester);
        }

        // GET: semesters/Create
        public ActionResult Create()
        {
            ViewBag.x = "semesters";

            return View();
        }

        // POST: semesters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,start_date,end_date")] semester semester)
        {
            if (ModelState.IsValid)
            {
                db.semesters.Add(semester);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(semester);
        }

        // GET: semesters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            semester semester = db.semesters.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            ViewBag.x = "semesters";

            return View(semester);
        }

        // POST: semesters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,start_date,end_date")] semester semester)
        {
            if (ModelState.IsValid)
            {
                db.Entry(semester).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(semester);
        }

        // GET: semesters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            semester semester = db.semesters.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            ViewBag.x = "semesters";

            return View(semester);
        }

        // POST: semesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            semester semester = db.semesters.Find(id);
            db.semesters.Remove(semester);
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
