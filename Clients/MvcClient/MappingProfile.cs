using AutoMapper;
using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Models.Dto;

namespace BookOnline.MvcClient
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Borrow, BorrowDto>();
            CreateMap<BookItemDto, BookItemDto>();
            CreateMap<BorrowItem, BorrowItemDto>();
        }
    }
}
