namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class UserRepository(PropertyManagementDbContext context) : Repository<User>(context), IUserRepository
    {
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId)
        {
            return await _context.Users
                .Where(u => u.RoleId == roleId)
                .ToListAsync();
        }
    }
}
