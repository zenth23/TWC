using TWC.IMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TWC.IMS.BL
{
    public class Accesses : IDisposable
    {
        private DL.Accesses _dlObj = null;
        private string _username;

        public Accesses(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<DataTable> GetModuleAccessesDataTableAsync(string username, string roleid, bool schemaOnly = false)
        {
            using (var mBL = new Modules(username))
            using (var aBL = new Accesses(username))
            {
                var mList = await mBL.GetListAsync().ConfigureAwait(false);
                var aList = await aBL.GetListAsync().ConfigureAwait(false);

                var keys = new DataColumn[1];
                DataTable dtList = new DataTable();
                // build table columns
                var excludeCols = new[] { "URL", "IconClassName", "CreatedBy", "Created", "ModifiedBy", "Modified", "RowVersion", "ModuleAccesses" };
                var module = new Models.Module();
                var props = module.GetType().GetProperties().Select(a => a.Name)
                                                            .Where(a => !excludeCols.ToList().Contains(a))
                                                            .Select(a => a);
                foreach (var item in props)
                {
                    var col = new DataColumn(item);
                    dtList.Columns.Add(col);
                    if (item == "Id")
                    {
                        col.DataType = System.Type.GetType("System.Int32");
                        keys[0] = col;
                        dtList.PrimaryKey = keys;
                    }
                }

                foreach (var item in aList)
                {
                    var col = new DataColumn(item.Name);
                    col.DataType = System.Type.GetType("System.Boolean");
                    dtList.Columns.Add(col);

                    col = new DataColumn($"{item.Name}_role");
                    col.DataType = System.Type.GetType("System.Boolean");
                    dtList.Columns.Add(col);
                }

                if (!schemaOnly)
                {
                    // add rows to datatable by column
                    foreach (var item in mList.OrderBy(a => a.Name))
                    {
                        var dr = dtList.NewRow();
                        foreach (var col in dtList.Columns)
                        {
                            string colName = col.ToString();
                            var prop = item.GetType().GetProperty(colName);
                            if (prop != null)
                            {
                                var value = prop.GetValue(item);
                                if (colName == "Id")
                                    dr.SetField(colName, Convert.ToInt32(value));
                                else
                                    dr.SetField(colName, value ?? "");
                            }
                            else
                            {
                                // access data goes here
                                if (colName.ToLower().EndsWith("_role"))
                                {
                                    var permissions = item.ModuleAccesses.Select(a => a.RolePermissions).ToList();
                                    var faltList = permissions.SelectMany(a => a);
                                    var hasAccess = faltList.Any(a => string.Compare(a.RolePermission_Role, roleid, true) == 0 &&
                                                                      string.Compare(a.ModuleAccess.Access.Name.Trim(), colName.Replace("_role", ""), true) == 0);
                                    dr.SetField(colName, hasAccess);
                                }
                                else
                                {
                                    var hasAccess = item.ModuleAccesses.Any(a => string.Compare(a.Access.Name.Trim(), colName, true) == 0);
                                    dr.SetField(colName, hasAccess);
                                }
                            }
                        }
                        dtList.Rows.Add(dr);
                    }
                }

                return dtList;
            }
        }

        public Task<IEnumerable<Models.Access>> GetListAsync()
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Accesses(username))
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

        public Task<Models.Access> GetAsync(int id)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Accesses(username))
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

        public Task<Models.Access> GetAsync(Guid uniqueKey)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Accesses(username))
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

        public Task<Models.Access> GetAsync(string name)
        {
            string username = _username;
            try
            {
                using (_dlObj = new DL.Accesses(username))
                {
                    return _dlObj.GetAsync(name);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<int> InsertAsync(Models.Access obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.UniqueKey = Guid.NewGuid();
                    obj.Created = DateTime.Now;
                    obj.CreatedBy = username;

                    using (_dlObj = new DL.Accesses(username))
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

        public Task<int> UpdateAsync(Models.Access obj)
        {
            string username = _username;
            try
            {
                if (obj != null)
                {
                    obj.Modified = DateTime.Now;
                    obj.ModifiedBy = username;

                    using (_dlObj = new DL.Accesses(username))
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
                using (_dlObj = new DL.Accesses(username))
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
                using (_dlObj = new DL.Accesses(username))
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
        // ~Accesses() {
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
