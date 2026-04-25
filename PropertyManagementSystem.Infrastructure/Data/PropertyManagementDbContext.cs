namespace PropertyManagementSystem.Infrastructure.Data
{
    public class PropertyManagementDbContext(DbContextOptions<PropertyManagementDbContext> options)
        : DbContext(options)
    {
        #region Tables

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<Unit> Units { get; set; } = null!;
        public DbSet<Lease> Leases { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        #endregion

        #region Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region User & Role

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Property & Owner (User)

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Unit & Property  ← FIX: WithMany(p => p.Units)

            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Property)
                .WithMany(p => p.Units)          // was: .WithMany()
                .HasForeignKey(u => u.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Lease & Unit  ← FIX: WithMany(u => u.Leases)

            modelBuilder.Entity<Lease>()
                .HasOne(l => l.Unit)
                .WithMany(u => u.Leases)         // was: .WithMany()
                .HasForeignKey(l => l.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Lease & Tenant (User)

            modelBuilder.Entity<Lease>()
                .HasOne(l => l.Tenant)
                .WithMany()
                .HasForeignKey(l => l.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Payment & Lease  ← FIX: WithMany(l => l.Payments)

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Lease)
                .WithMany(l => l.Payments)       // was: .WithMany()
                .HasForeignKey(p => p.LeaseId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region MaintenanceRequest & Unit

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Unit)
                .WithMany()
                .HasForeignKey(m => m.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region MaintenanceRequest & Tenant

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Tenant)
                .WithMany()
                .HasForeignKey(m => m.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Message & Sender

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Message & Receiver

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Notification & User

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region AuditLog & User

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Decimal Precision

            modelBuilder.Entity<Lease>()
                .Property(l => l.RentAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Unit>()
                .Property(u => u.RentAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            #endregion

            RoleSeeder.SeedRoles(modelBuilder);
        }

        #endregion
    }
}


//namespace PropertyManagementSystem.Infrastructure.Data
//{
//    public class PropertyManagementDbContext(DbContextOptions<PropertyManagementDbContext> options) : DbContext(options)
//    {

//        #region Tables

//        public DbSet<User> Users { get; set; } = null!;

//        public DbSet<Role> Roles { get; set; } = null!;

//        public DbSet<Property> Properties { get; set; } = null!;

//        public DbSet<Unit> Units { get; set; } = null!;

//        public DbSet<Lease> Leases { get; set; } = null!;

//        public DbSet<Payment> Payments { get; set; } = null!;

//        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; } = null!;

//        public DbSet<Message> Messages { get; set; } = null!;

//        public DbSet<Notification> Notifications { get; set; } = null!;

//        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

//        #endregion

//        #region Methods

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            #region Methods User & Role

//            modelBuilder.Entity<User>()
//                .HasOne(u => u.Role)
//                .WithMany()
//                .HasForeignKey(u => u.RoleId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods Property & Owner (User)

//            modelBuilder.Entity<Property>()
//                .HasOne(p => p.Owner)
//                .WithMany()
//                .HasForeignKey(p => p.OwnerId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods Unit & Property

//            modelBuilder.Entity<Unit>()
//                .HasOne(u => u.Property)
//                .WithMany()
//                .HasForeignKey(u => u.PropertyId)
//                .OnDelete(DeleteBehavior.Cascade);

//            #endregion

//            #region Methods Lease & Unit

//            modelBuilder.Entity<Lease>()
//                .HasOne(l => l.Unit)
//                .WithMany()
//                .HasForeignKey(l => l.UnitId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods Lease & Tenant (User)

//            modelBuilder.Entity<Lease>()
//                .HasOne(l => l.Tenant)
//                .WithMany()
//                .HasForeignKey(l => l.TenantId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods Payment & Lease

//            modelBuilder.Entity<Payment>()
//                .HasOne(p => p.Lease)
//                .WithMany()
//                .HasForeignKey(p => p.LeaseId)
//                .OnDelete(DeleteBehavior.Cascade);

//            #endregion

//            #region Methods MaintenanceRequest & Unit

//            modelBuilder.Entity<MaintenanceRequest>()
//                .HasOne(m => m.Unit)
//                .WithMany()
//                .HasForeignKey(m => m.UnitId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods MaintenanceRequest & Tenant

//            modelBuilder.Entity<MaintenanceRequest>()
//                .HasOne(m => m.Tenant)
//                .WithMany()
//                .HasForeignKey(m => m.TenantId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods Message & Sender

//            modelBuilder.Entity<Message>()
//                .HasOne(m => m.Sender)
//                .WithMany()
//                .HasForeignKey(m => m.SenderId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods Message & Receiver

//            modelBuilder.Entity<Message>()
//                .HasOne(m => m.Receiver)
//                .WithMany()
//                .HasForeignKey(m => m.ReceiverId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods Notification & User

//            modelBuilder.Entity<Notification>()
//                .HasOne(n => n.User)
//                .WithMany()
//                .HasForeignKey(n => n.UserId)
//                .OnDelete(DeleteBehavior.Cascade);

//            #endregion

//            #region Methods AuditLog & User

//            modelBuilder.Entity<AuditLog>()
//                .HasOne(a => a.User)
//                .WithMany()
//                .HasForeignKey(a => a.UserId)
//                .OnDelete(DeleteBehavior.Restrict);

//            #endregion

//            #region Methods lease & Unit & Payment

//            modelBuilder.Entity<Lease>()
//                .Property(l => l.RentAmount)
//                .HasPrecision(18, 2);

//            modelBuilder.Entity<Unit>()
//                .Property(u => u.RentAmount)
//                .HasPrecision(18, 2);

//            modelBuilder.Entity<Payment>()
//                .Property(p => p.Amount)
//                .HasPrecision(18, 2);

//            #endregion

//            RoleSeeder.SeedRoles(modelBuilder);
//        }

//        #endregion
//    }
//}