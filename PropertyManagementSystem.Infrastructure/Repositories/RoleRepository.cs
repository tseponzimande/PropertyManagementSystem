namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class RoleRepository(PropertyManagementDbContext context) : Repository<Role>(context), IRoleRepository
    {
        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleType.ToLower() == roleName.ToLower());
        }
    }
}
