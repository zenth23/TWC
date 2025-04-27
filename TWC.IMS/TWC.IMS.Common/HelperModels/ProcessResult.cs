using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperModels
{
    [Serializable]
    public struct ProcessResult
    {
        public bool Completed;
        public int? ExitCode;
        public string Output;
        public string GpgUserId;
    }
}
