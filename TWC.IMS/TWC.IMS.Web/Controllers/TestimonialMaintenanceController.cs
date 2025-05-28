using System;
using System.Net;
using System.Threading.Tasks;
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

        // POST: TestimonialMaintenance/Create
        [HttpPost]
        public async Task<ActionResult> Create(TestimonialMaintenance model)
        {
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid data");

            try
            {
                model.CreatedBy = _username;
                model.Created = DateTime.Now;

                using (var bl = new TestimonialMaintenanceBL(_username))
                {
                    await bl.AddAsync(model).ConfigureAwait(false);
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: TestimonialMaintenance/Update
        [HttpPost]
        public async Task<ActionResult> Update(TestimonialMaintenance model)
        {
            if (model.Id <= 0 || !ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid data");

            try
            {
                using (var bl = new TestimonialMaintenanceBL(_username))
                {
                    await bl.UpdateAsync(model).ConfigureAwait(false);
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
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
