using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        // POST: OtherMaintenances/Update
        [HttpPost]
        public async Task<ActionResult> Update(OtherMaintenance model)
        {
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid data.");

            using (var bl = new OtherMaintenanceBL(_username))
            {
                var existing = await bl.GetAsync(model.Id);
                if (existing == null)
                    return HttpNotFound("Item not found.");

                // Update editable fields
                existing.Name = model.Name;
                existing.Category = model.Category;
                existing.FilePath = model.FilePath;
                existing.ProductMasterId = model.ProductMasterId;

                // Metadata
                existing.Modified = DateTime.UtcNow;
                existing.ModifiedBy = _username;

                await bl.UpdateAsync(existing);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        // POST: OtherMaintenances/Create
        [HttpPost]
        public async Task<ActionResult> Create(OtherMaintenance model)
        {
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid data.");

            using (var bl = new OtherMaintenanceBL(_username))
            {
                model.Created = DateTime.UtcNow;
                model.CreatedBy = _username;
                model.UniqueKey = Guid.NewGuid();

                // ✅ Ensure ProductMasterId is assigned
                var newItem = new OtherMaintenance
                {
                    Name = model.Name,
                    Category = model.Category,
                    FilePath = model.FilePath,
                    ProductMasterId = model.ProductMasterId,
                    Created = model.Created,
                    CreatedBy = model.CreatedBy,
                    UniqueKey = model.UniqueKey
                };

                await bl.AddAsync(newItem);
                return new HttpStatusCodeResult(HttpStatusCode.Created);
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
