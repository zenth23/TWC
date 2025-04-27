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
using TWC.IMS.Common.Models;

namespace TWC.IMS.Common.HelperClasses
{
    public class AuditLogger
    {
        public async Task<List<AuditLog>> GetAuditRecordsWithChangeAsync(DbEntityEntry dbEntry, string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                List<AuditLog> result = new List<AuditLog>();
                DateTime changeTime = DateTime.Now;
                // Get the Table() attribute, if one exists
                TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() as TableAttribute;

                // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
                string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

                // Get primary key value (If you have more than one key column, this will need to be adjusted)
                var keyNames = dbEntry.Entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).ToList();

                if (keyNames.Count > 0)
                {
                    string keyName = keyNames[0].Name;
                    if (dbEntry.State == EntityState.Added)
                    {
                        // For Inserts, just add the whole record
                        // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()
                        result.Add(new AuditLog()
                        {
                            UniqueKey = Guid.NewGuid(),
                            CreatedBy = username,
                            Created = changeTime,
                            EventType = "ADDED",    // Added
                            TableName = tableName,
                            RowID = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(),
                            ColumnName = "*ALL",
                            NewValue = (dbEntry.CurrentValues.ToObject() is IDescribableEntity) ? await (dbEntry.CurrentValues.ToObject() as IDescribableEntity).Describe() : dbEntry.CurrentValues.ToObject().ToString()
                        });
                    }
                    else if (dbEntry.State == EntityState.Deleted)
                    {
                        // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                        result.Add(new AuditLog()
                        {
                            UniqueKey = Guid.NewGuid(),
                            CreatedBy = username,
                            Created = changeTime,
                            EventType = "DELETED",    // Deleted
                            TableName = tableName,
                            RowID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                            ColumnName = "*ALL",
                            OldValue = (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? await (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
                        });
                    }
                    else if (dbEntry.State == EntityState.Modified)
                    {
                        string[] ignoredColumns = { "created", "createdby", "modified", "modifiedby", "rowversion" };
                        // skip columns listed in [IgnoredColumns] list
                        var propertyNames = dbEntry.OriginalValues.PropertyNames.Where(a => !ignoredColumns.Contains(a.ToLower()));
                        foreach (string propertyName in propertyNames)
                        {
                            // For updates, we only want to capture the columns that actually changed
                            if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                            {
                                result.Add(new AuditLog()
                                {
                                    UniqueKey = Guid.NewGuid(),
                                    CreatedBy = username,
                                    Created = changeTime,
                                    EventType = "MODIFIED",    // Modified
                                    TableName = tableName,
                                    RowID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                                    ColumnName = propertyName,
                                    OldValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                                    NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                                });
                            }
                        }
                    }
                    // Otherwise, don't do anything, we don't care about Unchanged or Detached entities
                    return result;
                }
                else
                    throw new InvalidOperationException(string.Format("The model {0} is not compatible for Audit Trail use. Missing Key Attribute.", tableName));
            }
            else
                throw new InvalidOperationException("Username is required by Audit Trail. Cannot be empty.");
        }
    }
}
