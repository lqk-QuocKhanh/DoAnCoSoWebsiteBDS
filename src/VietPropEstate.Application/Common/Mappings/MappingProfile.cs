using AutoMapper;
using VietPropEstate.Application.Features.Agents.DTOs;
using VietPropEstate.Application.Features.Customers.DTOs;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Application.Features.Transactions.DTOs;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Property list card ────────────────────────────────────────────────
        CreateMap<Property, PropertyDto>()
            .ForMember(d => d.Slug,           o => o.MapFrom(s => s.Slug))
            .ForMember(d => d.PriceAmount,    o => o.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.PriceCurrency,  o => o.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.FullAddress,    o => o.MapFrom(s => s.FullAddress ?? s.Address.FullAddress))
            .ForMember(d => d.ProvinceName,   o => o.MapFrom(s => s.ProvinceName))
            .ForMember(d => d.ProvinceCode,   o => o.MapFrom(s => s.ProvinceCode))
            .ForMember(d => d.WardName,       o => o.MapFrom(s => s.WardName))
            .ForMember(d => d.WardCode,       o => o.MapFrom(s => s.WardCode))
            .ForMember(d => d.Latitude,       o => o.MapFrom(s => s.Latitude ?? s.Address.Latitude))
            .ForMember(d => d.Longitude,      o => o.MapFrom(s => s.Longitude ?? s.Address.Longitude))
            .ForMember(d => d.PropertyTypeId,     o => o.MapFrom(s => s.PropertyTypeId))
            .ForMember(d => d.PropertyTypeName,   o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : null))
            .ForMember(d => d.TransactionTypeId,  o => o.MapFrom(s => s.TransactionTypeId))
            .ForMember(d => d.TransactionTypeName,o => o.MapFrom(s => s.TransactionType != null ? s.TransactionType.Name : null))
            .ForMember(d => d.AgentId,   o => o.MapFrom(s => s.AgentId))
            .ForMember(d => d.AgentName, o => o.MapFrom(s => s.Agent != null ? s.Agent.FullName : null))
            .ForMember(d => d.PrimaryImageUrl, o => o.MapFrom(s =>
                s.Images.OrderBy(i => i.IsPrimary ? 0 : 1).ThenBy(i => i.DisplayOrder).FirstOrDefault() != null
                    ? s.Images.OrderBy(i => i.IsPrimary ? 0 : 1).ThenBy(i => i.DisplayOrder).First().Url
                    : null));

        // ── Property detail page ──────────────────────────────────────────────
        CreateMap<Property, PropertyDetailDto>()
            .ForMember(d => d.Slug,           o => o.MapFrom(s => s.Slug))
            .ForMember(d => d.PriceAmount,    o => o.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.PriceCurrency,  o => o.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.Street,         o => o.MapFrom(s => s.Address.Street))
            .ForMember(d => d.AddressWard,    o => o.MapFrom(s => s.Address.Ward))
            .ForMember(d => d.AddressDistrict,o => o.MapFrom(s => s.Address.District))
            .ForMember(d => d.AddressProvince,o => o.MapFrom(s => s.Address.Province))
            .ForMember(d => d.FullAddress,    o => o.MapFrom(s => s.FullAddress ?? s.Address.FullAddress))
            .ForMember(d => d.ProvinceName,   o => o.MapFrom(s => s.ProvinceName))
            .ForMember(d => d.ProvinceCode,   o => o.MapFrom(s => s.ProvinceCode))
            .ForMember(d => d.WardName,       o => o.MapFrom(s => s.WardName))
            .ForMember(d => d.WardCode,       o => o.MapFrom(s => s.WardCode))
            .ForMember(d => d.Latitude,       o => o.MapFrom(s => s.Latitude ?? s.Address.Latitude))
            .ForMember(d => d.Longitude,      o => o.MapFrom(s => s.Longitude ?? s.Address.Longitude))
            .ForMember(d => d.PropertyTypeName,   o => o.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : null))
            .ForMember(d => d.TransactionTypeId,  o => o.MapFrom(s => s.TransactionTypeId))
            .ForMember(d => d.TransactionTypeName,o => o.MapFrom(s => s.TransactionType != null ? s.TransactionType.Name : null))
            .ForMember(d => d.AgentName,      o => o.MapFrom(s => s.Agent != null ? s.Agent.FullName : null))
            .ForMember(d => d.AgentPhone,     o => o.MapFrom(s => s.Agent != null ? s.Agent.PhoneNumber : null))
            .ForMember(d => d.AgentEmail,     o => o.MapFrom(s => s.Agent != null ? s.Agent.Email : null))
            .ForMember(d => d.AgentAvatarUrl, o => o.MapFrom(s => s.Agent != null ? s.Agent.AvatarUrl : null))
            .ForMember(d => d.Images,         o => o.MapFrom(s => s.Images));

        CreateMap<PropertyImage, PropertyImageDto>();
        CreateMap<PropertyType, PropertyTypeDto>();
        CreateMap<TransactionType, TransactionTypeDto>();

        // ── People ────────────────────────────────────────────────────────────
        CreateMap<Agent, AgentDto>();
        CreateMap<Agent, AgentDetailDto>();
        CreateMap<Customer, CustomerDto>();
        CreateMap<Customer, CustomerDetailDto>();

        // ── Transaction ───────────────────────────────────────────────────────
        CreateMap<Transaction, TransactionDto>()
            .ForMember(d => d.Amount,   o => o.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Amount.Currency));
    }
}
