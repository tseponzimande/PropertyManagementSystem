namespace PropertyManagementSystem.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.RoleType))
            .ReverseMap()
            .ForMember(dest => dest.Role,
                opt => opt.Ignore());

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