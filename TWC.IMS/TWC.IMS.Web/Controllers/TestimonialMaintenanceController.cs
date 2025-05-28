using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.BL;
using TWC.IMS.Models;

namespace TWC.IMS.Web.Controllers
{
    public class TestimonialMaintenanceController : Controller
    {
        private readonly string _username = "system"; // Replace with actual authenticated user

        // GET: TestimonialMaintenance
        public async Task<ActionResult> Index()
        {
            try
            {
                using (var bl = new TestimonialMaintenanceBL(_username))
                {
                    var testimonial = await bl.GetListAsync().ConfigureAwait(false);
                    return View(testimonial);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // POST: VideoMaintenance/UploadAndCreate
        [HttpPost]
        public async Task<ActionResult> UploadAndCreate(TestimonialMaintenance model, HttpPostedFileBase testimonialFile)
        {
            if (testimonialFile == null || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                string fileName = Path.GetFileName(testimonialFile.FileName);
                string savePath = Path.Combine(Server.MapPath("~/Upload/Testimonials"), fileName);
                testimonialFile.SaveAs(savePath);

                model.Name = fileName;
                model.FilePath = savePath;//"/" + fileName;
                model.CreatedBy = _username;
                model.Created = DateTime.Now;

                using (var bl = new TestimonialMaintenanceBL(_username))
                {
                    await bl.AddAsync(model).ConfigureAwait(false);
                }

                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        // POST: VideoMaintenance/UploadAndUpdate
        [HttpPost]
        public async Task<ActionResult> UploadAndUpdate(TestimonialMaintenance model, HttpPostedFileBase testimonialFile)
        {
            if (model.Id <= 0 || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                if (testimonialFile != null)
                {
                    string fileName = Path.GetFileName(testimonialFile.FileName);
                    string savePath = Path.Combine(Server.MapPath("~/Upload/Testimonials"), fileName);
                    testimonialFile.SaveAs(savePath);

                    model.Name = fileName;
                    model.FilePath = savePath;//"/" + fileName;
                }

                using (var bl = new TestimonialMaintenanceBL(_username))
                {
                    await bl.UpdateAsync(model).ConfigureAwait(false);
                }

                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }


        // POST: TestimonialMaintenance/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid ID");

            try
            {
                using (var bl = new TestimonialMaintenanceBL(_username))
                {
                    await bl.DeleteAsync(id).ConfigureAwait(false);
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
