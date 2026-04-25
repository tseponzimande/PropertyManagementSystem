namespace PropertyManagementSystem.Application.DTOs
{
    public class PropertyStatsDto
    {
        public Guid PropertyId { get; set; }
        public string Address { get; set; } = null!;
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int AvailableUnits { get; set; }
        public decimal OccupancyRate { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int ActiveLeases { get; set; }
    }
}
