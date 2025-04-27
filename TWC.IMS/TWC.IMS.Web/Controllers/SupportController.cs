using TWC.IMS.Web.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.Web.Models;

using TWC.IMS.BL;
using System.Threading.Tasks;
using TWC.IMS.Models;

using Kendo;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace TWC.IMS.Web.Controllers
{
    [CustomAuthorize(Users = "smitsadmin.projectmold")]
    public class SupportController : BaseController
    {
        public ActionResult Maintenance()
        {
            return View();
        }

        public ActionResult QueryTool()
        {
            return View();
        }

        public async Task<ActionResult> Columns(int id)
        {
            using (_supportToolBL = new SupportTool(User.Identity.Name))
            {
                var columns = await _supportToolBL.GetColumnListAsync(id).ConfigureAwait(false);
                return Json(columns, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetTableName()
        {
            using (_supportToolBL = new SupportTool(User.Identity.Name))
            {
                return Json(await _supportToolBL.GetTableListAsync().ConfigureAwait(false), JsonRequestBehavior.AllowGet);
            }
        }

        #region helper functions
        public IEnumerable<SelectListItem> GenerateSelectTableList(int? selectedVal, IEnumerable<SQLTable> allList)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            item = new SelectListItem();
            item.Value = "0";
            item.Text = "-SELECT TABLE-";
            item.Selected = true;
            list.Add(item);

            foreach (var i in allList)
            {
                item = new SelectListItem();

                item.Value = i.TableId.ToString();
                item.Text = i.TableName;
                item.Selected = i.TableId == selectedVal ? true : false;

                list.Add(item);
            }
            return list.OrderBy(x => x.Text);
        }

        public async Task<DataTable> GetDataTableAsync(String connectionName, TWC.IMS.Models.HelperModels.QueryFilter condition, CommandType cmdType)
        {
            DataTable dtResults = new DataTable();
            //ConfigurationManager.AppSettings
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationEntities"].ToString()))
                {
                    var queryString = new StringBuilder();
                    queryString.AppendFormat("SELECT * FROM {0}", condition.TableName);
                    if (condition.Filters != null)
                    {
                        if (condition.Filters.Count > 0)
                        {
                            queryString.Append(" WHERE ");
                            for (var i = 0; i < condition.Filters.Count; i++)
                            {
                                condition.Filters[i].ParameterName = string.Format("{0}_{1}", condition.Filters[i].Field, i);
                                queryString.Append(condition.Filters[i].ConditionString);
                            }
                        }
                    }

                    using (SqlCommand sqlCmd = new SqlCommand(queryString.ToString(), sqlCon))
                    {
                        if (condition.Filters != null)
                        {
                            SqlParameter sqlParam = null;
                            foreach (var filter in condition.Filters)
                            {
                                if (filter.Condition != "IS NULL" && filter.Condition != "IS NOT NULL")
                                {
                                    if (filter.Condition.ToLower() == "contains")
                                    {
                                        sqlParam = new SqlParameter
                                        {
                                            ParameterName = filter.ParameterName,
                                            Value = filter.Value == null ? string.Empty : "%" + filter.Value + "%"
                                        };
                                    }
                                    else
                                    {


                                        sqlParam = new SqlParameter
                                        {
                                            ParameterName = filter.ParameterName,
                                            Value = filter.Value == null ? string.Empty : filter.Value
                                        };
                                    }
                                    sqlCmd.Parameters.Add(sqlParam);
                                }

                            }
                        }

                        sqlCon.Open();
                        SqlDataReader sqlDR = await sqlCmd.ExecuteReaderAsync().ConfigureAwait(false);
                        dtResults.Load(sqlDR);
                        sqlCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;// (new Exception("Database Error: " + ex.Message));
            }
            return dtResults;
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QueryTable(TWC.IMS.Models.HelperModels.QueryFilter condition)
        {
            var model = new QueryToolViewModel();
            if (condition != null)
            {
                model.ResultTable = TWC.IMS.Common.HelperClasses.AsyncHelpers.RunSync(() => GetDataTableAsync("ApplicationEntities", condition, CommandType.Text));
            }

            return PartialView("~/Views/Shared/EditorTemplates/QueryTable.cshtml", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_supportToolBL != null)
                    _supportToolBL = null;
            }

            base.Dispose(disposing);
        }
    }
}