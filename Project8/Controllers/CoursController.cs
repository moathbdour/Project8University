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

    public class CoursController : Controller
    {
        private Project8Entities db = new Project8Entities();

        // GET: Cours
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.Major).Include(c => c.Cours1);
            ViewBag.x = "Cours";

            return View(courses.ToList());
        }
        [HttpPost]
        public ActionResult Index(string search5)
        {
            if (search5 != null)
            {
                var abumahmood = db.Courses.Where(x => x.Course_Name.Contains(search5)).ToList();
                return View(abumahmood);
            }

            return View(db.Courses.ToList());
        }

        // GET: Cours/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            ViewBag.x = "Cours";

            return View(cours);
        }

        // GET: Cours/Create
        public ActionResult Create()
        {
            ViewBag.Major_Id = new SelectList(db.Majors, "Major_Id", "Major_Name");
            ViewBag.dependent_Course = new SelectList(db.Courses, "Course_Id", "Course_Name");
            ViewBag.x = "Cours";

            return View();
        }

        // POST: Cours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Course_Id,Course_Name,Number_Of_Hours,Course_Description,Major_Id,syllabus,dependent_Course")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(cours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Major_Id = new SelectList(db.Majors, "Major_Id", "Major_Name", cours.Major_Id);
            ViewBag.dependent_Course = new SelectList(db.Courses, "Course_Id", "Course_Name", cours.dependent_Course);
            return View(cours);
        }

        // GET: Cours/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            ViewBag.Major_Id = new SelectList(db.Majors, "Major_Id", "Major_Name", cours.Major_Id);
            ViewBag.dependent_Course = new SelectList(db.Courses, "Course_Id", "Course_Name", cours.dependent_Course);
            ViewBag.x = "Cours";

            return View(cours);
        }

        // POST: Cours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Course_Id,Course_Name,Number_Of_Hours,Course_Description,Major_Id,syllabus,dependent_Course")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Major_Id = new SelectList(db.Majors, "Major_Id", "Major_Name", cours.Major_Id);
            ViewBag.dependent_Course = new SelectList(db.Courses, "Course_Id", "Course_Name", cours.dependent_Course);
            return View(cours);
        }

        // GET: Cours/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            ViewBag.x = "Cours";

            return View(cours);
        }

        // POST: Cours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cours cours = db.Courses.Find(id);
            db.Courses.Remove(cours);
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
