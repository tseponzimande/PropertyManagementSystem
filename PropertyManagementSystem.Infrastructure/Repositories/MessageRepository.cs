namespace PropertyManagementSystem.Infrastructure.Repositories
{
    public class MessageRepository(PropertyManagementDbContext context) : Repository<Message>(context), IMessageRepository
    {
        public async Task<IEnumerable<Message>> GetMessagesBySenderAsync(Guid senderId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == senderId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByReceiverAsync(Guid receiverId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.ReceiverId == receiverId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
