namespace PropertyManagementSystem.Application.DTOs
{
    public class PropertyOccupancyDto
    {
        public Guid PropertyId { get; set; }
        public string Address { get; set; } = null!;
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public decimal OccupancyRate { get; set; }
    }
}
