using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TWC.IMS.Models;

namespace TWC.IMS.DL
{
    public class TestimonialMaintenanceDL : IDisposable
    {
        private string _username;

        public TestimonialMaintenanceDL(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<TestimonialMaintenance>> GetListAsync(params string[] includeEntities)
        {
            try
            {
                using (var db = new Entities())
                {
                    var query = (DbQuery<TestimonialMaintenance>)db.Set<TestimonialMaintenance>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .OrderBy(x => x.Created)
                                      .ToListAsync()
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<TestimonialMaintenance> GetAsync(int id, params string[] includeEntities)
        {
            try
            {
                using (var db = new Entities())
                {
                    var query = (DbQuery<TestimonialMaintenance>)db.Set<TestimonialMaintenance>();

                    if (includeEntities != null && includeEntities.Length > 0)
                        includeEntities.ToList().ForEach(a => { query = query.Include(a); });

                    return await query.AsNoTracking()
                                      .FirstOrDefaultAsync(a => a.Id == id)
                                      .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task AddAsync(TestimonialMaintenance item)
        {
            try
            {
                using (var db = new Entities())
                {
                    item.Created = DateTime.UtcNow;
                    item.CreatedBy = _username;
                    item.UniqueKey = Guid.NewGuid();

                    db.TestimonialMaintenance.Add(item);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(TestimonialMaintenance updatedItem)
        {
            try
            {
                using (var db = new Entities())
                {
                    var existing = await db.TestimonialMaintenance.FindAsync(updatedItem.Id).ConfigureAwait(false);
                    if (existing != null)
                    {
                        existing.Name = updatedItem.Name;
                        existing.Category = updatedItem.Category;
                        existing.FilePath = updatedItem.FilePath;
                        // Optionally: existing.Updated = DateTime.UtcNow;
                        // Optionally: existing.UpdatedBy = _username;

                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        throw new KeyNotFoundException("TestimonialMaintenance item not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using (var db = new Entities())
                {
                    var item = await db.TestimonialMaintenance.FindAsync(id).ConfigureAwait(false);
                    if (item != null)
                    {
                        db.TestimonialMaintenance.Remove(item);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed resources
                }

                _username = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
