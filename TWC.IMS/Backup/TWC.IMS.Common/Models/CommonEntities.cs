namespace TWC.IMS.Common.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CommonEntities : DbContext
    {
        public CommonEntities()
            : base("name=ApplicationEntities")
        {
        }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<SmsOtpResponse> SmsOtpResponses { get; set; }
        public virtual DbSet<SystemConfig> SystemConfigs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SmsOtpResponse>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SystemConfig>()
                .Property(e => e.RowVersion)
                .IsFixedLength();
        }
    }
}
