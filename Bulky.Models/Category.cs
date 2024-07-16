using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Range(1, 999, ErrorMessage = "Display Order value must be between 1-999")]
        public int DisplayOrder { get; set; }
    }
}
