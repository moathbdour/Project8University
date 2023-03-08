using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Project8.Models;

namespace Project8.Controllers
{
    [Authorize(Roles = "Admin")]

    public class DoctorsController : Controller
    {
        private Project8Entities db = new Project8Entities();

        // GET: Doctors
        public ActionResult Index()
        {
            var doctors = db.Doctors.Include(d => d.AspNetUser);
            ViewBag.x = "Doctors";

            return View(doctors.ToList());
        }
        [HttpPost]
        public ActionResult Index(string search5)
        {
            if (search5 != null)
            {
                var abumahmood = db.Doctors.Where(x => x.Doctor_Name.Contains(search5)).ToList();
                return View(abumahmood);
            }

            return View(db.Doctors.ToList());
        }


        // GET: Doctors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            ViewBag.x = "Doctors";

            return View(doctor);
        }

        // GET: Doctors/Create
        public ActionResult Create()
        {
            ViewBag.User_ID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.x = "Doctors";

            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //async public Task<ActionResult> Create([Bind(Include = "Doctor_Id,Doctor_Name,Doctor_Image,Doctor_Phone,Doctor_Email")] Doctor doctor, HttpPostedFileBase Doctor_Image)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Guid guid1 = Guid.NewGuid();

        //        doctor.Doctor_Image = guid1 + "-" + Doctor_Image.FileName;
        //        string doctorimg = guid1 + "-" + Doctor_Image.FileName;
        //        Doctor_Image.SaveAs(Server.MapPath("../Images/" + doctorimg));

        //        db.Doctors.Add(doctor);
        //        db.SaveChanges();
        //        await CreateDoctor(Request["Doctor_Email"], Request["Doctor_Password"]);
        //        string email = Request["Doctor_Email"];

        //        var x = db.AspNetUsers.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefault();
        //        doctor.User_ID = x;
        //        AspNetUserRole role= new AspNetUserRole();
        //        role.UserId = x;
        //        role.RoleId = "4";
        //        db.AspNetUserRoles.Add(role);
        //        db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.User_ID = new SelectList(db.AspNetUsers, "Id", "Email", doctor.User_ID);
        //    return View(doctor);
        //}
        async public Task<ActionResult> Create([Bind(Include = "Doctor_Id,Doctor_Name,Doctor_Image,Doctor_Phone,Doctor_Email")] Doctor doctor, HttpPostedFileBase Doctor_Image)
        {
            string email = Request["Doctor_Email"];
            var x = db.AspNetUsers.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefault();
            if (x == null)
            {
                if (ModelState.IsValid)
                {
                    Guid guid1 = Guid.NewGuid();

                    doctor.Doctor_Image = guid1 + "-" + Doctor_Image.FileName;
                    string doctorimg = guid1 + "-" + Doctor_Image.FileName;
                    Doctor_Image.SaveAs(Server.MapPath("../Images/" + doctorimg));

                    db.Doctors.Add(doctor);
                    db.SaveChanges();
                    await CreateDoctor(Request["Doctor_Email"], Request["Doctor_Password"]);
                    x = db.AspNetUsers.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefault();

                    doctor.User_ID = x;
                    AspNetUserRole doctorRole = new AspNetUserRole();
                    doctorRole.RoleId = "4";
                    doctorRole.UserId = x;
                    db.AspNetUserRoles.Add(doctorRole);
                    db.SaveChanges();
                    ViewBag.x = "Doctors";

                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["swal_message"] = $"Doctor Email already exist ";
                ViewBag.title = "Error";
                ViewBag.icon = "error";
                ViewBag.x = "Doctors";

                return View();
            }
            ViewBag.User_ID = new SelectList(db.AspNetUsers, "Id", "Email", doctor.User_ID);
            ViewBag.x = "Doctors";

            return View(doctor);

        }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        // If we got this far, something failed, redisplay form
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDoctor(string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Email, Email = Email };
                var result = await UserManager.CreateAsync(user, Password);
                return RedirectToAction("Index", "Doctors");

            }
            return View();

            // If we got this far, something failed, redisplay form
        }

        // GET: Doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_ID = new SelectList(db.AspNetUsers, "Id", "Email", doctor.User_ID);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id,[Bind(Include = "Doctor_Id,Doctor_Name,Doctor_Image,Doctor_Phone,Doctor_Email,User_ID")] Doctor doctor, HttpPostedFileBase Doctor_Image)
        {

            if (ModelState.IsValid)
            {
                var existingModel = db.Doctors.AsNoTracking().FirstOrDefault(x => x.Doctor_Id == id);

                if (Doctor_Image != null)
                {
                    Guid guid1 = Guid.NewGuid();

                    doctor.Doctor_Image = guid1 + "-" + Doctor_Image.FileName;
                    string collegeImage = guid1 + "-" + Doctor_Image.FileName;
                    Doctor_Image.SaveAs(Server.MapPath("../../Images/" + collegeImage));
                }
                else
                {
                    doctor.Doctor_Image = existingModel.Doctor_Image;
                }
                db.Entry(doctor).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_ID = new SelectList(db.AspNetUsers, "Id", "Email", doctor.User_ID);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            ViewBag.x = "Doctors";

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
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
