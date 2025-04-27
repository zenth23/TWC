using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using TWC.IMS.Common.HelperClasses;

namespace TWC.IMS.Models
{
    public partial class Entities
    {
        // SOURCE: http://stackoverflow.com/questions/20961489/how-to-create-an-audit-trail-with-entity-framework-5-and-mvc-4

        public async Task<int> SaveChangesAsync(string username)
        {
            await DoAuditLogAsync(username).ConfigureAwait(false);
            // Call the original SaveChanges(), which will save both the changes made and the audit records
            return await base.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task DoAuditLogAsync(string username)
        {
            var al = new AuditLogger();
            // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
            var entries = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added ||
                                                                     p.State == EntityState.Deleted ||
                                                                     p.State == EntityState.Modified);
            foreach (var ent in entries)
            {
                // For each changed record, get the audit record entries and add them
                var changesCommon = await al.GetAuditRecordsWithChangeAsync(ent, username).ConfigureAwait(false);
                var changes = changesCommon.Select(a => new Models.AuditLog
                {
                    ColumnName = a.ColumnName,
                    Created = a.Created,
                    CreatedBy = a.CreatedBy,
                    EventType = a.EventType,
                    NewValue = a.NewValue,
                    OldValue = a.OldValue,
                    RowID = a.RowID,
                    TableName = a.TableName,
                    UniqueKey = a.UniqueKey
                })
                .ToList();

                foreach (Models.AuditLog x in changes)
                {
                    this.AuditLogs.Add(x);
                }
            }
        }

      
    }
}
