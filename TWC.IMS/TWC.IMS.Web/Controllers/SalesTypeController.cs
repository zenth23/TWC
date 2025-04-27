using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using TWC.IMS.BL;
using TWC.IMS.Models;
using TWC.IMS.Models.HelperClasses;
using TWC.IMS.Web.HelperClasses;
using TWC.IMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.Common.HelperClasses;
using System.Web.Script.Serialization;
using System.IO;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize]
    public class SalesTypesController : BaseController
    {
        [CustomAuthorize(AccessName = "SalesTypes.CanView")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SalesTypes()
        {
            return View();
        }

        [Route("{controller}/Details")]
        [Route("{controller}/Details/{id}")]
        public async Task<ActionResult> Details(int? id)
        {
            using (_SalesTypeBL = new BL.SalesTypes(User.Identity.Name))
            {
                var model = new Models.SalesTypeViewModel();

                model.Id = id == null ? Convert.ToInt32(0) : Convert.ToInt32(id);
                // edit mode
                if (model.Id != 0)
                {
                    var obj = await _SalesTypeBL.GetAsync(model.Id).ConfigureAwait(false);
                    if (obj != null)
                    {
                        model.Id = obj.Id;

                        // get audit trail from the parent module                                 
                        model.CreatedBy = string.Format("{0} {1}", obj.CreatedBy, (obj.Created == null ? "" : "on " + obj.Created.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                        model.ModifiedBy = string.Format("{0} {1}", obj.ModifiedBy, (obj.Modified == null ? "" : "on " + obj.Modified.Value.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_1)));
                    }
                    else
                        throw new HttpException(404, $"Subscriber not found.");
                }

                ViewBag.Mode = model.Id == 0 ? "CREATE" : "EDIT";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveSalesType([DataSourceRequest] DataSourceRequest request, SalesTypeViewModel model)
        {
            var username = User.Identity.Name;
            try
            {
                using (_SalesTypeBL = new BL.SalesTypes(username))
                {
                    Guid code = new Guid();
                    ModelState.Remove("Created");
                    ModelState.Remove("Modified");
                    ModelState.Remove("UniqueKey");
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0 && HttpContext.HasPermission("SalesTypes.CanAdd"))
                        {
                            var objGetByCode = await _SalesTypeBL.GetByNameAsync(model.SalesType_name).ConfigureAwait(false);

                            if (objGetByCode == null)
                            {
                                Guid UniqueKey = Guid.NewGuid();
                                var SupObj = new TWC.IMS.Models.SalesType();
                                SupObj.Description = model.Description;
                                SupObj.SalesType_name = model.SalesType_name;
                                SupObj.UniqueKey = UniqueKey;
                                SupObj.Created = DateTime.Now;
                                SupObj.CreatedBy = username;


                                await _SalesTypeBL.InsertAsync(SupObj).ConfigureAwait(false);
                            }
                            else
                                return Json(new { Success = false, Message = "SalesType already exists!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (model.Id != 0 && HttpContext.HasPermission("SalesTypes.CanEdit"))
                        {
                            var bussList = await _SalesTypeBL.GetListAsync().ConfigureAwait(false);
                            var obj = bussList.FirstOrDefault(b => b.Id == model.Id);

                            if (bussList.FirstOrDefault(b => b.Id != model.Id && b.SalesType_name == model.SalesType_name) != null)
                                return Json(new { Success = false, Message = "SalesType already exists!" }, JsonRequestBehavior.AllowGet);

                            if (obj != null)
                            {

                                obj.SalesType_name = model.SalesType_name;
                                obj.Description = model.Description;
                                obj.Modified = DateTime.Now;
                                obj.ModifiedBy = username;


                                await _SalesTypeBL.UpdateAsync(obj).ConfigureAwait(false);
                            }
                            else
                                return Json(new { Success = false, Message = "No SalesType found." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Success = false, Message = "Unauthorized." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                }
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [SkipLogActionFilter]
        public async Task<ActionResult> GetSalesType([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                using (_SalesTypeBL = new BL.SalesTypes(User.Identity.Name))
                {
                    var units = await _SalesTypeBL.GetListAsync().ConfigureAwait(false);
                    var list = new List<SalesTypeViewModel>();
                    foreach (var unit in units)
                    {
                        var model = new SalesTypeViewModel()
                        {
                            Id = unit.Id,

                            SalesType_name = unit.SalesType_name,
                            Description = unit.Description,
                            Created = unit.Created,
                            CreatedBy = unit.CreatedBy,
                            Modified = unit.Modified?.DateTime.AsNullable(),
                            ModifiedBy = unit.ModifiedBy,

                        };
                        list.Add(model);
                    }
                    DataSourceResult dsResult = list.ToDataSourceResult(request);
                    var serializer = new JavaScriptSerializer();
                    var result = new ContentResult();
                    serializer.MaxJsonLength = int.MaxValue;
                    result.Content = serializer.Serialize(dsResult);
                    result.ContentType = "application/json";
                    return result;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        [CustomAuthorize(AccessName = "SalesTypes.CanDelete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSalesType(int id)
        {
            var username = User.Identity.Name;
            try
            {
                using (var stBL = new BL.SalesTypes(username))

                if (id != 0)
                {
                    await stBL.DeleteAsync(id);
                }

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var _ = this.LogErrorAsync(MessageType.ERROR, ex, username);

                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}