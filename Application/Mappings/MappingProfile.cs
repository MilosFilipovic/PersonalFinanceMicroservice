using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Application.Mappings
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<TransactionDto, Transaction>()
                .ConstructUsing(dto => new Transaction(
                    dto.Id,
                    dto.Date,
                    dto.Direction,
                    dto.Amount,
                    dto.Currency,
                    dto.Kind,
                    (int?)dto.Mcc,
                    dto.BeneficiaryName,
                    dto.Description
                    
                    )).ForMember(
                dto => dto.CatCode,
                opt => opt.MapFrom(src => src.CatCode)
            );

            CreateMap<Transaction, TransactionDto>();

            CreateMap<TransactionSplit, SplitItemDto>();
            CreateMap<Transaction, TransactionDto>()
                .ForMember(d => d.Splits,
                           opt => opt.MapFrom(s => s.Splits));

            CreateMap<Category, CategoryDto>();
        }
    }
}
