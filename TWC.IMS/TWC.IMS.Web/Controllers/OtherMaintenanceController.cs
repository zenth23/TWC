using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.BL;
using TWC.IMS.Models;
using Product_Inventory = TWC.IMS.BL.Product_Inventory;

namespace TWC.IMS.Web.Controllers
{
    public class OtherMaintenanceController : Controller
    {
        private string _username => User?.Identity?.Name ?? "System";

        // GET: OtherMaintenances
        public async Task<ActionResult> Index()
        {
            using (var otherBL = new OtherMaintenanceBL(_username))
            using (var productBL = new Products(_username))
            {
                var data = await otherBL.GetListAsync();
                var productList = await productBL.GetListAsync();

                ViewBag.ProductList = productList
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.product_name
                    })
                    .ToList();

                return View(data);
            }
        }

        // POST: VideoMaintenance/UploadAndCreate
        [HttpPost]
        public async Task<ActionResult> UploadAndCreate(OtherMaintenance model, HttpPostedFileBase othersFile)
        {
            if (othersFile == null || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                string fileName = Path.GetFileName(othersFile.FileName);
                string savePath = Path.Combine(Server.MapPath("~/Upload/Others"), fileName);
                othersFile.SaveAs(savePath);

                model.Name = fileName;
                model.FilePath = savePath;//"/" + fileName;
                model.CreatedBy = _username;
                model.Created = DateTime.Now;

                using (var bl = new OtherMaintenanceBL(_username))
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
        public async Task<ActionResult> UploadAndUpdate(OtherMaintenance model, HttpPostedFileBase othersFile)
        {
            if (model.Id <= 0 || string.IsNullOrWhiteSpace(model.Category))
                return new HttpStatusCodeResult(400, "Invalid data");

            try
            {
                if (othersFile != null)
                {
                    string fileName = Path.GetFileName(othersFile.FileName);
                    string savePath = Path.Combine(Server.MapPath("~/Upload/Others"), fileName);
                    othersFile.SaveAs(savePath);

                    model.Name = fileName;
                    model.FilePath = savePath;//"/" + fileName;
                }

                using (var bl = new OtherMaintenanceBL(_username))
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



        // POST: OtherMaintenances/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (var bl = new OtherMaintenanceBL(_username))
            {
                var existing = await bl.GetAsync(id);
                if (existing == null)
                    return HttpNotFound("Item not found.");

                await bl.DeleteAsync(id);
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
        }
    }
}
