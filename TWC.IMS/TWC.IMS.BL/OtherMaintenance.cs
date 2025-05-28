using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TWC.IMS.DL;
using TWC.IMS.Models;

namespace TWC.IMS.BL
{
    public class OtherMaintenanceBL : IDisposable
    {
        private readonly string _username;

        public OtherMaintenanceBL(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");

            _username = username;
        }

        // Get a list of all OtherMaintenance items
        public async Task<IEnumerable<OtherMaintenance>> GetListAsync()
        {
            using (var dl = new OtherMaintenanceDL(_username))
            {
                return await dl.GetListAsync();
            }
        }

        // Get a specific OtherMaintenance item by ID
        public async Task<OtherMaintenance> GetAsync(int id)
        {
            using (var dl = new OtherMaintenanceDL(_username))
            {
                return await dl.GetAsync(id);
            }
        }

        // Add a new OtherMaintenance item
        public async Task AddAsync(OtherMaintenance item)
        {
            using (var dl = new OtherMaintenanceDL(_username))
            {
                await dl.AddAsync(item);
            }
        }

        // Update an existing OtherMaintenance item
        public async Task UpdateAsync(OtherMaintenance item)
        {
            using (var dl = new OtherMaintenanceDL(_username))
            {
                await dl.UpdateAsync(item);
            }
        }

        // Delete an OtherMaintenance item by ID
        public async Task DeleteAsync(int id)
        {
            using (var dl = new OtherMaintenanceDL(_username))
            {
                await dl.DeleteAsync(id);
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
