namespace PropertyManagementSystem.Application.DTOs
{
    public class OccupancyReportDto
    {
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int AvailableUnits { get; set; }
        public decimal OccupancyRate { get; set; }
        public List<PropertyOccupancyDto> PropertyOccupancy { get; set; } = new();
    }
}
