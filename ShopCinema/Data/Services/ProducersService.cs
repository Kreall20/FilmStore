using ShopCinema.Data;
using ShopCinema.Data.Base;
using ShopCinema.Data.Services;
using ShopCinema.Models;

namespace ShopCinema.Data.Services
{
    public class ProducersService : EntityBaseRepository<Producer>, IProducersService
    {
        public ProducersService(AppDbContext context) : base(context) { }
    }
}
