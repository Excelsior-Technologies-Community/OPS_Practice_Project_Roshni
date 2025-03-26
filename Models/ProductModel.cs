namespace OPS_Practice_Project.Models
{
    public class ProductModel
    {
        public long? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile? ProductImagePath { get; set; }
        public string? ImageUrl { get; set; }
        public long? CategoryId { get; set; }
        public string? Flag { get; set; }
        public long? CreateUser { get; set; }
        public CategoryModel? Category { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        public long? SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
    }
}
