using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TWC.IMS.BL;
using TWC.IMS.Models;
using System.Reflection;
using System.Text;
using System.Security.Principal;
using TWC.IMS.Common.HelperClasses;
using System.Diagnostics;

namespace TWC.IMS.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly VideoMaintenanceBL videoBL;
        private readonly CarouselMaintenanceBL carouselBL;
        private readonly Products productBL;
        private readonly BL.Product_Master_Image productimgBL;

        public CatalogController()
        {
            string username = "twcusr";//System.Web.HttpContext.Current?.User?.Identity?.Name ?? "twcusr";
            videoBL = new VideoMaintenanceBL(username);
            carouselBL = new CarouselMaintenanceBL(username);
            productBL = new Products(username);
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetVideos()
        {
            var videos = await videoBL.GetListAsync();
            var grouped = videos
                .GroupBy(v => v.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(v => new {
                        name = v.Name,
                        FilePath = v.FilePath
                    }).ToList()
                );

            return Json(grouped, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetCarousels()
        {
            var carousels = await carouselBL.GetListAsync();
            var data = carousels
                .Select(c => new {
                    FilePath = c.FilePath
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetProducts()
        {
            var products = await productBL.GetListAsync();
            var productImages = await productimgBL.GetListAsync(); // You should implement this if not already

            // Join products with their images
            var catalogProducts = products
                .Where(p => p.Catalog == true)
                .Select(p =>
                {
                    var image = productImages.FirstOrDefault(img => img.product_id == p.Id);
                    return new
                    {
                        name = p.product_name,
                        category = "",
                        price = p.selling_price,
                        file_location = image?.FilePath // Use null-check in case there's no image
                    };
                })
                .ToList();


            return Json(catalogProducts, JsonRequestBehavior.AllowGet);
        }


    }
}
