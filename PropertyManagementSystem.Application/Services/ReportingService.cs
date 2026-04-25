namespace PropertyManagementSystem.Application.Services
{
    public class ReportingService(IUnitOfWork unitOfWork, ILogger<ReportingService> logger) : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReportingService> _logger = logger;

        public async Task<FinancialReportDto> GenerateFinancialReportAsync(DateTime startDate, DateTime endDate, Guid? ownerId = null)
        {
            try
            {
                var payments = await _unitOfWork.Payments.GetAllAsync();
                var filteredPayments = payments.Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate).ToList();

                if (ownerId.HasValue)
                {
                    var ownerProperties = await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId.Value);
                    var propertyIds = ownerProperties.Select(p => p.Id).ToList();
                    var allUnits = await _unitOfWork.Units.GetAllAsync();
                    var ownerUnitIds = allUnits.Where(u => propertyIds.Contains(u.PropertyId)).Select(u => u.Id).ToList();
                    var allLeases = await _unitOfWork.Leases.GetAllAsync();
                    var ownerLeaseIds = allLeases.Where(l => ownerUnitIds.Contains(l.UnitId)).Select(l => l.Id).ToList();
                    filteredPayments = filteredPayments.Where(p => ownerLeaseIds.Contains(p.LeaseId)).ToList();
                }

                var completedPayments = filteredPayments.Where(p => p.Status == PaymentEnum.Completed.ToString()).ToList();
                var pendingPayments = filteredPayments.Where(p => p.Status == PaymentEnum.Pending.ToString()).ToList();

                var paymentBreakdown = completedPayments
                    .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                    .Select(g => new PaymentBreakdownDto
                    {
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                        Amount = g.Sum(p => p.Amount),
                        PaymentCount = g.Count()
                    })
                    .OrderBy(p => p.Month)
                    .ToList();

                var reportProperties = ownerId.HasValue ? await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId.Value) : await _unitOfWork.Properties.GetAllAsync();
                var allUnitsList = await _unitOfWork.Units.GetAllAsync();
                var allLeasesList = await _unitOfWork.Leases.GetAllAsync();

                var propertyRevenue = new List<PropertyRevenueDto>();
                foreach (var property in reportProperties)
                {
                    var propertyUnitIds = allUnitsList.Where(u => u.PropertyId == property.Id).Select(u => u.Id).ToList();
                    var propertyLeaseIds = allLeasesList.Where(l => propertyUnitIds.Contains(l.UnitId)).Select(l => l.Id).ToList();
                    var propertyPayments = completedPayments.Where(p => propertyLeaseIds.Contains(p.LeaseId)).ToList();

                    if (propertyPayments.Any())
                    {
                        propertyRevenue.Add(new PropertyRevenueDto
                        {
                            PropertyId = property.Id,
                            Address = property.Address,
                            Revenue = propertyPayments.Sum(p => p.Amount),
                            PaymentCount = propertyPayments.Count
                        });
                    }
                }

                return new FinancialReportDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalRevenue = completedPayments.Sum(p => p.Amount),
                    TotalPending = pendingPayments.Sum(p => p.Amount),
                    TotalPayments = completedPayments.Count,
                    PendingPayments = pendingPayments.Count,
                    PaymentBreakdown = paymentBreakdown,
                    PropertyRevenue = propertyRevenue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating financial report");
                return new FinancialReportDto { StartDate = startDate, EndDate = endDate };
            }
        }

        public async Task<OccupancyReportDto> GenerateOccupancyReportAsync(Guid? propertyId = null)
        {
            try
            {
                var reportProperties = propertyId.HasValue
                    ? new[] { await _unitOfWork.Properties.GetByIdAsync(propertyId.Value) }.Where(p => p != null).ToList()
                    : (await _unitOfWork.Properties.GetAllAsync()).ToList();

                var allUnits = await _unitOfWork.Units.GetAllAsync();
                var propertyOccupancy = new List<PropertyOccupancyDto>();

                foreach (var property in reportProperties)
                {
                    var propertyUnits = allUnits.Where(u => u.PropertyId == property.Id).ToList();
                    var totalUnits = propertyUnits.Count;
                    var occupiedUnits = propertyUnits.Count(u => u.Status == UnitEnum.Occupied.ToString());
                    var occupancyRate = totalUnits > 0 ? (decimal)occupiedUnits / totalUnits * 100 : 0;

                    propertyOccupancy.Add(new PropertyOccupancyDto
                    {
                        PropertyId = property.Id,
                        Address = property.Address,
                        TotalUnits = totalUnits,
                        OccupiedUnits = occupiedUnits,
                        OccupancyRate = Math.Round(occupancyRate, 2)
                    });
                }

                var overallTotalUnits = propertyOccupancy.Sum(p => p.TotalUnits);
                var totalOccupied = propertyOccupancy.Sum(p => p.OccupiedUnits);
                var overallRate = overallTotalUnits > 0 ? (decimal)totalOccupied / overallTotalUnits * 100 : 0;

                return new OccupancyReportDto
                {
                    TotalUnits = overallTotalUnits,
                    OccupiedUnits = totalOccupied,
                    AvailableUnits = overallTotalUnits - totalOccupied,
                    OccupancyRate = Math.Round(overallRate, 2),
                    PropertyOccupancy = propertyOccupancy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating occupancy report");
                return new OccupancyReportDto();
            }
        }

        public async Task<MaintenanceReportDto> GenerateMaintenanceReportAsync(DateTime startDate, DateTime endDate, Guid? ownerId = null)
        {
            try
            {
                var maintenanceRequests = await _unitOfWork.MaintenanceRequests.GetAllAsync();
                var filteredRequests = maintenanceRequests.Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate).ToList();

                if (ownerId.HasValue)
                {
                    var ownerProperties = await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId.Value);
                    var propertyIds = ownerProperties.Select(p => p.Id).ToList();
                    var allUnits = await _unitOfWork.Units.GetAllAsync();
                    var ownerUnitIds = allUnits.Where(u => propertyIds.Contains(u.PropertyId)).Select(u => u.Id).ToList();
                    filteredRequests = filteredRequests.Where(m => ownerUnitIds.Contains(m.UnitId)).ToList();
                }

                var resolvedRequests = filteredRequests.Where(m => m.Status == MaintenanceRequestEnum.Resolved.ToString()).ToList();
                var avgResolutionDays = resolvedRequests.Any() ? resolvedRequests.Average(m => (DateTime.UtcNow - m.CreatedAt).TotalDays) : 0;

                var reportProperties = ownerId.HasValue ? await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId.Value) : await _unitOfWork.Properties.GetAllAsync();
                var allUnitsList = await _unitOfWork.Units.GetAllAsync();
                var breakdown = new List<MaintenanceBreakdownDto>();

                foreach (var property in reportProperties)
                {
                    var propertyUnitIds = allUnitsList.Where(u => u.PropertyId == property.Id).Select(u => u.Id).ToList();
                    var propertyRequests = filteredRequests.Where(m => propertyUnitIds.Contains(m.UnitId)).ToList();

                    if (propertyRequests.Any())
                    {
                        breakdown.Add(new MaintenanceBreakdownDto
                        {
                            PropertyId = property.Id,
                            PropertyAddress = property.Address,
                            TotalRequests = propertyRequests.Count,
                            OpenRequests = propertyRequests.Count(m => m.Status == MaintenanceRequestEnum.Open.ToString()),
                            ResolvedRequests = propertyRequests.Count(m => m.Status == MaintenanceRequestEnum.Resolved.ToString())
                        });
                    }
                }

                return new MaintenanceReportDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalRequests = filteredRequests.Count,
                    OpenRequests = filteredRequests.Count(m => m.Status == MaintenanceRequestEnum.Open.ToString()),
                    InProgressRequests = filteredRequests.Count(m => m.Status == MaintenanceRequestEnum.InProgress.ToString()),
                    ResolvedRequests = resolvedRequests.Count,
                    AverageResolutionDays = Math.Round(avgResolutionDays, 2),
                    Breakdown = breakdown
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating maintenance report");
                return new MaintenanceReportDto { StartDate = startDate, EndDate = endDate };
            }
        }

        public async Task<TenantReportDto> GenerateTenantReportAsync(Guid? ownerId = null)
        {
            try
            {
                var leases = await _unitOfWork.Leases.GetAllAsync();
                var units = await _unitOfWork.Units.GetAllAsync();
                var properties = await _unitOfWork.Properties.GetAllAsync();
                var users = await _unitOfWork.Users.GetAllAsync();

                if (ownerId.HasValue)
                {
                    var ownerProperties = properties.Where(p => p.OwnerId == ownerId.Value).Select(p => p.Id).ToList();
                    units = units.Where(u => ownerProperties.Contains(u.PropertyId)).ToList();
                    leases = leases.Where(l => units.Select(u => u.Id).Contains(l.UnitId)).ToList();
                }

                var tenantDetails = new List<TenantDetailDto>();
                foreach (var lease in leases)
                {
                    var tenant = users.FirstOrDefault(u => u.Id == lease.TenantId);
                    var unit = units.FirstOrDefault(u => u.Id == lease.UnitId);
                    var property = unit != null ? properties.FirstOrDefault(p => p.Id == unit.PropertyId) : null;

                    if (tenant != null && unit != null && property != null)
                    {
                        tenantDetails.Add(new TenantDetailDto
                        {
                            TenantId = tenant.Id,
                            Name = tenant.Name,
                            Email = tenant.Email,
                            PropertyAddress = property.Address,
                            UnitNumber = unit.UnitNumber,
                            LeaseStartDate = lease.StartDate,
                            LeaseEndDate = lease.EndDate,
                            LeaseStatus = lease.Status,
                            RentAmount = lease.RentAmount
                        });
                    }
                }

                return new TenantReportDto
                {
                    TotalTenants = leases.Select(l => l.TenantId).Distinct().Count(),
                    ActiveTenants = leases.Count(l => l.Status == LeaseEnum.Active.ToString()),
                    TenantsWithExpiredLeases = leases.Count(l => l.Status == LeaseEnum.Expired.ToString()),
                    Tenants = tenantDetails
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tenant report");
                return new TenantReportDto();
            }
        }

        public async Task<LeaseExpiryReportDto> GenerateLeaseExpiryReportAsync(int daysAhead = 90)
        {
            try
            {
                var leases = await _unitOfWork.Leases.GetAllAsync();
                var activeLeases = leases.Where(l => l.Status == LeaseEnum.Active.ToString()).ToList();
                var today = DateTime.UtcNow;
                var futureDate = today.AddDays(daysAhead);

                var expiringLeases = activeLeases
                    .Where(l => l.EndDate >= today && l.EndDate <= futureDate)
                    .ToList();

                var units = await _unitOfWork.Units.GetAllAsync();
                var properties = await _unitOfWork.Properties.GetAllAsync();
                var users = await _unitOfWork.Users.GetAllAsync();

                var expiringLeaseDetails = new List<ExpiringLeaseDto>();
                foreach (var lease in expiringLeases)
                {
                    var tenant = users.FirstOrDefault(u => u.Id == lease.TenantId);
                    var unit = units.FirstOrDefault(u => u.Id == lease.UnitId);
                    var property = unit != null ? properties.FirstOrDefault(p => p.Id == unit.PropertyId) : null;

                    if (tenant != null && unit != null && property != null)
                    {
                        expiringLeaseDetails.Add(new ExpiringLeaseDto
                        {
                            LeaseId = lease.Id,
                            TenantId = tenant.Id,
                            TenantName = tenant.Name,
                            PropertyAddress = property.Address,
                            UnitNumber = unit.UnitNumber,
                            ExpiryDate = lease.EndDate,
                            DaysUntilExpiry = (lease.EndDate - today).Days
                        });
                    }
                }

                return new LeaseExpiryReportDto
                {
                    DaysAhead = daysAhead,
                    ExpiringLeases = expiringLeaseDetails.Count,
                    Leases = expiringLeaseDetails.OrderBy(l => l.DaysUntilExpiry).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating lease expiry report");
                return new LeaseExpiryReportDto { DaysAhead = daysAhead };
            }
        }

        public async Task<byte[]> ExportFinancialReportToCsvAsync(DateTime startDate, DateTime endDate, Guid? ownerId = null)
        {
            try
            {
                var report = await GenerateFinancialReportAsync(startDate, endDate, ownerId);

                var csv = new StringBuilder();
                csv.AppendLine("Financial Report");
                csv.AppendLine($"Period,{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                csv.AppendLine($"Total Revenue,${report.TotalRevenue:N2}");
                csv.AppendLine($"Total Pending,${report.TotalPending:N2}");
                csv.AppendLine($"Total Payments,{report.TotalPayments}");
                csv.AppendLine($"Pending Payments,{report.PendingPayments}");
                csv.AppendLine();
                csv.AppendLine("Monthly Breakdown");
                csv.AppendLine("Month,Amount,Payment Count");

                foreach (var item in report.PaymentBreakdown)
                {
                    csv.AppendLine($"{item.Month},${item.Amount:N2},{item.PaymentCount}");
                }

                csv.AppendLine();
                csv.AppendLine("Property Revenue");
                csv.AppendLine("Property Address,Revenue,Payment Count");

                foreach (var item in report.PropertyRevenue)
                {
                    csv.AppendLine($"{item.Address},${item.Revenue:N2},{item.PaymentCount}");
                }

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting financial report to CSV");
                return Array.Empty<byte>();
            }
        }
    }
}