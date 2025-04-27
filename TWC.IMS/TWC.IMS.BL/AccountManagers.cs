using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TWC.IMS.Models.HelperClasses;

namespace TWC.IMS.BL
{
    public class AccountManagers : IDisposable
    {
        private PasswordHistories _phBL = null;
        private SystemConfigs _scBL = null;
        private AspNetUsers _anuBL = null;
        private string _username;

        public AccountManagers(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<bool> IsPasswordExpiredAsync(string userId)
        {
            string username = _username;
            using (_phBL = new PasswordHistories(username))
            using (_scBL = new SystemConfigs(username))
            {
                var numString = await _scBL.GetValueAsync(SystemConfigName.PASSWORD_AGE).ConfigureAwait(false);
                var list = await _phBL.GetCurrentPasswordAsync(userId).ConfigureAwait(false);
                int num = 90;
                var isValidNum = int.TryParse(numString, out num);
                if (list == null)
                {
                    return false;
                }
                else if (!list.Created.HasValue)
                {
                    return true;
                }
                else
                {
                    var totalDays = (DateTime.Now - list.Created).Value.TotalDays;
                    if (totalDays >= (isValidNum ? num : 90))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public async Task<bool> IsPasswordInUseAsync(string userId, string passwordHash)
        {
            string username = _username;
            using (_phBL = new PasswordHistories(username))
            using (_scBL = new SystemConfigs(username))
            {
                PasswordHasher ph = new PasswordHasher();
                var numString = await _scBL.GetValueAsync(SystemConfigName.PASSWORD_THRESHOLD_COUNT).ConfigureAwait(false);
                int num = 13;
                var result = int.TryParse(numString, out num);
                var list = await _phBL.GetHistoricalPasswordsByUserIdAsync(userId, result ? num : 13).ConfigureAwait(false);
                if (list.Any())
                {
                    var existing = list.Where(x => ph.VerifyHashedPassword(x.PasswordHash, passwordHash) == PasswordVerificationResult.Success).FirstOrDefault();
                    if (existing != null)
                        return true;
                }
                return false;
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
                    if (_phBL != null)
                    {
                        _phBL.Dispose();
                        _phBL = null;
                    }

                    if (_scBL != null)
                    {
                        _scBL.Dispose();
                        _scBL = null;
                    }

                    if (_anuBL != null)
                    {
                        _anuBL.Dispose();
                        _anuBL = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AccountManagers() {
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
