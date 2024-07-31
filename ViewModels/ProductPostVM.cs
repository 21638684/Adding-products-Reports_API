using System.ComponentModel.DataAnnotations;

namespace Assignment3_Backend.ViewModels
{
    public class ProductPostVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
