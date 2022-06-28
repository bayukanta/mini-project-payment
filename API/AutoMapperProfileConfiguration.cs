using AutoMapper;
using API.DTO;
using DAL.Model;


namespace API
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<PaymentDTO, Payments>();
            CreateMap<Payments, PaymentDTO>();

        }
    }
}
