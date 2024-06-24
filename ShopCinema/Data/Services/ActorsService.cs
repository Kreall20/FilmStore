using Microsoft.EntityFrameworkCore;
using ShopCinema.Data;
using ShopCinema.Data.Base;
using ShopCinema.Data.Services;
using ShopCinema.Models;

namespace ShopCinema.Data.Services
{
    public class ActorsService : EntityBaseRepository<Actor>,IActorService
    {
        public ActorsService(AppDbContext context) : base(context) { }     
    }
}
