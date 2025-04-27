using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Models.HelperModels
{
    [Serializable]
    public struct KeyIdValueModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
    }
}
