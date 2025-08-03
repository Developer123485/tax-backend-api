using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.DAL.Models;
using static TaxApp.BAL.Models.EnumModel;

namespace TaxApp.BAL.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public bool isDeductorList { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            Username = user.UserName;
            Token = token;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            Role =  Enum.GetName(typeof(Role), user.RoleId);
        }
    }
}
