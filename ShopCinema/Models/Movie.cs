using ShopCinema.Data.Base;
using ShopCinema.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopCinema.Models
{
    public class Movie: IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public MovieCategory Category { get; set; }
        public string Description { get; set; }
        public List<Actor_Movie> Actors_Movies { get; set; }
        public int countofMovie { get; set; }

        public int ProducerId { get; set; }
        [ForeignKey("ProducerId")]
        public Producer Producer { get; set; }
    }
}
