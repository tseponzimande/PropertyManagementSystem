namespace PropertyManagementSystem.Application.Services
{
    public class AnalyticsService(IUnitOfWork unitOfWork, ILogger<AnalyticsService> logger) : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<AnalyticsService> _logger = logger;

        public async Task<SystemAnalyticsDto> GetSystemAnalyticsAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var properties = await _unitOfWork.Properties.GetAllAsync();
                var units = await _unitOfWork.Units.GetAllAsync();
                var leases = await _unitOfWork.Leases.GetAllAsync();
                var payments = await _unitOfWork.Payments.GetAllAsync();
                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfYear = new DateTime(now.Year, 1, 1);

                var totalUnits = units.Count();
                var occupiedUnits = units.Count(u => u.Status == UnitEnum.Occupied.ToString());
                var avgOccupancy = totalUnits > 0 ? (decimal)occupiedUnits / totalUnits * 100 : 0;

                var resolvedMaintenance = maintenanceRequests
                    .Where(m => m.Status == MaintenanceRequestEnum.Resolved.ToString())
                    .ToList();

                var avgResolutionDays = resolvedMaintenance.Any()
                    ? resolvedMaintenance.Average(m => (DateTime.UtcNow - m.CreatedAt).TotalDays)
                    : 0;

                return new SystemAnalyticsDto
                {
                    TotalUsers = users.Count(),
                    ActiveUsers = users.Count(u => u.CreatedAt >= startOfMonth),
                    NewUsersThisMonth = users.Count(u => u.CreatedAt >= startOfMonth),
                    TotalProperties = properties.Count(),
                    TotalUnits = totalUnits,
                    AverageOccupancyRate = Math.Round(avgOccupancy, 2),
                    TotalRevenueThisMonth = payments
                        .Where(p => p.PaymentDate >= startOfMonth && p.Status == PaymentEnum.Completed.ToString())
                        .Sum(p => p.Amount),
                    TotalRevenueThisYear = payments
                        .Where(p => p.PaymentDate >= startOfYear && p.Status == PaymentEnum.Completed.ToString())
                        .Sum(p => p.Amount),
                    ActiveLeases = leases.Count(l => l.Status == LeaseEnum.Active.ToString()),
                    PendingMaintenanceRequests = maintenanceRequests
                        .Count(m => m.Status == MaintenanceRequestEnum.Open.ToString()),
                    AverageMaintenanceResolutionDays = Math.Round(avgResolutionDays, 2),
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system analytics");
                return new SystemAnalyticsDto { LastUpdated = DateTime.UtcNow };
            }
        }

        public async Task<UserActivityDto> GetUserActivityAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return new UserActivityDto { UserId = userId };
                }

                var messages = await _unitOfWork.Messages.GetAllAsync();
                var userMessages = messages.Where(m => m.SenderId == userId || m.ReceiverId == userId).ToList();
                var unreadMessages = messages.Count(m => m.ReceiverId == userId && !m.IsRead);

                var notifications = await _unitOfWork.Notifications.GetNotificationsByUserAsync(userId);
                var unreadNotifications = notifications.Count(n => !n.IsRead);

                var activityBreakdown = new Dictionary<string, int>();

                var sentMessages = userMessages.Count(m => m.SenderId == userId);
                var receivedMessages = userMessages.Count(m => m.ReceiverId == userId);

                activityBreakdown["Messages Sent"] = sentMessages;
                activityBreakdown["Messages Received"] = receivedMessages;
                activityBreakdown["Notifications Received"] = notifications.Count();

                if (user.Role.RoleType == RoleType.Tenant.ToString())
                {
                    var leases = await _unitOfWork.Leases.GetLeasesByTenantAsync(userId);
                    var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
                    var userMaintenance = maintenanceRequests.Where(m => m.TenantId == userId).ToList();

                    activityBreakdown["Active Leases"] = leases.Count(l => l.Status == LeaseEnum.Active.ToString());
                    activityBreakdown["Maintenance Requests"] = userMaintenance.Count;
                }
                else if (user.Role.RoleType == RoleType.Owner.ToString())
                {
                    var properties = await _unitOfWork.Properties.GetPropertiesByOwnerAsync(userId);
                    activityBreakdown["Properties Owned"] = properties.Count();
                    activityBreakdown["Approved Properties"] = properties.Count(p => p.Status == StatusEnum.Approved.ToString());
                }

                return new UserActivityDto
                {
                    UserId = userId,
                    UserName = user.Name,
                    Role = user.Role.RoleType,
                    TotalMessages = userMessages.Count,
                    UnreadMessages = unreadMessages,
                    TotalNotifications = notifications.Count(),
                    UnreadNotifications = unreadNotifications,
                    LastLoginDate = user.CreatedAt, 
                    ActivityBreakdown = activityBreakdown
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user activity for {UserId}", userId);
                return new UserActivityDto { UserId = userId };
            }
        }

        public async Task<PaymentTrendsDto> GetPaymentTrendsAsync(int months = 12)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAllAsync();
                var completedPayments = payments
                    .Where(p => p.Status == PaymentEnum.Completed.ToString())
                    .ToList();

                var cutoffDate = DateTime.UtcNow.AddMonths(-months);
                var recentPayments = completedPayments
                    .Where(p => p.PaymentDate >= cutoffDate)
                    .ToList();

                var monthlyTrends = recentPayments
                    .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                    .Select(g => new MonthlyTrendDto
                    {
                        Year = g.Key.Year,
                        MonthNumber = g.Key.Month,
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                        Revenue = g.Sum(p => p.Amount),
                        PaymentCount = g.Count(),
                        AveragePaymentAmount = g.Average(p => p.Amount)
                    })
                    .OrderByDescending(m => m.Year)
                    .ThenByDescending(m => m.MonthNumber)
                    .ToList();

                var totalRevenue = monthlyTrends.Sum(m => m.Revenue);
                var avgMonthlyRevenue = monthlyTrends.Any() ? totalRevenue / monthlyTrends.Count : 0;
                var highestMonth = monthlyTrends.OrderByDescending(m => m.Revenue).FirstOrDefault();
                var lowestMonth = monthlyTrends.OrderBy(m => m.Revenue).FirstOrDefault();

                var growthRate = 0.0;
                if (monthlyTrends.Count >= 2)
                {
                    var currentMonth = monthlyTrends[0].Revenue;
                    var previousMonth = monthlyTrends[1].Revenue;
                    if (previousMonth > 0)
                    {
                        growthRate = ((double)(currentMonth - previousMonth) / (double)previousMonth) * 100;
                    }
                }

                return new PaymentTrendsDto
                {
                    MonthlyTrends = monthlyTrends,
                    AverageMonthlyRevenue = avgMonthlyRevenue,
                    HighestMonthRevenue = highestMonth?.Revenue ?? 0,
                    LowestMonthRevenue = lowestMonth?.Revenue ?? 0,
                    HighestRevenueMonth = highestMonth?.Month ?? "N/A",
                    GrowthRate = Math.Round(growthRate, 2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment trends");
                return new PaymentTrendsDto();
            }
        }

        public async Task<MaintenanceTrendsDto> GetMaintenanceTrendsAsync(int months = 6)
        {
            try
            {
                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
                var cutoffDate = DateTime.UtcNow.AddMonths(-months);
                var recentRequests = maintenanceRequests
                    .Where(m => m.CreatedAt >= cutoffDate)
                    .ToList();

                var monthlyTrends = recentRequests
                    .GroupBy(m => new { m.CreatedAt.Year, m.CreatedAt.Month })
                    .Select(g =>
                    {
                        var resolvedInMonth = g.Where(m => m.Status == MaintenanceRequestEnum.Resolved.ToString()).ToList();
                        var avgResolution = resolvedInMonth.Any()
                            ? resolvedInMonth.Average(m => (DateTime.UtcNow - m.CreatedAt).TotalDays)
                            : 0;

                        return new MaintenanceTrendDto
                        {
                            Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                            TotalRequests = g.Count(),
                            OpenRequests = g.Count(m => m.Status == MaintenanceRequestEnum.Open.ToString()),
                            ResolvedRequests = resolvedInMonth.Count,
                            AverageResolutionDays = Math.Round(avgResolution, 2)
                        };
                    })
                    .OrderBy(m => m.Month)
                    .ToList();

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var thisMonthRequests = recentRequests.Where(m => m.CreatedAt >= startOfMonth).ToList();

                var resolvedRequests = maintenanceRequests
                    .Where(m => m.Status == MaintenanceRequestEnum.Resolved.ToString())
                    .ToList();

                var overallAvgResolution = resolvedRequests.Any()
                    ? resolvedRequests.Average(m => (DateTime.UtcNow - m.CreatedAt).TotalDays)
                    : 0;

                var totalRequests = maintenanceRequests.Count();
                var totalResolved = resolvedRequests.Count;
                var resolutionRate = totalRequests > 0 ? (double)totalResolved / totalRequests * 100 : 0;

                return new MaintenanceTrendsDto
                {
                    MonthlyTrends = monthlyTrends,
                    AverageResolutionTime = Math.Round(overallAvgResolution, 2),
                    TotalRequestsThisMonth = thisMonthRequests.Count,
                    ResolvedThisMonth = thisMonthRequests.Count(m => m.Status == MaintenanceRequestEnum.Resolved.ToString()),
                    ResolutionRate = Math.Round(resolutionRate, 2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance trends");
                return new MaintenanceTrendsDto();
            }
        }

        public async Task<GrowthMetricsDto> GetGrowthMetricsAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var properties = await _unitOfWork.Properties.GetAllAsync();
                var leases = await _unitOfWork.Leases.GetAllAsync();
                var payments = await _unitOfWork.Payments.GetAllAsync();
                var units = await _unitOfWork.Units.GetAllAsync();

                var now = DateTime.UtcNow;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfLastMonth = startOfMonth.AddMonths(-1);
                var startOfTwoMonthsAgo = startOfMonth.AddMonths(-2);

                int newPropertiesThisMonth =
                    properties.Count(p => p.CreatedAt >= startOfMonth);

                int newLeasesThisMonth =
                    leases.Count(l => l.CreatedAt >= startOfMonth);

                int newTenantsThisMonth =
                    users.Count(u =>
                        u.Role.RoleType == RoleType.Tenant.ToString() &&
                        u.CreatedAt >= startOfMonth);

                int usersLastMonth =
                    users.Count(u =>
                        u.CreatedAt >= startOfLastMonth &&
                        u.CreatedAt < startOfMonth);

                int usersTwoMonthsAgo =
                    users.Count(u =>
                        u.CreatedAt >= startOfTwoMonthsAgo &&
                        u.CreatedAt < startOfLastMonth);

                int propertiesLastMonth =
                    properties.Count(p =>
                        p.CreatedAt >= startOfLastMonth &&
                        p.CreatedAt < startOfMonth);

                int propertiesTwoMonthsAgo =
                    properties.Count(p =>
                        p.CreatedAt >= startOfTwoMonthsAgo &&
                        p.CreatedAt < startOfLastMonth);

                decimal revenueThisMonth =
                    payments
                        .Where(p =>
                            p.PaymentDate >= startOfMonth &&
                            p.Status == PaymentEnum.Completed.ToString())
                        .Sum(p => p.Amount);

                decimal revenueLastMonth =
                    payments
                        .Where(p =>
                            p.PaymentDate >= startOfLastMonth &&
                            p.PaymentDate < startOfMonth &&
                            p.Status == PaymentEnum.Completed.ToString())
                        .Sum(p => p.Amount);

                int occupiedUnitsThisMonth =
                    units.Count(u => u.Status == UnitEnum.Occupied.ToString());

                int totalUnits = units.Count();

                double occupancyThisMonth =
                    totalUnits > 0
                        ? (double)occupiedUnitsThisMonth / totalUnits * 100d
                        : 0d;

                double userGrowthRate =
                    usersTwoMonthsAgo > 0
                        ? ((double)(usersLastMonth - usersTwoMonthsAgo) / usersTwoMonthsAgo) * 100d
                        : 0d;

                double propertyGrowthRate =
                    propertiesTwoMonthsAgo > 0
                        ? ((double)(propertiesLastMonth - propertiesTwoMonthsAgo) / propertiesTwoMonthsAgo) * 100d
                        : 0d;

                double revenueGrowthRate =
                    revenueLastMonth > 0
                        ? ((double)(revenueThisMonth - revenueLastMonth) / (double)revenueLastMonth) * 100d
                        : 0d;

                var monthOverMonthGrowth = new Dictionary<string, double>
                {
                    ["Users"] = Math.Round(userGrowthRate, 2),
                    ["Properties"] = Math.Round(propertyGrowthRate, 2),
                    ["Revenue"] = Math.Round(revenueGrowthRate, 2),
                    ["Leases"] = Math.Round((double)newLeasesThisMonth, 2)
                };

                return new GrowthMetricsDto
                {
                    UserGrowthRate = Math.Round(userGrowthRate, 2),
                    PropertyGrowthRate = Math.Round(propertyGrowthRate, 2),
                    RevenueGrowthRate = Math.Round(revenueGrowthRate, 2),
                    OccupancyGrowthRate = Math.Round(occupancyThisMonth, 2),
                    NewPropertiesThisMonth = newPropertiesThisMonth,
                    NewLeasesThisMonth = newLeasesThisMonth,
                    NewTenantsThisMonth = newTenantsThisMonth,
                    MonthOverMonthGrowth = monthOverMonthGrowth
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting growth metrics");
                return new GrowthMetricsDto();
            }
        }

    }
}
