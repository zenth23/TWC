using System;
using System.Threading.Tasks;
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

        // POST: BannerMaintenance/Create (AJAX endpoint)
        [HttpPost]
        public async Task<ActionResult> Create(BannerMaintenance model)
        {
            if (string.IsNullOrWhiteSpace(model.Category) || string.IsNullOrWhiteSpace(model.Name))
                return new HttpStatusCodeResult(400, "Missing required fields");

            try
            {
                model.Created = DateTime.Now;
                model.CreatedBy = _username;

                using (var bl = new BannerMaintenanceBL(_username))
                {
                    await bl.AddAsync(model).ConfigureAwait(false);
                    return new HttpStatusCodeResult(200);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        // POST: BannerMaintenance/Update
        [HttpPost]
        public async Task<ActionResult> Update(BannerMaintenance model)
        {
            if (model.Id <= 0)
                return new HttpStatusCodeResult(400, "Invalid ID");

            try
            {
                using (var bl = new BannerMaintenanceBL(_username))
                {
                    await bl.UpdateAsync(model).ConfigureAwait(false);
                    return new HttpStatusCodeResult(200);
                }
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
