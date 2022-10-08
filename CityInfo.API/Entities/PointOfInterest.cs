using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // annotation is not necessary but recommended to be explicit
        // or when not following convention to name foreign key by class name followed by Id
        // in this is case we do follow convention so not necessary
        // states the foreign key for nav prop in Point Of Interest
        [ForeignKey("CityId")]
        // we want to navigate from point of interest to parent city
        // so we need a property to refer to that parent city
        // and state foreign key property
        // by convention: can add a navigation property to create relationship (a prop that is not a scalar)
        // it will target the primary key of navigation prop so the Id prop in City which will be our foreign key
        // the foreign key by convention will be named by class named appended with Id so CityId
        public City? City { get; set; }

        // dont have to define foreign key explicitily but recommended
        public int CityId { get; set; }

        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
