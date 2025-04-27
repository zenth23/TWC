using TWC.IMS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.BL
{
    public class UserDetails : IDisposable
    {
        private DL.UserDetails _dlObj = null;
        private string _username;

        public UserDetails(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public string SetUserStatus(bool isLocked, bool isExpired, bool isActive)
        {
            return isLocked ? "Locked" : isExpired ? "Expired" : isActive ? "Active" : "Inactive";
        }

        public Task<bool> IsAccountActiveAsync(string uname)
        {
            string username = _username;
            try
            {
                _dlObj = new DL.UserDetails(username);
                return _dlObj.IsAccountActiveAsync(uname);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.UserDetail>> GetListAsync(bool? isActive = null, params string[] includeEntities)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
                {
                    if(includeEntities.Length == 0)
                    {
                        includeEntities = new string[]
                        {
                            "AspNetUser.AspNetRoles"
                        };
                    }

                    return _dlObj.GetListAsync(isActive, includeEntities);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }      

        public Task<Models.UserDetail> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
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

        public Task<Models.UserDetail> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
                {
                    return _dlObj.GetAsync(uniqueKey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.UserDetail> GetByUsernameAsync(string username)
        {
            string username2 = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username2))
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

        public Task<Models.UserDetail> GetByUserIdAsync(string userId)
        {
            string username2 = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username2))
                {
                    return _dlObj.GetByUserIdAsync(userId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> InsertAsync(Models.UserDetail obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.UniqueKey = Guid.NewGuid();
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    using (_dlObj = new DL.UserDetails(username))
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

        public Task<int> DeactivateUserAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
                {
                    return _dlObj.DeactivateUserAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> SetUserLastLoginDatetimeAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
                {
                    return _dlObj.SetUserLastLoginDatetimeAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> ActivateUserAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
                {
                    return _dlObj.ActivateUserAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateAsync(Models.UserDetail obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.Modified = DateTime.Now;
                    obj.ModifiedBy = username;

                    var x = await this.GetAsync(obj.Id).ConfigureAwait(false);
                    if (x != null)
                    {
                        DateTime? activationDatetime = null;
                        DateTime? deactivationDatetime = null;

                        //inactive, inactive
                        //if (!obj.IsActive && !x.IsActive)
                        //{
                        //// keep both null
                        //}
                        //else 
                        if (obj.IsActive && !x.IsActive)
                        {
                            activationDatetime = DateTime.Now;
                        }
                        else if (!obj.IsActive && x.IsActive)
                        {
                            activationDatetime = x.ActivationDatetime;
                            deactivationDatetime = DateTime.Now;
                        }
                        else if (obj.IsActive && x.IsActive)
                        {
                            // keep as is
                            activationDatetime = x.ActivationDatetime;
                            deactivationDatetime = x.DeactivationDatetime;
                        }

                        obj.ActivationDatetime = activationDatetime;
                        obj.DeactivationDatetime = deactivationDatetime;
                    }

                    using (_dlObj = new DL.UserDetails(username))
                    {
                        return await _dlObj.UpdateAsync(obj).ConfigureAwait(false);
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

        public Task<int> UpdateStatusLockUserAsync(string userId)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
                {
                    return _dlObj.UpdateStatusAsync(userId, "Locked");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> UpdateStatusUnlockUserAsync(string userId, bool isLocked, bool isExpired, bool isActive)
        {
            string username = _username;
            try
            {
                string status = this.SetUserStatus(isLocked, isExpired, isActive);
                using (_dlObj = new DL.UserDetails(username))
                {
                    return _dlObj.UpdateStatusAsync(userId, status);
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
                using (_dlObj = new DL.UserDetails(username))
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

        public Task<int> DeleteAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.UserDetails(username))
                {
                    return _dlObj.DeleteAsync(uniqueKey);
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
        // ~UserDetails() {
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
