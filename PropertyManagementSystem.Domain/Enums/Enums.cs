namespace PropertyManagementSystem.Domain.Enums
{
    public enum LeaseEnum
    { 
        Active,
        Expired,
        Terminated 
    }
    public enum MaintenanceRequestEnum
    { 
        Open,
        InProgress,
        Resolved
    }
    public enum NotificationEnum 
    {
        Payment,
        Maintenance,
        Lease,
        Message,
        Alert
    }
    public enum PaymentEnum 
    { 
        Payment,
        Pending,
        Completed 
    }
    public enum PropertyEnum
    { 
        Active, 
        Inactive,
        Pending
    }
    public enum RoleEnum
    { 
        Admin,
        Owner, 
        Tenant 
    }
    public enum RoleType
    { 
        Admin,
        Owner, 
        Tenant
    }
    public enum StatusEnum
    { 
        Approved,
        Reject, 
        Pending
    }
    public enum UnitEnum
    { 
        Available,
        Occupied 
    }
}