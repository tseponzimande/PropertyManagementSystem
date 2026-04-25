namespace PropertyManagementSystem.Application.IRepository
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}
