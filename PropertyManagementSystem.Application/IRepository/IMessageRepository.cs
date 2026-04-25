namespace PropertyManagementSystem.Application.IRepository
{
    public interface IMessageRepository: IRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesBySenderAsync(Guid senderId);
        Task<IEnumerable<Message>> GetMessagesByReceiverAsync(Guid receiverId);
    }
}
