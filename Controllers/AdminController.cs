using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using OPS_Practice_Project.Models;
using OPS_Practice_Project.Repository;

namespace OPS_Practice_Project.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly AdminRepository _adminRepository;
        private readonly UserRepository _userRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminController(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _adminRepository = new AdminRepository();
            _userRepository = new UserRepository();
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult AdminHomePage()
        {
            return View();
        }
        public IActionResult UsersData()
        {
            List<UserModel> usersList = new List<UserModel>();
            usersList = _userRepository.GetUserList();
            return View(new Tuple<List<UserModel>>(usersList));
        }
        public IActionResult EditUserdata()
        {
            List<UserModel> usersList = new List<UserModel>();
            usersList = _userRepository.GetUserList();
            return View(new Tuple<List<UserModel>>(usersList));
        }
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                int createUser = 1; // Replace with the actual user ID of the person performing the delete
                _userRepository.DeleteUser(id, createUser);

                return Json(new { success = true, message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting user: " + ex.Message });
            }
        }
        [HttpGet]
        public IActionResult AddProductForm()
        {
            var viewModel = new ProductViewModel
            {
                Categories = GetCategorySelectList()
            };
            return View(viewModel);
        }

        // POST: Add Product
        [HttpPost]
        public async Task<IActionResult> AddProductForm(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = viewModel.Product;
                try
                {
                    // Ensure ProductId is null for new entries
                    if (model.ProductId == 0) model.ProductId = null;

                    // Image Upload Logic
                    if (model?.ProductImagePath != null && model.ProductImagePath.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(model.ProductImagePath.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError(string.Empty, "Invalid image format.");
                            viewModel.Categories = GetCategorySelectList();
                            return View(viewModel);
                        }

                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "Products");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProductImagePath.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ProductImagePath.CopyToAsync(stream);
                        }

                        model.ImageUrl = $"/uploads/Products/{uniqueFileName}";
                    }

                    // Insert Product
                    long newProductId = _adminRepository.AddProduct(model, "Insert");
                    if (newProductId > 0)
                    {
                        TempData["SuccessMessage"] = "Product added successfully!";
                        return RedirectToAction("AdminHomePage", "Admin");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to insert product.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error adding product: {ex.Message}");
                }
            }

            viewModel.Categories = GetCategorySelectList();
            return View(viewModel);
        }

        // Get All Categories as JSON
        [HttpGet]
        public JsonResult GetCategories()
        {
            try
            {
                var categories = _adminRepository.GetAllCategories()
                    .Select(c => new { c.CategoryId, c.CategoryName });
                return Json(categories);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching categories: " + ex.Message });
            }
        }

        // Helper Method to Get Category List
        private List<SelectListItem> GetCategorySelectList()
        {
            return _adminRepository.GetAllCategories()
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
        }

        public IActionResult EditProduct(long id)
        {
            var product = _adminRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound(); // If product is not found, return 404
            }

            // Convert List<CategoryModel> to List<SelectListItem> for dropdown
            var categories = _adminRepository.GetAllCategories()
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

            ViewBag.Categories = categories; // Store dropdown list in ViewBag

            return View(product); // Pass product data to the view
        }



        // POST: Update Product
        [HttpPost]
        public IActionResult EditProduct(ProductModel product)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdown if validation fails
                ViewBag.Categories = _adminRepository.GetAllCategories()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                return View(product);
            }

            _adminRepository.UpdateProduct(product, "Update");

            return RedirectToAction("ProductDataTable"); // Redirect to product listing after update
        }
        
        public IActionResult ViewProduct(long id)
        {
            var product = _adminRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound(); // Return 404 if product is not found
            }

            return View(product); // Pass product to the ViewProduct page
        }

        [HttpPost]
        public IActionResult DeleteProduct(long productId)
        {
            try
            {
                if (productId <= 0)
                {
                    return Json(new { success = false, message = "Invalid Product ID" });
                }

                _adminRepository.DeleteProduct(productId,"Delete");

                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult ProductDataTable()
        {
            var productsList = _adminRepository.GetAllProducts("Select");
            return View(productsList);
        }

        public IActionResult CategoriesList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCategorie()
        {
            var categories = _adminRepository.GetAllCategories()
                                .Select(c => new
                                {
                                    c.CategoryId,
                                    CategoryTitle = c.CategoryName, // Assuming CategoryName is the actual title
                                    CreateDate = DateTime.Now // Replace this with the actual CreateDate if available
                                })
                                .ToList();

            return Json(categories);
        }

        [HttpPost]
        public JsonResult AddCategory([FromBody] CategoryModel category)
        {
            try
            {
                if (category == null || string.IsNullOrWhiteSpace(category.CategoryName))
                {
                    return Json(new { success = false, message = "Invalid category data!" });
                }

                category.CreateDate = DateTime.Now;
                _adminRepository.AddCategory(category,"Insert");

                return Json(new { success = true, message = "Category added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding category.", error = ex.Message });
            }
        }

        // POST: Edit Category
        [HttpPost]
        public JsonResult EditCategory([FromBody] CategoryModel category)
        {
            try
            {
                if (category.CategoryId <= 0 || string.IsNullOrWhiteSpace(category.CategoryName))
                {
                    return Json(new { success = false, message = "Invalid category data!" });
                }

                _adminRepository.UpdateCategory(category,"Update");
                return Json(new { success = true, message = "Category updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating category.", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteCategory([FromBody] CategoryModel category)
        {
            try
            {
                Console.WriteLine($"Received CategoryId: {category?.CategoryId}");

                if (category == null || category.CategoryId <= 0)
                {
                    return Json(new { success = false, message = "Invalid category ID!" });
                }

                _adminRepository.DeleteCategory(category.CategoryId,0,"Delete");
                return Json(new { success = true, message = "Category deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting category.", error = ex.Message });
            }
        }

        public IActionResult Productsdata()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                var products = _adminRepository.GetAllProducts("Select"); // Ensure "GetAll" matches the stored procedure parameter
                var productList = new List<object>();

                foreach (var product in products)
                {
                    productList.Add(new
                    {
                        ProductId = product.ProductId,
                        CategoryId = product.CategoryId,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        ImageUrl = product.ImageUrl,
                        SubcategoryId = product.SubcategoryId,
                        CreateDate = product.CreateDate,
                        Description = product.Description, // Included description
                        Stock = product.Stock // Included stock count
                    });
                }

                return Json(productList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error fetching products.", error = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Categories()
        {
            var categories = _adminRepository.GetAllCategories();

           
            return Json(categories.Select(c => new
            {
                c.CategoryId,
                c.CategoryName
            }));
        }
        [HttpGet]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            if (categoryId == 0) // If "All Categories" is selected
            {
                return GetProducts(); // Return all products
            }

            var products = _adminRepository.GetProductsByCategory(categoryId);
            return Json(products);
        }

        public IActionResult CategoriesByProducts()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetProductsByCategorys(int categoryId)
        {
            if (categoryId == 0) // If "All Categories" is selected
            {
                return GetProducts(); // Return all products
            }

            var products = _adminRepository.GetProductsByCategory(categoryId);

            return Json(products ?? new List<ProductModel>()); // Ensure it never returns null
        }


    }
}

