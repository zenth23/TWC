using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.BL;
using TWC.IMS.Models;

namespace TWC.IMS.Web.Controllers
{
    public class BannerMaintenanceController : Controller
    {
        private readonly string _username = "system"; // Replace with actual authenticated user

        // GET: BannerMaintenance
        public async Task<ActionResult> Index()
        {
            try
            {
                using (var bl = new BannerMaintenanceBL(_username))
                {
                    var banners = await bl.GetListAsync().ConfigureAwait(false);
                    return View(banners);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        // GET: BannerMaintenance/Create (not needed for AJAX)
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadAndCreate(BannerMaintenance model, HttpPostedFileBase bannerFile)
        {
            if (bannerFile == null || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                string fileName = Path.GetFileName(bannerFile.FileName);
                string savePath = Path.Combine(Server.MapPath("~/Upload/Banner"), fileName);
                bannerFile.SaveAs(savePath);

                model.Name = fileName;
                model.FilePath = savePath;//"/" + fileName;
                model.CreatedBy = _username;
                model.Created = DateTime.Now;

                using (var bl = new BannerMaintenanceBL(_username))
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
        public async Task<ActionResult> UploadAndUpdate(BannerMaintenance model, HttpPostedFileBase bannerFile)
        {
            if (model.Id <= 0 || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                if (bannerFile != null)
                {
                    string fileName = Path.GetFileName(bannerFile.FileName);
                    string savePath = Path.Combine(Server.MapPath("~/Upload/Banner"), fileName);
                    bannerFile.SaveAs(savePath);

                    model.Name = fileName;
                    model.FilePath = savePath;//"/" + fileName;
                }

                using (var bl = new BannerMaintenanceBL(_username))
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

        // POST: BannerMaintenance/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return new HttpStatusCodeResult(400, "Invalid ID");

            try
            {
                using (var bl = new BannerMaintenanceBL(_username))
                {
                    await bl.DeleteAsync(id).ConfigureAwait(false);
                    return new HttpStatusCodeResult(200);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }
    }
}
