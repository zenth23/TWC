using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperModels
{
    [Serializable]
    public abstract class MessageObject
    {
        public MessageObject()
        {
            this.Messages = new List<string>();
        }

        public string Status { get; set; }
        public List<string> Messages { get; set; }
    }

}
