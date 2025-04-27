using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperModels
{
    [Serializable]
    public struct HierarchyModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
    }
}
