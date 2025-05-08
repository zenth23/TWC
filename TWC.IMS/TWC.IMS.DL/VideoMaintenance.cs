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
    public class VideoMaintenanceDL : IDisposable
    {
        private string _username;

        public VideoMaintenanceDL(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new NullReferenceException(TWC.IMS.Common.Messages.MUST_LOGIN_FIRST);

            _username = username;
        }

        public async Task<IEnumerable<VideoMaintenance>> GetListAsync(params string[] includeEntities)
        {
            try
            {
                using (var db = new Entities())
                {
                    var query = (DbQuery<VideoMaintenance>)db.Set<VideoMaintenance>();

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

        public async Task<VideoMaintenance> GetAsync(int id, params string[] includeEntities)
        {
            try
            {
                using (var db = new Entities())
                {
                    var query = (DbQuery<VideoMaintenance>)db.Set<VideoMaintenance>();

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

        public async Task AddAsync(VideoMaintenance item)
        {
            try
            {
                using (var db = new Entities())
                {
                    item.Created = DateTime.UtcNow;
                    item.CreatedBy = _username;
                    item.UniqueKey = Guid.NewGuid();

                    db.VideoMaintenance.Add(item);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(VideoMaintenance updatedItem)
        {
            try
            {
                using (var db = new Entities())
                {
                    var existing = await db.VideoMaintenance.FindAsync(updatedItem.Id).ConfigureAwait(false);
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
                        throw new KeyNotFoundException("VideoMaintenance item not found.");
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
                    var item = await db.VideoMaintenance.FindAsync(id).ConfigureAwait(false);
                    if (item != null)
                    {
                        db.VideoMaintenance.Remove(item);
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
