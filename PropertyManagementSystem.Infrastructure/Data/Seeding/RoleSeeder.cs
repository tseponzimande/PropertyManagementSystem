namespace PropertyManagementSystem.Infrastructure.Data.Seeding
{
    public static class RoleSeeder
    {
        public static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    RoleType = RoleType.Admin.ToString(),
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2024-01-01T00:00:00Z"), DateTimeKind.Utc)
                },
                new Role
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    RoleType = RoleType.Owner.ToString(),
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2024-01-01T00:00:00Z"), DateTimeKind.Utc)
                },
                new Role
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    RoleType = RoleType.Tenant.ToString(),
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2024-01-01T00:00:00Z"), DateTimeKind.Utc)
                }
            );
        }
    }
}
