using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class RoleDetails : IDisposable
    {
        private DL.RoleDetails _dlObj = null;
        private string _username;

        public RoleDetails(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public Task<IEnumerable<Models.RoleDetail>> GetListAsync(bool? isActive = null)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
                {
                    return _dlObj.GetListAsync(isActive);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<IEnumerable<Models.RoleDetail>> GetListAsync(IEnumerable<string> roles)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
                {
                    return _dlObj.GetListAsync(roles);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.RoleDetail> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
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

        public Task<List<Models.RoleDetail>> GetRoleWithUsersAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
                {
                    return _dlObj.GetRoleWithUsersAsync(id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.RoleDetail> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
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

        public Task<Models.RoleDetail> GetAsync(string roleId)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
                {
                    return _dlObj.GetAsync(roleId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<Models.RoleDetail> GetByNameAsync(string roleName)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
                {
                    return _dlObj.GetByNameAsync(roleName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<bool> HasActiveRoleAsync(IEnumerable<string> roleIds)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
                {
                    return _dlObj.HasActiveRoleAsync(roleIds);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> InsertAsync(Models.RoleDetail obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.UniqueKey = Guid.NewGuid();
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    using (_dlObj = new DL.RoleDetails(username))
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

        public Task<int> UpdateAsync(Models.RoleDetail obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.Modified = DateTime.Now;
                    obj.ModifiedBy = username;

                    using (_dlObj = new DL.RoleDetails(username))
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

        public Task<int> DeleteAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.RoleDetails(username))
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
                using (_dlObj = new DL.RoleDetails(username))
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
        // ~RoleDetails() {
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
