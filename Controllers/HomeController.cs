using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OPS_Practice_Project.Models;
using OPS_Practice_Project.Repository;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace OPS_Practice_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepository _registerRepository;
        private readonly AdminRepository _adminRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginRepository _loginHistoryRepository;

        public HomeController(IWebHostEnvironment env,IHttpContextAccessor httpContextAccessor, LoginRepository loginHistoryRepository)
        {
            _registerRepository = new UserRepository();
            _env = env;
            _adminRepository = new AdminRepository();
            _httpContextAccessor = httpContextAccessor;
            _loginHistoryRepository = loginHistoryRepository;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterPage()
        {
            var model = new UserViewModel
            {
                Register = new UserModel(),
                Countries = _registerRepository.GetCountries()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.Country
                    
                    })
                    .ToList()
            };


            if (model.Countries == null)
            {
                model.Countries = new List<SelectListItem>();
            }
            GenerateCaptcha();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPage(UserViewModel model)
        {
            try
            {

                if (model?.Register?.ProfileImagePath != null && model.Register.ProfileImagePath.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Register.ProfileImagePath.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Register.ProfileImagePath.CopyToAsync(stream);
                    }

                    model.Register.ProfileImage = "/uploads/" + uniqueFileName;
                }

                if (!ValidateCaptcha(model.Register.CaptchaCode, model.Register.CaptchaHashKey))
                {
                    ModelState.AddModelError("Register.CaptchaCode", "Invalid CAPTCHA.");
                    GenerateCaptcha();
                    return View(model);
                }

                _registerRepository.InsertUserS(model?.Register, "Insert");
                GenerateCaptcha();
                TempData["Success"] = "Registration Successful!";
                return RedirectToAction("RegisterPage", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                return View(model);
            }
        }

        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string captchaCode = new string(Enumerable.Repeat(chars, 5)
                                            .Select(s => s[random.Next(s.Length)]).ToArray()); // Generates a 5-character alphanumeric code
            string hashKey = ComputeSha256Hash(captchaCode);

            ViewBag.CaptchaCode = captchaCode;
            ViewBag.CaptchaHashKey = hashKey;
        }

        private bool ValidateCaptcha(string userInput, string storedHash)
        {
            if (string.IsNullOrEmpty(userInput) || string.IsNullOrEmpty(storedHash))
                return false;

            string inputHash = ComputeSha256Hash(userInput);
            return inputHash == storedHash;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpGet]
        public JsonResult ReloadCaptcha()
        {
            try
            {
                var (newCaptcha, newHash) = GenerateCaptchaCode();

                // Store new CAPTCHA hash in session (if using session-based validation)
                HttpContext.Session.SetString("CaptchaHash", newHash);

                return Json(new { success = true, newCaptcha, newHash });
            }
            catch (Exception ex)
            {
                Console.WriteLine("CAPTCHA Reload Error: " + ex.Message);
                return Json(new { success = false, message = "Error loading new CAPTCHA. Please try again.", error = ex.Message });
            }
        }


        private (string CaptchaCode, string CaptchaHash) GenerateCaptchaCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string captchaCode = new string(Enumerable.Repeat(chars, 5)
                                               .Select(s => s[random.Next(s.Length)]).ToArray());
            string captchaHash = ComputeSha256Hash(captchaCode);
            return (captchaCode, captchaHash);
        }



        [HttpGet]
        public JsonResult GetStates(int countryId)
        {
            return Json(_registerRepository.GetStates(countryId)
                .Select(s => new { s.StateId, s.State }));
        }

        [HttpGet]
        public JsonResult GetCities(int stateId)
        {
            return Json(_registerRepository.GetCities(stateId)
                .Select(c => new { c.CityId, c.City }));
        }
        

        [HttpGet]
        public JsonResult GetUserTypes()
        {
            var userTypes = _registerRepository.GetUserTypes()
                .Select(ut => new { ut.Id, ut.UserType })
                .ToList();

            return Json(userTypes);
        }



        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPage(string username, string password)
        {
            var user = _registerRepository.GetUserList()
                .FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", Convert.ToInt32(user.Id));
                HttpContext.Session.SetString("UserName", username);
                int userTypeId = (int)user.UserTypeId;
                HttpContext.Session.SetInt32("UserTypeId", userTypeId);

                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                string userAgent = Request.Headers["User-Agent"].ToString();

                // Insert login history and get the inserted record Id
                bool isInserted = await _loginHistoryRepository.InsertLoginRecordAsync(user.Id, user.UserName, ipAddress, userAgent, "INSERT");

                if (isInserted)
                {
                    // Fetch the last inserted LoginHistory ID (you may need a stored procedure for this)
                    long loginHistoryId = _loginHistoryRepository.GetLastInsertedId(user.Id);
                    HttpContext.Session.SetInt32("LoginHistoryId", Convert.ToInt32(loginHistoryId));
                }

                if (userTypeId == 1)
                {
                    TempData["Success"] = "Admin Login Successful!";
                    return RedirectToAction("AdminHomePage", "Admin");
                }
                else if (userTypeId == 2)
                {
                    TempData["Success"] = "User Login Successful!";
                    return RedirectToAction("ProfileHome", "Profile");
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Retrieve the Login History ID from session
                int? loginHistoryId = HttpContext.Session.GetInt32("LoginHistoryId");

                if (loginHistoryId.HasValue)
                {
                    // Update logout time in the database using Id
                    await _loginHistoryRepository.UpdateLogoutRecordAsync(loginHistoryId.Value, "Logout");
                }

                // Clear session
                HttpContext.Session.Clear();

                // Redirect to Login Page
                return RedirectToAction("LoginPage", "Home");
            }
            catch (Exception ex)
            {
                // Log error (if needed)
                Debug.WriteLine("Logout Error: " + ex.Message);
                return RedirectToAction("Index", "Home"); // Redirect to home if an error occurs
            }
        }



        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult ElectronicsProducts()
        {
            return View();
        }

        public IActionResult Chat()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
