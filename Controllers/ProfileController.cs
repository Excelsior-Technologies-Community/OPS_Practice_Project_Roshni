using Microsoft.AspNetCore.Mvc;
using OPS_Practice_Project.Models;
using OPS_Practice_Project.Repository;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using OPS_Practice_Project.Hubs;

namespace OPS_Practice_Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserRepository _registerRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginRepository _loginHistoryRepository;
        private readonly ChatRepository _chatRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        public ProfileController(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, LoginRepository loginHistoryRepository,ChatRepository chatRepository, IHubContext<ChatHub> hubContext)
        {
            _registerRepository = new UserRepository();
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _loginHistoryRepository = loginHistoryRepository;
            _chatRepository = chatRepository;
            _hubContext = hubContext;
        }
        
        public IActionResult ProfileHome()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProfilePage()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage", "Home");
            }

            var user = _registerRepository.GetUserById(userId.Value);
            if (user == null)
            {
                return RedirectToAction("LoginPage", "Home");
            }

            string countryName = _registerRepository.GetCountryNameById((int)user.CountryId);
            string stateName = _registerRepository.GetStateNameById((int)user.StateId);
            string cityName = _registerRepository.GetCityNameById((int)user.CityId);
            string userTypeName = _registerRepository.GetUserTypeById((int)user.UserTypeId);


            // Setting data directly into the view model
            var viewModel = new UserViewModel
            {
                Register = new UserModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    MobileNumber = user.MobileNumber,
                    Password = user.Password,
                    UserName = user.UserName,
                    Address = user.Address,
                    CountryName = countryName, // Directly assigning the string value
                    StateName = stateName,
                    CityName = cityName,
                    UserTypeName = userTypeName
                }
            };

            return View(viewModel);
        }
        

        [HttpGet]
        public JsonResult GetUserTypes()
        {
            return Json(_registerRepository.GetUserTypes()
                .Select(ut => new { ut.Id, ut.UserType }));
        }
        [HttpGet]
        public IActionResult ProfileUpdatePage(int id)
        {
            // Fetch the user details from the database
            var user = _registerRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            // Create a ViewModel and populate it with user details and countries
            UserViewModel viewModel = new UserViewModel
            {
                Register = user, // Set the user data
                Countries = _registerRepository.GetCountries()
                    .Select(c => new SelectListItem
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.Country
                    }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ProfileUpdatePage(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdown lists if validation fails
                model.Countries = _registerRepository.GetCountries()
                    .Select(c => new SelectListItem { Value = c.CountryId.ToString(), Text = c.Country }).ToList();
                model.States = _registerRepository.GetStates(Convert.ToInt32(model.Register.CountryId))
                    .Select(s => new SelectListItem { Value = s.StateId.ToString(), Text = s.State }).ToList();

                model.Cities = _registerRepository.GetCities(Convert.ToInt32(model.Register.StateId))
                    .Select(c => new SelectListItem { Value = c.CityId.ToString(), Text = c.City }).ToList();

                model.UserTypes = _registerRepository.GetUserTypes()
                    .Select(ut => new SelectListItem { Value = ut.Id.ToString(), Text = ut.UserType }).ToList();

                _registerRepository.UpdateUser(model,"Update");
                return RedirectToAction("ProfilePage", "Profile");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetStates(int countryId)
        {
            var states = _registerRepository.GetStates(countryId)
                .Select(s => new { StateId = s.StateId, StateName = s.State }).ToList();
            return Json(states);
        }

        [HttpGet]
        public JsonResult GetCities(int stateId)
        {
            var cities = _registerRepository.GetCities(stateId)
                .Select(c => new { CityId = c.CityId, CityName = c.City }).ToList();
            return Json(cities);
        }

        public IActionResult UserChat()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("LoginPage", "Hone");
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SendMessage(string message)
        {
            int? senderId = HttpContext.Session.GetInt32("UserId");
            if (senderId == null || senderId == 0)
            {
                return Json(new { success = false, error = "User not logged in" });
            }

            ChatModel obj = new ChatModel
            {
                SenderId = senderId.Value,
                ReceiverId = 4,
                Message = message,
                MessageType = "personal",
                IsAdminReply = false
            };

            string result = _chatRepository.InsertChat(obj, "INSERT");

            if (result == "Message Inserted Successfully")
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "User " + senderId.Value, message, false, DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"));
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }



        //[HttpPost]
        //public async Task<JsonResult> SendAdminMessage(string message)
        //{
        //    ChatModel obj = new ChatModel();
        //    obj.SenderId = 0;
        //    obj.ReceiverId = 0;
        //    obj.Message = message;
        //    obj.MessageType = "admin";
        //    obj.IsAdminReply = true;

        //    string result = _chatRepository.InsertChat(obj, "INSERT");

        //    if (result == "Message Inserted Successfully")
        //    {
        //        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Admin", message, true, DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"));
        //        return Json(true);
        //    }
        //    else
        //    {
        //        return Json(false);
        //    }
        //}

        
        [HttpGet]
        public JsonResult GetAllMessages()
        {
            var list = _chatRepository.SelectAllMessages("SELECT");
            return Json(list);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
