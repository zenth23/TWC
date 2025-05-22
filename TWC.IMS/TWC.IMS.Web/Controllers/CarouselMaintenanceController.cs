using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.BL;
using TWC.IMS.Models;

namespace TWC.IMS.Web.Controllers
{
    public class CarouselMaintenanceController : Controller
    {
        private readonly string _username = "system"; // Replace with actual authenticated user

        // GET: CarouselMaintenance
        public async Task<ActionResult> Index()
        {
            try
            {
                using (var bl = new CarouselMaintenanceBL(_username))
                {
                    var carousel = await bl.GetListAsync().ConfigureAwait(false);
                    return View(carousel);
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
        public async Task<ActionResult> UploadAndCreate(CarouselMaintenance model, HttpPostedFileBase carouselFile)
        {
            if (carouselFile == null || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                string fileName = Path.GetFileName(carouselFile.FileName);
                string savePath = Path.Combine(Server.MapPath("~//Upload/Carousel"), fileName);
                carouselFile.SaveAs(savePath);

                model.Name = fileName;
                model.FilePath = savePath;//"/" + fileName;
                model.CreatedBy = _username;
                model.Created = DateTime.Now;

                using (var bl = new CarouselMaintenanceBL(_username))
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
        public async Task<ActionResult> UploadAndUpdate(CarouselMaintenance model, HttpPostedFileBase carouselFile)
        {
            if (model.Id <= 0 || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                if (carouselFile != null)
                {
                    string fileName = Path.GetFileName(carouselFile.FileName);
                    string savePath = Path.Combine(Server.MapPath("~/Upload/Carousel"), fileName);
                    carouselFile.SaveAs(savePath);

                    model.Name = fileName;
                    model.FilePath = savePath;//"/" + fileName;
                }

                using (var bl = new CarouselMaintenanceBL(_username))
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

        // POST: CarouselMaintenance/Delete
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "Invalid ID." });

            try
            {
                using (var bl = new CarouselMaintenanceBL(_username))
                {
                    await bl.DeleteAsync(id).ConfigureAwait(false);
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
