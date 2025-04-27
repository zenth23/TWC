using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperClasses
{
    public interface IDescribableEntity
    {
        // Override this method to provide a description of the entity for audit purposes
        Task<string> Describe();
    }
}
