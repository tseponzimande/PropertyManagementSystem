namespace PropertyManagementSystem.Application.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId);
    }
}
