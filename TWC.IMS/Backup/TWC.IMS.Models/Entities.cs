namespace TWC.IMS.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=ApplicationEntities")
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }
        #region twcimc


        public virtual DbSet<Asset_Movement> Asset_Movement { get; set; }

        public virtual DbSet<Inventory_Entry> Inventory_Entry { get; set; }
        public virtual DbSet<Location> Locations { get; set; }

        public virtual DbSet<Movement_Type> Movement_Type { get; set; }

        public virtual DbSet<Product_Inventory> Product_Inventory { get; set; }
        public virtual DbSet<Product_Master> Product_Master { get; set; }

        public virtual DbSet<Shop> Shops { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        #endregion


        public virtual DbSet<Access> Accesses { get; set; }
      
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<DatabaseArchivingLog> DatabaseArchivingLogs { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<ModuleAccess> ModuleAccesses { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<PasswordHistory> PasswordHistories { get; set; }
        public virtual DbSet<ReportCache> ReportCaches { get; set; }
        public virtual DbSet<RoleDetail> RoleDetails { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<SQLColumn> SQLColumn { get; set; }
        public virtual DbSet<SQLTable> SQLTable { get; set; }
        public virtual DbSet<StatusSet> StatusSets { get; set; }
        public virtual DbSet<SystemConfig> SystemConfigs { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
     
        public virtual DbSet<Request> Requests { get; set; } // REQUEST FOR REMOVAL
    
        public virtual DbSet<SystemNotification> SystemNotifications { get; set; }
        public virtual DbSet<SmsOtpResponse> SmsOtpResponses { get; set; }
        public virtual DbSet<SignalRConnection> SignalRConnections { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product_Master_Image> Product_Master_Images { get; set; }
        
        public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public virtual DbSet<SalesOrderHeader> SalesOrderHeaders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            #region modelregion


            modelBuilder.Entity<Asset_Movement>()
                .Property(e => e.RowVersion)
                .IsFixedLength();


            modelBuilder.Entity<Inventory_Entry>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Location>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Location>()
                .HasMany(e => e.Product_Inventory)
                .WithOptional(e => e.Location)
                .HasForeignKey(e => e.location_id);



            modelBuilder.Entity<Movement_Type>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Movement_Type>()
                .HasMany(e => e.Asset_Movement)
                .WithOptional(e => e.Movement_Type)
                .HasForeignKey(e => e.movement_type_id);



            modelBuilder.Entity<Product_Inventory>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Product_Inventory>()
                .HasMany(e => e.Asset_Movement)
                .WithOptional(e => e.Product_Inventory)
                .HasForeignKey(e => e.inventory_id);

            modelBuilder.Entity<Product_Inventory>()
                .HasMany(e => e.Inventory_Entry)
                .WithOptional(e => e.Product_Inventory)
                .HasForeignKey(e => e.inventory_id);

            modelBuilder.Entity<Product_Master>()
                .Property(e => e.weight)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product_Master>()
                .Property(e => e.retail_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product_Master>()
                .Property(e => e.selling_price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product_Master>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Product_Master>()
                .HasMany(e => e.Product_Inventory)
                .WithOptional(e => e.Product_Master)
                .HasForeignKey(e => e.product_id);



            modelBuilder.Entity<Shop>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Shop>()
                .HasMany(e => e.Asset_Movement)
                .WithOptional(e => e.Shop)
                .HasForeignKey(e => e.shop_id);



            modelBuilder.Entity<Supplier>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.Product_Inventory)
                .WithOptional(e => e.Supplier)
                .HasForeignKey(e => e.supplier_id);



            #endregion

            modelBuilder.Entity<Access>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Access>()
                .HasMany(e => e.ModuleAccesses)
                .WithRequired(e => e.Access)
                .HasForeignKey(e => e.ModuleAccess_Access);

          

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.RoleDetails)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RoleDetail_AspNetRole);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.RolePermissions)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RolePermission_Role);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.PasswordHistories)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.PasswordHistory_AspNetUser);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.UserDetails)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserDetail_AspNetUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<DatabaseArchivingLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ModuleAccess>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ModuleAccess>()
                .HasMany(e => e.RolePermissions)
                .WithRequired(e => e.ModuleAccess)
                .HasForeignKey(e => e.RolePermission_ModuleAccess);

            modelBuilder.Entity<Module>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Module>()
                .HasMany(e => e.ModuleAccesses)
                .WithRequired(e => e.Module)
                .HasForeignKey(e => e.ModuleAccess_Module);

            modelBuilder.Entity<PasswordHistory>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

       

            modelBuilder.Entity<RoleDetail>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<RolePermission>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SQLTable>();

            modelBuilder.Entity<SQLColumn>();

            modelBuilder.Entity<StatusSet>()
                 .Property(e => e.RowVersion)
                 .IsFixedLength();

            modelBuilder.Entity<SystemConfig>()
                .Property(e => e.RowVersion)
                .IsFixedLength()
                .IsConcurrencyToken();

            modelBuilder.Entity<UserActivityLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<UserDetail>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

           

            // REQUEST FOR REMOVAL
            modelBuilder.Entity<StatusSet>()
              .HasMany(e => e.Requests)
              .WithRequired(e => e.Status)
              .HasForeignKey(e => e.Request_Status)
              .WillCascadeOnDelete(false);

        
            modelBuilder.Entity<UserDetail>()
             .HasMany(e => e.SystemNotifications)
             .WithRequired(e => e.UserDetail)
             .HasForeignKey(e => e.SystemNotification_UserDetail)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserDetail>()
            .HasMany(e => e.Requests)
            .WithRequired(e => e.Proponent)
            .HasForeignKey(e => e.Request_Proponent)
            .WillCascadeOnDelete(false);

          
            modelBuilder.Entity<SmsOtpResponse>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

         

            modelBuilder.Entity<ReportCache>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Supplier>()
            .HasMany(e => e.InventoryEntries)
            .WithRequired(e => e.Supplier)
            .HasForeignKey(e => e.supplier_id);

            modelBuilder.Entity<Location>()
            .HasMany(e => e.InventoryEntries)
            .WithRequired(e => e.Location)
            .HasForeignKey(e => e.location_id);

            modelBuilder.Entity<Category>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Inventory_Entry)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.category_id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Product_Master>()
              .HasMany(e => e.Inventory_Entry)
              .WithRequired(e => e.Product_Master)
              .HasForeignKey(e => e.product_id);


            modelBuilder.Entity<Product_Master_Image>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Product_Master>()
             .HasMany(e => e.Product_Master_Images)
             .WithRequired(e => e.Product_Master)
             .HasForeignKey(e => e.product_id);

            modelBuilder.Entity<Product_Master>()
                .HasMany(e => e.SalesOrderDetails)
                .WithRequired(e => e.Product_Master)
                .HasForeignKey(e => e.SalesOrderDetail_Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SalesOrderDetail>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SalesOrderHeader>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SalesOrderHeader>()
                .HasMany(e => e.SalesOrderDetails)
                .WithRequired(e => e.SalesOrderHeader)
                .HasForeignKey(e => e.SalesOrderDetail_SalesOrderHeader)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Supplier>()
              .HasMany(e => e.SalesOrderHeaders)
              .WithRequired(e => e.Supplier)
              .HasForeignKey(e => e.supplier_id);

            modelBuilder.Entity<Location>()
             .HasMany(e => e.SalesOrderHeaders)
             .WithRequired(e => e.Location)
             .HasForeignKey(e => e.location_id);
        }
    }
}
