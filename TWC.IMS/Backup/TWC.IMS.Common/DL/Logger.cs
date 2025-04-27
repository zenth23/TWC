using TWC.IMS.Common.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.DL
{
    public class Logger : IDisposable
    {
        public async Task LogAsync(Models.ErrorLog obj)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    db.ErrorLogs.Add(obj);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // write error message to EventViewer
                string message = Tools.ObjectToJson(ex);
                var _ = TWC.IMS.Common.Logger.LogToEventViewer(message);
            }
            finally
            {
                this.Dispose();
            }
        }

        public async Task LogEmailAsync(Models.EmailLog obj)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    db.EmailLogs.Add(obj);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // write error message to EventViewer
                string message = Tools.ObjectToJson(ex);
                var _ = TWC.IMS.Common.Logger.LogToEventViewer(message);
            }
            finally
            {
                this.Dispose();
            }
        }

        public async Task UpdateEmailLogStatusAsync(int emailLogId, StatusType statusType, string username)
        {
            try
            {
                using (var db = new Models.CommonEntities())
                {
                    var obj = await db.EmailLogs.FindAsync(emailLogId).ConfigureAwait(false);
                    if (obj != null)
                    {
                        obj.Status = statusType.ToString();
                        obj.Modified = DateTime.Now;
                        obj.ModifiedBy = username;
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                // write error message to EventViewer
                string message = Tools.ObjectToJson(ex);
                var _ = TWC.IMS.Common.Logger.LogToEventViewer(message);
            }
            finally
            {
                this.Dispose();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LoggerDL() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
