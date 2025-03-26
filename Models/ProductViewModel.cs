using Microsoft.AspNetCore.Mvc.Rendering;

namespace OPS_Practice_Project.Models
{
    public class ProductViewModel
    {
        public ProductModel? Product { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}
