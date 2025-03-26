using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Data.SqlClient;
using OPS_Practice_Project.Models;
using System;
using System.Data;

namespace OPS_Practice_Project.Repository
{

    public class AdminRepository
    {
        string _connectionString = "Server=DESKTOP-0OMS0D3\\SQLEXPRESS;Database=ExTPracticeProject;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=True;";
        public IEnumerable<ProductModel> GetAllProducts(string actionMode)
        {
            var products = new List<ProductModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Product_List", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ActionMode", actionMode);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductModel
                            {
                                ProductId = reader["ProductId"] != DBNull.Value ? (long)reader["ProductId"] : 0,
                                ProductName = reader["ProductName"]?.ToString() ?? "No Name",
                                Description = reader["Description"]?.ToString() ?? "No Description",
                                Price = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0.00m,
                                Stock = reader["Stock"] != DBNull.Value ? (int)reader["Stock"] : 0,
                                ImageUrl = reader["ImageUrl"]?.ToString() ?? "/uploads/default.jpg",
                                CategoryId = reader["CategoryId"] != DBNull.Value ? (long?)reader["CategoryId"] : null,
                                SubcategoryId = reader["SubcategoryId"] != DBNull.Value ? Convert.ToInt64(reader["SubcategoryId"]) : (int?)null,
                                Flag = reader["Flag"]?.ToString() ?? "N/A",
                                CreateUser = reader["CreateUser"] != DBNull.Value ? (long?)reader["CreateUser"] : null,
                                CreateDate = reader["CreateDate"] != DBNull.Value ? (DateTime?)reader["CreateDate"] : null,
                                UpdateUser = reader["UpdateUser"] != DBNull.Value ? (long?)reader["UpdateUser"] : null,
                                UpdateDate = reader["UpdateDate"] != DBNull.Value ? (DateTime?)reader["UpdateDate"] : null
                            });
                        }
                    }
                }
            }
            return products;
        }

        public ProductModel GetProductById(long productId, string actionMode = "SelectById")
        {
            ProductModel product = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Product_List", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@ActionMode", actionMode);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new ProductModel
                            {
                                ProductId = (long)reader["ProductId"],
                                ProductName = reader["ProductName"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = (decimal)reader["Price"],
                                Stock = (int)reader["Stock"],
                                ImageUrl = reader["ImageUrl"].ToString(),
                                CategoryId = reader["CategoryId"] as long?,
                                Flag = reader["Flag"].ToString(),
                                CreateUser = reader["CreateUser"] as long?,
                                CreateDate = reader["CreateDate"] as DateTime?,
                                UpdateUser = reader["UpdateUser"] as long?,
                                UpdateDate = reader["UpdateDate"] as DateTime?
                            };
                        }
                    }
                }
            }
            return product;
        }

        public IEnumerable<CategoryModel> GetAllCategories()
        {
            var categories = new List<CategoryModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
               

                using (var command = new SqlCommand("SP_GetAllCategories", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new CategoryModel
                            {
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                CategoryName = reader["CategoryName"].ToString()
                            });
                        }
                    }
                }
            }

           
            return categories;
        }

        public IEnumerable<ProductModel> GetProductsByCategory(int categoryId)
        {
            var products = new List<ProductModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_GetProductsByCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryId", categoryId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductModel
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = reader["ProductName"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Stock = Convert.ToInt32(reader["Stock"]),
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                ImageUrl = reader["ImageUrl"].ToString(),
                                CreateDate = Convert.ToDateTime(reader["CreateDate"])
                            });
                        }
                    }
                }
            }
            return products;
        }

        public CategoryModel GetCategoryById(long categoryId,string action)
        {
            CategoryModel category = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Category_CRUD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CategoryID", categoryId);
                    command.Parameters.AddWithValue("@ActionMode", action);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            category = new CategoryModel
                            {
                                CategoryId = (long)reader["CategoryID"],
                                CategoryName = reader["CategoryName"].ToString()
                            };
                        }
                    }
                }
            }
            return category;
        }

        // ✅ ADD CATEGORY
        public long AddCategory(CategoryModel category,string action)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Category_CRUD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    command.Parameters.AddWithValue("@CreateUser", category.CreateUser);
                    command.Parameters.AddWithValue("@ActionMode", action);

                    connection.Open();
                    return Convert.ToInt64(command.ExecuteScalar()); // Get inserted ID
                }
            }
        }

        // ✅ UPDATE CATEGORY
        public void UpdateCategory(CategoryModel category,string action)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Category_CRUD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryID", category.CategoryId);
                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    command.Parameters.AddWithValue("@UpdateUser", category.UpdateUser);
                    command.Parameters.AddWithValue("@ActionMode", action);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        
        public void DeleteCategory(long categoryId, long updateUser,string action)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Category_CRUD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryID", categoryId);
                    command.Parameters.AddWithValue("@UpdateUser", updateUser);
                    command.Parameters.AddWithValue("@ActionMode", action);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        public long AddProduct(ProductModel product, string ActionMode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Product_Save", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Handle NULL ProductId properly
                    var productIdParam = new SqlParameter("@ProductId", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = product.ProductId > 0 ? product.ProductId : (object)DBNull.Value
                    };
                    command.Parameters.Add(productIdParam);

                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@StockQuantity", product.Stock);
                    command.Parameters.AddWithValue("@ProductImage", product.ImageUrl ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                    command.Parameters.AddWithValue("@Flag", product.Flag);
                    command.Parameters.AddWithValue("@CreateUser", product.CreateUser);
                    command.Parameters.AddWithValue("@ActionMode", ActionMode);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if rows were inserted
                    if (rowsAffected > 0)
                    {
                        return productIdParam.Value != DBNull.Value ? Convert.ToInt64(productIdParam.Value) : 0;
                    }
                    else
                    {
                        throw new Exception("No rows inserted.");
                    }
                }
            }
        }

        public void UpdateProduct(ProductModel product, string ActionMode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Product_Save", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ProductId", product.ProductId);
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@StockQuantity", product.Stock);

                    // ✅ Handle Product Image correctly
                    command.Parameters.AddWithValue("@ProductImage",
                        string.IsNullOrEmpty(product.ImageUrl) ? (object)DBNull.Value : product.ImageUrl);

                    command.Parameters.AddWithValue("@CategoryId", product.CategoryId);

                    if (product.UpdateUser == 0 || product.UpdateUser == null)
                    {
                        product.UpdateUser = 1;  // Set default user ID (change this if needed)
                    }
                    command.Parameters.AddWithValue("@CreateUser", product.UpdateUser);

                    if (string.IsNullOrEmpty(product.Flag))
                    {
                        product.Flag = "A"; // Default to 'A' (Active)
                    }
                    command.Parameters.AddWithValue("@Flag", product.Flag);

                    command.Parameters.AddWithValue("@ActionMode", ActionMode);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(long productId,string ActionMode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_Product_Save", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@Flag", "D"); // Soft delete
                    command.Parameters.AddWithValue("@CreateUser", 1);
                    command.Parameters.AddWithValue("@ActionMode", ActionMode);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
