using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Runtime.Remoting.Messaging;

namespace TWC.IMS.Common.HelperClasses
{
    public class ProjectDbConfiguration : DbConfiguration
    {
        public ProjectDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => SuspendExecutionStrategy ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
                                                                                         : new ExecutionStrategy());
        }

        public static bool SuspendExecutionStrategy
        {
            get
            {
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
            }

            set
            {
                CallContext.LogicalSetData("SuspendExecutionStrategy", value);
            }
        }
    }
}
