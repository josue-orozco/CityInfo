using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    // want to have annotations at the lowest level to maintain data integrity
    // so at the entity level even if its defined at the DTO level

    public class City
    {
        // giving a class a field named ID or class name followed by ID
        // will default to primary key by convention but always good to add key annotation
        [Key]
        // not needed to be explicit since by convention if ID is int it will be auto incremented
        // but shows that the ID is automatically generated when added
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        // where not part of DTO since it was only to get data
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; } = string.Empty;
        public ICollection<PointOfInterest> PointOfInterest { get; set; }
            = new List<PointOfInterest>();

        // we created a ctor to convey that we want this class
        // to always have a name.
        public City(string name)
        {
            Name = name;
        }
    }
}
