using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWC.IMS.Models;
using System.Data;

namespace TWC.IMS.Web.Models
{
    [Serializable]
    public class QueryToolViewModel
    {
        //public IEnumerable<SelectListItem> TableList { get; set; }
        public IEnumerable<SQLColumn> ColumnList { get; set; }
        public IEnumerable<SelectListItem> TableList { get; set; }
        public long TableId { get; set; }
        public String Columns { get; set; }
        public DataTable ResultTable { get; set; }
        public IEnumerable<QueryFilter> QueryFilters { get; set; }

    }

    [Serializable]
    public class QueryFilter
    {
        public int Id { get; set; }
        public string LogicalOperator { get; set; }
        public string Field { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
    }
}