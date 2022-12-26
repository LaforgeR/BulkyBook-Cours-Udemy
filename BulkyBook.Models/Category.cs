using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Obligatoire")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Doit être entre 1 et 100")]
        [Required(ErrorMessage = "Obligatoire")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
