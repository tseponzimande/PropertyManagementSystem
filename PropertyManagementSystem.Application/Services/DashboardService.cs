namespace PropertyManagementSystem.Application.Services
{
    public class DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger) : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DashboardService> _logger = logger;

        public async Task<DashboardStatsDto> GetAdminDashboardStatsAsync()
        {
            try
            {
                var properties = await _unitOfWork.Properties.GetAllAsync();
                var units = await _unitOfWork.Units.GetAllAsync();
                var leases = await _unitOfWork.Leases.GetAllAsync();
                var users = await _unitOfWork.Users.GetAllAsync();
                var roles = await _unitOfWork.Roles.GetAllAsync();

                var tenantRoleId = roles
                    .FirstOrDefault(r => r.RoleType == RoleType.Tenant.ToString())?.Id;

                var ownerRoleId = roles
                    .FirstOrDefault(r => r.RoleType == RoleType.Owner.ToString())?.Id;

                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
                var payments = await _unitOfWork.Payments.GetAllAsync();
                var messages = await _unitOfWork.Messages.GetAllAsync();
                var notifications = await _unitOfWork.Notifications.GetAllAsync();

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfYear = new DateTime(now.Year, 1, 1);

                return new DashboardStatsDto
                {
                    TotalProperties = properties.Count(),
                    TotalUnits = units.Count(),
                    AvailableUnits = units.Count(u => u.Status == UnitEnum.Available.ToString()),
                    OccupiedUnits = units.Count(u => u.Status == UnitEnum.Occupied.ToString()),
                    TotalLeases = leases.Count(),
                    ActiveLeases = leases.Count(l => l.Status == LeaseEnum.Active.ToString()),
                    ExpiredLeases = leases.Count(l => l.Status == LeaseEnum.Expired.ToString()),             

                    TotalTenants = tenantRoleId.HasValue ? users.Count(u => u.RoleId == tenantRoleId.Value) : 0,

                    TotalOwners = ownerRoleId.HasValue ? users.Count(u => u.RoleId == ownerRoleId.Value) : 0,


                    PendingMaintenanceRequests = maintenanceRequests.Count(m => m.Status == MaintenanceRequestEnum.Open.ToString()),
                    InProgressMaintenanceRequests = maintenanceRequests.Count(m => m.Status == MaintenanceRequestEnum.InProgress.ToString()),
                    ResolvedMaintenanceRequests = maintenanceRequests.Count(m => m.Status == MaintenanceRequestEnum.Resolved.ToString()),
                    TotalRevenueThisMonth = payments.Where(p => p.PaymentDate >= startOfMonth && p.Status == PaymentEnum.Completed.ToString()).Sum(p => p.Amount),
                    TotalRevenueThisYear = payments.Where(p => p.PaymentDate >= startOfYear && p.Status == PaymentEnum.Completed.ToString()).Sum(p => p.Amount),
                    PendingPropertyApprovals = properties.Count(p => p.Status == StatusEnum.Pending.ToString()),
                    UnreadMessages = messages.Count(m => !m.IsRead),
                    UnreadNotifications = notifications.Count(n => !n.IsRead),
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard stats");
                return new DashboardStatsDto { LastUpdated = DateTime.UtcNow };
            }
        }

        public async Task<DashboardStatsDto> GetOwnerDashboardStatsAsync(Guid ownerId)
        {
            try
            {
                var properties = await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId);
                var propertyIds = properties.Select(p => p.Id).ToList();

                var units = await _unitOfWork.Units.GetAllAsync();
                var ownerUnits = units.Where(u => propertyIds.Contains(u.PropertyId)).ToList();
                var unitIds = ownerUnits.Select(u => u.Id).ToList();

                var leases = await _unitOfWork.Leases.GetAllAsync();
                var ownerLeases = leases.Where(l => unitIds.Contains(l.UnitId)).ToList();

                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
                var ownerMaintenance = maintenanceRequests.Where(m => unitIds.Contains(m.UnitId)).ToList();

                var payments = await _unitOfWork.Payments.GetAllAsync();
                var ownerPayments = payments.Where(p => ownerLeases.Select(l => l.Id).Contains(p.LeaseId)).ToList();

                var messages = await _unitOfWork.Messages.GetAllAsync();
                var unreadMessages = messages.Count(m => m.ReceiverId == ownerId && !m.IsRead);

                var notifications = await _unitOfWork.Notifications.GetNotificationsByUserAsync(ownerId);

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfYear = new DateTime(now.Year, 1, 1);

                return new DashboardStatsDto
                {
                    TotalProperties = properties.Count(),
                    TotalUnits = ownerUnits.Count,
                    AvailableUnits = ownerUnits.Count(u => u.Status == UnitEnum.Available.ToString()),
                    OccupiedUnits = ownerUnits.Count(u => u.Status == UnitEnum.Occupied.ToString()),
                    TotalLeases = ownerLeases.Count,
                    ActiveLeases = ownerLeases.Count(l => l.Status == LeaseEnum.Active.ToString()),
                    ExpiredLeases = ownerLeases.Count(l => l.Status == LeaseEnum.Expired.ToString()),
                    PendingMaintenanceRequests = ownerMaintenance.Count(m => m.Status == MaintenanceRequestEnum.Open.ToString()),
                    InProgressMaintenanceRequests = ownerMaintenance.Count(m => m.Status == MaintenanceRequestEnum.InProgress.ToString()),
                    ResolvedMaintenanceRequests = ownerMaintenance.Count(m => m.Status == MaintenanceRequestEnum.Resolved.ToString()),
                    TotalRevenueThisMonth = ownerPayments.Where(p => p.PaymentDate >= startOfMonth && p.Status == PaymentEnum.Completed.ToString()).Sum(p => p.Amount),
                    TotalRevenueThisYear = ownerPayments.Where(p => p.PaymentDate >= startOfYear && p.Status == PaymentEnum.Completed.ToString()).Sum(p => p.Amount),
                    UnreadMessages = unreadMessages,
                    UnreadNotifications = notifications.Count(n => !n.IsRead),
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting owner dashboard stats for {OwnerId}", ownerId);
                return new DashboardStatsDto { LastUpdated = DateTime.UtcNow };
            }
        }

        public async Task<DashboardStatsDto> GetTenantDashboardStatsAsync(Guid tenantId)
        {
            try
            {
                var leases = await _unitOfWork.Leases.GetLeasesByTenantAsync(tenantId);
                var activeLease = leases.FirstOrDefault(l => l.Status == LeaseEnum.Active.ToString());

                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
                var tenantMaintenance = maintenanceRequests.Where(m => m.TenantId == tenantId).ToList();

                var payments = await _unitOfWork.Payments.GetAllAsync();
                var tenantPayments = payments.Where(p => leases.Select(l => l.Id).Contains(p.LeaseId)).ToList();

                var messages = await _unitOfWork.Messages.GetAllAsync();
                var unreadMessages = messages.Count(m => m.ReceiverId == tenantId && !m.IsRead);

                var notifications = await _unitOfWork.Notifications.GetNotificationsByUserAsync(tenantId);

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfYear = new DateTime(now.Year, 1, 1);

                return new DashboardStatsDto
                {
                    TotalLeases = leases.Count(),
                    ActiveLeases = leases.Count(l => l.Status == LeaseEnum.Active.ToString()),
                    ExpiredLeases = leases.Count(l => l.Status == LeaseEnum.Expired.ToString()),
                    PendingMaintenanceRequests = tenantMaintenance.Count(m => m.Status == MaintenanceRequestEnum.Open.ToString()),
                    InProgressMaintenanceRequests = tenantMaintenance.Count(m => m.Status == MaintenanceRequestEnum.InProgress.ToString()),
                    ResolvedMaintenanceRequests = tenantMaintenance.Count(m => m.Status == MaintenanceRequestEnum.Resolved.ToString()),
                    TotalRevenueThisMonth = tenantPayments.Where(p => p.PaymentDate >= startOfMonth && p.Status == PaymentEnum.Completed.ToString()).Sum(p => p.Amount),
                    TotalRevenueThisYear = tenantPayments.Where(p => p.PaymentDate >= startOfYear && p.Status == PaymentEnum.Completed.ToString()).Sum(p => p.Amount),
                    UnreadMessages = unreadMessages,
                    UnreadNotifications = notifications.Count(n => !n.IsRead),
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tenant dashboard stats for {TenantId}", tenantId);
                return new DashboardStatsDto { LastUpdated = DateTime.UtcNow };
            }
        }

        public async Task<RevenueStatsDto> GetRevenueStatsAsync(Guid? ownerId = null)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAllAsync();
                var completedPayments = payments.Where(p => p.Status == PaymentEnum.Completed.ToString()).ToList();

                if (ownerId.HasValue)
                {
                    var properties = await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId.Value);
                    var propertyIds = properties.Select(p => p.Id).ToList();
                    var units = await _unitOfWork.Units.GetAllAsync();
                    var ownerUnitIds = units.Where(u => propertyIds.Contains(u.PropertyId)).Select(u => u.Id).ToList();
                    var leases = await _unitOfWork.Leases.GetAllAsync();
                    var ownerLeaseIds = leases.Where(l => ownerUnitIds.Contains(l.UnitId)).Select(l => l.Id).ToList();
                    completedPayments = completedPayments.Where(p => ownerLeaseIds.Contains(p.LeaseId)).ToList();
                }

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfLastMonth = startOfMonth.AddMonths(-1);
                var startOfYear = new DateTime(now.Year, 1, 1);

                var monthlyBreakdown = completedPayments
                    .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                    .Select(g => new MonthlyRevenueDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                        Revenue = g.Sum(p => p.Amount),
                        PaymentCount = g.Count()
                    })
                    .OrderByDescending(m => m.Year)
                    .ThenByDescending(m => m.Month)
                    .Take(12)
                    .ToList();

                var totalMonths = monthlyBreakdown.Count > 0 ? monthlyBreakdown.Count : 1;

                return new RevenueStatsDto
                {
                    TotalRevenue = completedPayments.Sum(p => p.Amount),
                    ThisMonthRevenue = completedPayments.Where(p => p.PaymentDate >= startOfMonth).Sum(p => p.Amount),
                    LastMonthRevenue = completedPayments.Where(p => p.PaymentDate >= startOfLastMonth && p.PaymentDate < startOfMonth).Sum(p => p.Amount),
                    ThisYearRevenue = completedPayments.Where(p => p.PaymentDate >= startOfYear).Sum(p => p.Amount),
                    AverageMonthlyRevenue = completedPayments.Sum(p => p.Amount) / totalMonths,
                    MonthlyBreakdown = monthlyBreakdown
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue stats");
                return new RevenueStatsDto();
            }
        }

        public async Task<IEnumerable<PropertyStatsDto>> GetPropertyStatsAsync(Guid? ownerId = null)
        {
            try
            {
                var properties = ownerId.HasValue
                    ? await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId.Value)
                    : await _unitOfWork.Properties.GetAllAsync();

                var units = await _unitOfWork.Units.GetAllAsync();
                var leases = await _unitOfWork.Leases.GetAllAsync();
                var payments = await _unitOfWork.Payments.GetAllAsync();

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);

                var propertyStats = new List<PropertyStatsDto>();

                foreach (var property in properties)
                {
                    var propertyUnits = units.Where(u => u.PropertyId == property.Id).ToList();
                    var unitIds = propertyUnits.Select(u => u.Id).ToList();
                    var propertyLeases = leases.Where(l => unitIds.Contains(l.UnitId)).ToList();
                    var activeLeases = propertyLeases.Where(l => l.Status == LeaseEnum.Active.ToString()).ToList();
                    var leaseIds = propertyLeases.Select(l => l.Id).ToList();
                    var monthlyPayments = payments.Where(p => leaseIds.Contains(p.LeaseId) &&
                        p.PaymentDate >= startOfMonth && p.Status == PaymentEnum.Completed.ToString());

                    var totalUnits = propertyUnits.Count;
                    var occupiedUnits = propertyUnits.Count(u => u.Status == UnitEnum.Occupied.ToString());
                    var occupancyRate = totalUnits > 0 ? (decimal)occupiedUnits / totalUnits * 100 : 0;

                    propertyStats.Add(new PropertyStatsDto
                    {
                        PropertyId = property.Id,
                        Address = property.Address,
                        TotalUnits = totalUnits,
                        OccupiedUnits = occupiedUnits,
                        AvailableUnits = propertyUnits.Count(u => u.Status == UnitEnum.Available.ToString()),
                        OccupancyRate = Math.Round(occupancyRate, 2),
                        MonthlyRevenue = monthlyPayments.Sum(p => p.Amount),
                        ActiveLeases = activeLeases.Count
                    });
                }

                return propertyStats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting property stats");
                return Enumerable.Empty<PropertyStatsDto>();
            }
        }

        public async Task<IEnumerable<MaintenanceStatsDto>> GetMaintenanceStatsAsync()
        {
            try
            {
                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();

                var stats = maintenanceRequests
                    .GroupBy(m => m.Status)
                    .Select(g => new MaintenanceStatsDto
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        AverageResolutionDays = g.Where(m => m.Status == MaintenanceRequestEnum.Resolved.ToString())
                            .Select(m => (DateTime.UtcNow - m.CreatedAt).TotalDays)
                            .DefaultIfEmpty(0)
                            .Average()
                    })
                    .ToList();

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance stats");
                return Enumerable.Empty<MaintenanceStatsDto>();
            }
        }
    }
}