using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperClasses
{
    public abstract class DescribableEntity : IDescribableEntity
    {
        public async Task<string> Describe()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (System.Reflection.PropertyInfo property in this.GetType().GetProperties())
            {
                sb.Append(property.Name);
                sb.Append(": ");
                if (property.GetIndexParameters().Length > 0)
                {
                    sb.Append("Indexed Property cannot be used");
                }
                else
                {
                    sb.Append("\"" + property.GetValue(this, null) + "\"");
                }
                sb.Append(",");
            }
            sb.Append("}").Replace(",}", "}");
            return await Task.FromResult(sb.ToString());
        }
    }
}
