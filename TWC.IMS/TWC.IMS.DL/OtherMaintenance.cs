using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TWC.IMS.Models;

namespace TWC.IMS.DL
{
    public class OtherMaintenanceDL : IDisposable
    {
        private readonly string _username;

        public OtherMaintenanceDL(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");

            _username = username;
        }

        // Get a list of all OtherMaintenance items
        public async Task<IEnumerable<OtherMaintenance>> GetListAsync()
        {
            try
            {
                using (var db = new Entities())
                {
                    return await db.OtherMaintenance.AsNoTracking()
                                                     .OrderBy(x => x.Created)
                                                     .ToListAsync()
                                                     .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving data from the database.", ex);
            }
        }

        // Get a specific OtherMaintenance item by ID
        public async Task<OtherMaintenance> GetAsync(int id)
        {
            try
            {
                using (var db = new Entities())
                {
                    return await db.OtherMaintenance.FindAsync(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving the item from the database.", ex);
            }
        }

        // Add a new OtherMaintenance item
        public async Task AddAsync(OtherMaintenance item)
        {
            try
            {
                using (var db = new Entities())
                {
                    item.Created = DateTime.UtcNow;
                    item.CreatedBy = _username;
                    item.UniqueKey = Guid.NewGuid();

                    db.OtherMaintenance.Add(item);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding data to the database.", ex);
            }
        }

        // Update an existing OtherMaintenance item
        public async Task UpdateAsync(OtherMaintenance item)
        {
            try
            {
                using (var db = new Entities())
                {
                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating data in the database.", ex);
            }
        }

        // Delete an OtherMaintenance item by ID
        public async Task DeleteAsync(int id)
        {
            try
            {
                using (var db = new Entities())
                {
                    var item = await db.OtherMaintenance.FindAsync(id);
                    if (item != null)
                    {
                        db.OtherMaintenance.Remove(item);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting data from the database.", ex);
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
