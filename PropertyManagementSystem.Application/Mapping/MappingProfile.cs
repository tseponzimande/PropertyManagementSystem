namespace PropertyManagementSystem.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Property, PropertyDto>().ReverseMap();

            CreateMap<Unit, UnitDto>().ReverseMap();

            CreateMap<Lease, LeaseDto>().ReverseMap();

            CreateMap<Payment, PaymentDto>().ReverseMap();

            CreateMap<MaintenanceRequest, MaintenanceRequestDto>().ReverseMap();

            CreateMap<Message, MessageDto>().ReverseMap();

            CreateMap<Notification, NotificationDto>().ReverseMap();


        }
    }
}