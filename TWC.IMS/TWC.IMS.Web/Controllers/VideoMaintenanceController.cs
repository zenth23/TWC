using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.BL;
using TWC.IMS.Models;

namespace TWC.IMS.Web.Controllers
{
    public class VideoMaintenanceController : Controller
    {
        private readonly string _username = "system"; // Replace with actual authenticated user

        // GET: VideoMaintenance
        public async Task<ActionResult> Index()
        {
            try
            {
                using (var bl = new VideoMaintenanceBL(_username))
                {
                    var videos = await bl.GetListAsync().ConfigureAwait(false);
                    return View(videos);
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
        public async Task<ActionResult> UploadAndCreate(VideoMaintenance model, HttpPostedFileBase videoFile)
        {
            if (videoFile == null || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                string fileName = Path.GetFileName(videoFile.FileName);
                string savePath = Path.Combine(Server.MapPath("~//Upload/Videos"), fileName);
                videoFile.SaveAs(savePath);

                model.Name = fileName;
                model.FilePath = savePath;//"/" + fileName;
                model.CreatedBy = _username;
                model.Created = DateTime.Now;

                using (var bl = new VideoMaintenanceBL(_username))
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
        public async Task<ActionResult> UploadAndUpdate(VideoMaintenance model, HttpPostedFileBase videoFile)
        {
            if (model.Id <= 0 || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                if (videoFile != null)
                {
                    string fileName = Path.GetFileName(videoFile.FileName);
                    string savePath = Path.Combine(Server.MapPath("~/Upload/Videos"), fileName);
                    videoFile.SaveAs(savePath);

                    model.Name = fileName;
                    model.FilePath = savePath;//"/" + fileName;
                }

                using (var bl = new VideoMaintenanceBL(_username))
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

        // POST: VideoMaintenance/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return new HttpStatusCodeResult(400, "Invalid ID");

            try
            {
                using (var bl = new VideoMaintenanceBL(_username))
                {
                    await bl.DeleteAsync(id).ConfigureAwait(false);
                }

                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        // (Optional) GET: Create (form-based view, unused in index scenario)
        public ActionResult Create()
        {
            return View();
        }
    }
}
