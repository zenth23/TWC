using System;
using System.Threading.Tasks;
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

        // POST: CarouselMaintenance/Create
        [HttpPost]
        public async Task<JsonResult> Create(CarouselMaintenance model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data." });

            try
            {
                using (var bl = new CarouselMaintenanceBL(_username))
                {
                    model.Created = DateTime.Now;
                    model.CreatedBy = _username;
                    await bl.AddAsync(model).ConfigureAwait(false);
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: CarouselMaintenance/Update
        [HttpPost]
        public async Task<JsonResult> Update(CarouselMaintenance model)
        {
            if (!ModelState.IsValid || model.Id == 0)
                return Json(new { success = false, message = "Invalid data." });

            try
            {
                using (var bl = new CarouselMaintenanceBL(_username))
                {
                    await bl.UpdateAsync(model).ConfigureAwait(false);
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
