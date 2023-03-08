using Microsoft.AspNet.Identity;
using Project8.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Project8.Controllers
{
    public class HomeController : Controller
    {
        Project8Entities db = new Project8Entities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Change()
        {


            var semisteres = db.semesters;
            int current = 1;
            bool test = true;
            foreach (var semester in semisteres)
            {
                if (semester.end_date > DateTime.Now.Date && semester.start_date < DateTime.Now.Date)
                {
                    current = semester.id;
                    test = false;
                }
            }
            if (test)
            {
                ViewBag.message = "no";
                return View();
            }

            var dates1 = db.RegistrationPeriods.First(x => x.semester_id == current);
            if ( DateTime.Now.Date <= dates1.end_date)
            {
                ViewBag.message = "no";
                return View();

            }
            else
            {
                ViewBag.message = "yes";
            }



            return View();
        }

        [HttpPost]
        public ActionResult Change(string change)
        {
            if(change != null)
            {
                var semisteres = db.semesters;
                int current = 1;
              
                foreach (var semester in semisteres)
                {
                    if (semester.end_date > DateTime.Now.Date && semester.start_date < DateTime.Now.Date)
                    {
                        current = semester.id;
                       
                    }
                }
             
                var allstudent = db.AspNetUsers.Where(z => z.IsAccepted == true).ToList();
                foreach(var student in allstudent)
                {
                    int comulitave = 0;
                    var allenrollments =db.Enrollments.Where(x=>x.Student_id==student.Id && x.semester_id==current).ToList();
                    if ( allenrollments.Any()) { 
                    foreach(var enroll in allenrollments)
                    {
                            if (enroll.Is_Paid == false) { 
                            comulitave +=Convert.ToInt32( enroll.Courses_Offered.Cours.Number_Of_Hours);
                        enroll.Is_Paid= true;
                            }
                        }
                        if (comulitave == 0)
                        {
                            break;
                        }
                        var usermajor = db.Majors.Find(student.Major_Id);
                        int houreprice = Convert.ToInt32( usermajor.Price);
                        int amount = houreprice * comulitave;

                        Transaction newtran = new Transaction();
                        newtran.Transaction_Date = DateTime.Now;
                        newtran.UserId = student.Id;
                        newtran.Amount = Convert.ToInt32(amount);
                        newtran.User_Action = false;
                        db.Transactions.Add(newtran);
                        
                        db.SaveChanges();
                        
                    }
                    
                }
              








            }
            ViewBag.message = "yes";
            TempData["swal_message"] = $"All Enrollments where checked ";
            ViewBag.title = "success";
            ViewBag.icon = "success";
            return View();
        }

            public ActionResult Contact(string name,string subject,string email,string message)
        {
            ViewBag.Message = "Your contact page.";
            MailMessage mail = new MailMessage();
            mail.To.Add("projectnvc99@gmail.com");
            mail.From = new MailAddress("projectnvc99@gmail.com");
            mail.Subject = subject +" " + email;

            mail.Body = message;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Credentials = new System.Net.NetworkCredential("projectnvc99", "cbhcmnlhnosyinag");
            smtp.Send(mail);
            return View();
        }
        public ActionResult Collages()
        {
            var collages = db.Colleges.ToList();
            return View(collages);
        }
        public ActionResult Majors(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Collages");
            }
            var majors = db.Majors.Where(x => x.College_Id == id).ToList();
            return View(majors);
        }

        public ActionResult singleMajor(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Majors");
            }
            var majors = db.Majors.Where(x => x.Major_Id == id).ToList();
            return View(majors);
        }
        [Authorize(Roles = "Doctor")]
        public ActionResult DoctorsResults(int? courseid)
        {
            string doctorid = User.Identity.GetUserId();
            var doc = db.Doctors.Where(x => x.User_ID == doctorid).FirstOrDefault();
            int doc_id = doc.Doctor_Id;
            var semisteres = db.semesters;
            int current = 1;
            foreach (var semester in semisteres)
            {
                if (semester.end_date > DateTime.Now.Date && semester.start_date < DateTime.Now.Date)
                {
                    current = semester.id;
                }
            }
            var doctor_courses = db.Courses_Offered.Where(x => x.doctor_id == doc_id && x.semester_id == current).ToList();
            if (courseid == null)
            {
                ViewBag.message = "no";
                return View(doctor_courses);
            }


            var all_enrollments = db.Enrollments.Where(x => x.Course_id == courseid && x.semester_id == current).ToList();
            ViewBag.message = $"{courseid}";
            dynamic all = new ExpandoObject();
            all.d = doctor_courses;
            all.e = all_enrollments;


            return View(all);
        }

        [HttpPost]
        public ActionResult DoctorsResults(int courseid, IEnumerable<Enrollment> enrollments)
        {
            foreach (var enrollment in enrollments)
            {
                var existingEnrollment = db.Enrollments.Find(enrollment.Enrollment_Id);
                if (existingEnrollment != null)
                {
                    existingEnrollment.Course_mark = enrollment.Course_mark;
                    db.Entry(existingEnrollment).State = EntityState.Modified;
                }
            }

            db.SaveChanges();

            TempData["swal_message"] = $"Marks Updated Successfully ";
            ViewBag.title = "success";
            ViewBag.icon = "success";




            string doctorid = User.Identity.GetUserId();
            var doc = db.Doctors.Where(x => x.User_ID == doctorid).FirstOrDefault();
            int doc_id = doc.Doctor_Id;
            var semisteres = db.semesters;
            int current = 1;
            foreach (var semester in semisteres)
            {
                if (semester.end_date > DateTime.Now.Date && semester.start_date < DateTime.Now.Date)
                {
                    current = semester.id;
                }
            }
            var doctor_courses = db.Courses_Offered.Where(x => x.doctor_id == doc_id && x.semester_id == current).ToList();




            var all_enrollments = db.Enrollments.Where(x => x.Course_id == courseid && x.semester_id == current).ToList();
            ViewBag.message = $"{courseid}";
            dynamic all = new ExpandoObject();
            all.d = doctor_courses;
            all.e = all_enrollments;


            return View(all);

        }


    }
}