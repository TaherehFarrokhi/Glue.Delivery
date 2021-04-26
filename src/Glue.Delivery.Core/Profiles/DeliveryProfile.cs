using AutoMapper;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;

namespace Glue.Delivery.Core.Profiles
{
    public sealed class DeliveryProfile : Profile
    {
        public DeliveryProfile()
        {
            CreateMap<NewDeliveryDto, OrderDelivery>(MemberList.Source);
            CreateMap<OrderDto, Order>(MemberList.Destination).ReverseMap();
            CreateMap<AccessWindowDto, AccessWindow>(MemberList.Destination).ReverseMap();
            CreateMap<RecipientDto, Recipient>(MemberList.Destination).ReverseMap();
            CreateMap<OrderDelivery, OrderDeliveryDto>(MemberList.Destination);
        }
    }
}