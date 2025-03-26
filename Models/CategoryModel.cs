namespace OPS_Practice_Project.Models
{
    public class CategoryModel
    {
        public long CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Flag { get; set; }
        public long? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        public ICollection<ProductModel>? Products { get; set; }
    }
}
