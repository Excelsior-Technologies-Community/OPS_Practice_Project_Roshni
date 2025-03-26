using Microsoft.AspNetCore.Mvc.Rendering;

namespace OPS_Practice_Project.Models
{
    public class UserViewModel
    {
        public UserModel? Register { get; set; }
        public List<SelectListItem>? Countries { get; set; }
        public IEnumerable<SelectListItem>? States { get; set; }
        public IEnumerable<SelectListItem>? Cities { get; set; }
        public List<SelectListItem>? UserTypes { get; set; }

    }
}
