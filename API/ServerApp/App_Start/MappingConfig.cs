using AltaBO;
using AutoMapper;

namespace ServerApp.App_Start
{
    public static class MappingConfig
    {
        public static void Register() { }

        public static IMapper CreateSupplierMapper() { 
            var mapperForSupplier = new MapperConfiguration(cfg => {
                var auctionMap = cfg.CreateMap<Auction, Auction>();
                auctionMap.ForMember(desc => desc.Procuratories, opt => opt.Ignore());
                auctionMap.ForMember(desc => desc.Comments, opt => opt.Ignore());
                auctionMap.ForMember(desc => desc.Applicants, opt => opt.Ignore());
                auctionMap.ForMember(desc => desc.SupplierOrders, opt => opt.Ignore());
            });

            return mapperForSupplier.CreateMapper();
        }
    }
}