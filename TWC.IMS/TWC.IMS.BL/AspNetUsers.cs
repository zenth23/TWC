using Kendo.Mvc.UI;
using TWC.IMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class AspNetUsers : IDisposable
    {
        private DL.AspNetUsers _dlObj = null;
        private string _username;

        public AspNetUsers(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.AspNetUser>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    return _dlObj.GetListAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.AspNetUser> GetAsync(string id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    return _dlObj.GetAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.AspNetUser> GetByUsernameAsync(string username)
        {
            string username2 = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username2))
                {
                    return _dlObj.GetByUsernameAsync(username);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<string> InsertAsync(Models.AspNetUser obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    using (_dlObj = new DL.AspNetUsers(username))
                    {
                        return _dlObj.InsertAsync(obj);
                    }
                }
                else throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateAsync(Models.AspNetUser obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    using (_dlObj = new DL.AspNetUsers(username))
                    {
                        return _dlObj.UpdateAsync(obj);
                    }
                }
                else throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateUsernameAsync(string userId, string username)
        {
            string username2 = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username2))
                {
                    return _dlObj.UpdateUsernameAsync(userId, username);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateUserEmailAsync(string userId, string email)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    return _dlObj.UpdateUserEmailAsync(userId, email);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdatePhoneNumberAsync(string userId, string newPhoneNumber)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    return _dlObj.UpdatePhoneNumberAsync(userId, newPhoneNumber);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateAllUsersSecurityStampAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    return _dlObj.UpdateAllUsersSecurityStampAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task UpdateSecurityStampAsync(IEnumerable<string> userIds)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    foreach (var userId in userIds)
                    {
                        await _dlObj.UpdateSecurityStampAsync(userId).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateSecurityStampAsync(string userId)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    return _dlObj.UpdateSecurityStampAsync(userId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.AspNetUsers(username))
                {
                    return _dlObj.DeleteAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
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
                    if (_dlObj != null)
                    {
                        _dlObj.Dispose();
                        _dlObj = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AspNetUsers() {
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
