using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.BL;
using TWC.IMS.Models;
using TWC.IMS.Web.HelperClasses;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class CatalogMaintenanceController : Controller
    {
        private CatalogMaintenanceBL _bl;
        private string _username;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _username = User?.Identity?.Name ?? "Unknown";
            _bl = new CatalogMaintenanceBL(_username);
        }

        // GET: Catalog
        public async Task<ActionResult> Index()
        {
            try
            {
                var catalogList = await _bl.GetListAsync();
                return View(catalogList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "There was an error loading the catalog items. Please try again later.";
                return View(new List<CatalogMaintenance>());
            }
        }

        // GET: Catalog/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var catalogItem = await _bl.GetAsync(id);
                if (catalogItem == null)
                    return HttpNotFound();

                return View(catalogItem);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "There was an error fetching the details for this catalog item.";
                return View();
            }
        }

        // GET: Catalog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Catalog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CatalogMaintenance catalog, HttpPostedFileBase uploadedFile)
        {
            try
            {
                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    catalog.FileName = Path.GetFileName(uploadedFile.FileName);
                    catalog.FileType = uploadedFile.ContentType;

                    using (var reader = new BinaryReader(uploadedFile.InputStream))
                    {
                        catalog.FileContent = reader.ReadBytes(uploadedFile.ContentLength);
                    }
                }

                if (ModelState.IsValid)
                {
                    await _bl.AddAsync(catalog);
                    return RedirectToAction("Index");
                }

                return View(catalog);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "There was an error creating the catalog item. Please try again later.";
                return View(catalog);
            }
        }

        // GET: Catalog/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var catalogItem = await _bl.GetAsync(id);
                if (catalogItem == null)
                    return HttpNotFound();

                return View(catalogItem);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "There was an error fetching the catalog item to delete.";
                return View();
            }
        }

        // POST: Catalog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _bl.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "There was an error deleting the catalog item. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _bl != null)
            {
                _bl.Dispose();
                _bl = null;
            }
            base.Dispose(disposing);
        }
    }
}
