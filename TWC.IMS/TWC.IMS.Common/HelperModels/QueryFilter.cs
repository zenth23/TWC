using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperModels
{
    [Serializable]
    public class QueryFilter
    {
        public string TableName { get; set; }
        public List<Filter> Filters { get; set; }
    }

    [Serializable]
    public class Filter
    {
        public string LogicalOperator { get; set; }
        public string Field { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
        public string ParameterName { get; set; }
        public string ConditionString
        {
            get
            {
                if(Condition.ToUpper() == "IS NOT NULL" || Condition.ToUpper() == "IS NULL")
                    return string.Format(" {0} {1} {2}", LogicalOperator, Field, Condition);
                else if (Condition.ToLower() == "contains")
                    return string.Format(" {0} {1} {2} @{3} ", LogicalOperator, Field, "LIKE", ParameterName);
                else
                    return string.Format(" {0} {1} {2} @{3} ", LogicalOperator, Field, Condition, ParameterName);
                
            }
        }
    }
}
