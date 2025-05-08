using System;
using System.Threading.Tasks;
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

        // POST: VideoMaintenance/Create (AJAX support)
        [HttpPost]
        public async Task<ActionResult> Create(VideoMaintenance model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.FilePath))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
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

        // POST: VideoMaintenance/Update (AJAX support)
        [HttpPost]
        public async Task<ActionResult> Update(VideoMaintenance model)
        {
            if (model.Id <= 0 || string.IsNullOrWhiteSpace(model.Name))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
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

        // POST: VideoMaintenance/Delete (AJAX support)
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

        // (Optional) GET: Create (legacy form-based view)
        public ActionResult Create()
        {
            return View();
        }

        // (Optional) POST: Create (form submission)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(VideoMaintenance model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    try
        //    {
        //        model.CreatedBy = _username;
        //        model.Created = DateTime.Now;

        //        using (var bl = new VideoMaintenanceBL(_username))
        //        {
        //            await bl.AddAsync(model).ConfigureAwait(false);
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.Message);
        //        return View(model);
        //    }
        //}
    }
}
