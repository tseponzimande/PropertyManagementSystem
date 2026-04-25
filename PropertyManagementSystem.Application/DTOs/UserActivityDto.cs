namespace PropertyManagementSystem.Application.DTOs
{
    public class UserActivityDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int TotalMessages { get; set; }
        public int UnreadMessages { get; set; }
        public int TotalNotifications { get; set; }
        public int UnreadNotifications { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public Dictionary<string, int> ActivityBreakdown { get; set; } = new();
    }
}
