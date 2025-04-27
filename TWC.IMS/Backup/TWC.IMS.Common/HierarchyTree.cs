using TWC.IMS.Common.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class HierarchyTree
    {
        public static IEnumerable<long> GetDescHelper(IEnumerable<HierarchyModel> list, long id, HashSet<long> already)
        {
            foreach (long child in GetChildren(list, id))
                if (already.Add(child))
                {
                    yield return child;
                    foreach (long desc in GetDescHelper(list, child, already))
                        yield return desc;
                }
        }

        public static IEnumerable<long> GetChildren(IEnumerable<HierarchyModel> list, long id)
        {
            foreach (HierarchyModel c in list)
                if (c.ParentId == id)
                    yield return c.Id;
        }

        public static IEnumerable<long> GetDescendants(IEnumerable<HierarchyModel> list, long id)
        {
            return GetDescHelper(list, id, new HashSet<long>());
        }
    }
}
