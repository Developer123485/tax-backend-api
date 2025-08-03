using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Utilities;
using TaxApp.DAL.Models;
using static TaxApp.BAL.Models.EnumModel;

namespace TaxApp.BAL.Services
{
    public class UserService : IUserService
    {


        public readonly IConfiguration _configuration;
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            using (var context = new TaxAppContext())
            {
                string decryptedPwd = Encryption.Encrypt(model.Password);
                var user = context.Users.SingleOrDefault(x => x.Email == model.Email && x.Password == decryptedPwd);


                // return null if user not found
                if (user == null) return null;

                // authentication successful so generate jwt token
                var token = generateJwtToken(user);

                return new AuthenticateResponse(user, token);
            }

        }

        public bool CheckVerificationProcess(AuthenticateRequest model)
        {
            using (var context = new TaxAppContext())
            {
                string decryptedPwd = Encryption.Encrypt(model.Password);
                var user = context.Users.SingleOrDefault(x => x.Email == model.Email && x.Password == decryptedPwd && x.IsEmailVerify == true);
                if (user != null)
                    return true;
                else
                    return false;
            }

        }

        public async Task<UserModel> GetUsers(UserFilterModel model)
        {
            var models = new UserModel();
            using (var context = new TaxAppContext())
            {
                var users = await context.Users.Where(p => p.RoleId == Convert.ToInt32(Role.Admin)).ToListAsync();
                models.TotalRows = users.Count();
                if (!String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower().Replace(" ", "");
                    users = users.Where(e => e.UserName.ToLower().Replace(" ", "").Contains(model.Search) || e.Email.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                }
                users = users.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                models.UserList = users;
                models.UserCount = models.TotalRows;
                models.DeductorCount = context.Deductors.Count();
                models.DeducteeCount = context.Deductees.Count();
                models.ChallanCount = context.ChallanList.Count();
                models.DeducteeEntry = context.DeducteeEntry.Count();
                context.Dispose();
            }
            return models;
        }


        public UserSaveModel GetById(int id)
        {
            var response = new UserSaveModel();
            using (var context = new TaxAppContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);
                response.Id = user.Id;
                response.UserName = user.UserName;
                response.PhoneNumber = user.PhoneNumber;
                response.Email = user.Email;
                response.FirmType = user.FirmType;
                response.Organization = user.Organization;
                context.Dispose();
                return response;

            }
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            var issuer = "https://taxapp.com/";
            var audience = "https://taxapp.com/";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAErOdtape1YLgNJgTUg5OJaNDOP9zlCQvRC9grT0/LOsQG/Ci8fw2GUMmt7uhT7H9ye10ltk5nWWXdX5Bfb8OlzA=="));
            var claims = new List<Claim>()
           {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
             new Claim("Ids", user.Id.ToString()),
             new Claim(ClaimTypes.Role, Enum.GetName(typeof(Role), user.RoleId)),
           };
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
           issuer,
           audience,
           claims,
           expires: DateTime.Now.AddDays(1),
           signingCredentials: creds
       );
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }


        public async Task<int> SaveUserData(UserSaveModel model)
        {
            using (var context = new TaxAppContext())
            {
                bool isNewUser = false;
                var currentUser = context.Users.FirstOrDefault(x => x.Id == model.Id);
                if (currentUser == null)
                {
                    currentUser = new User();
                    isNewUser = true;
                    currentUser.IsEmailVerify = false;
                    currentUser.IsMobileVerify = false;
                    currentUser.Password = Encryption.Encrypt(model.Password);
                    currentUser.RoleId = Convert.ToInt32(Role.Admin);
                    currentUser.CreatedDate = DateTime.UtcNow;
                }
                currentUser.UserName = model.UserName;
                currentUser.Email = model.Email;
                currentUser.PhoneNumber = model.PhoneNumber;
                currentUser.FirmType = model.FirmType;
                currentUser.Organization = model.Organization;
                if (isNewUser)
                    await context.Users.AddAsync(currentUser);
                else
                    context.Users.Update(currentUser);
                await context.SaveChangesAsync();
                return currentUser.Id;
            }
        }

        public async Task<bool> UpdateUser(UserSaveModel model)
        {
            using (var context = new TaxAppContext())
            {
                var currentUser = context.Users.FirstOrDefault(x => x.Email == model.Email);
                if (model.EmailOTP != null)
                    currentUser.EmailOTP = model.EmailOTP;
                if (model.MobileOTP != null)
                    currentUser.MobileOTP = model.MobileOTP;
                if (model.PasswordEmailOTP != null)
                    currentUser.PasswordEmailOTP = model.PasswordEmailOTP;
                if (model.IsMobileVerify == true)
                    currentUser.IsMobileVerify = model.IsMobileVerify;
                if (model.IsEmailVerify == true)
                    currentUser.IsEmailVerify = model.IsEmailVerify;
                if (model.Password != null)
                    currentUser.Password = model.Password;
                context.Users.Update(currentUser);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UserActivate(string email)
        {
            using (var context = new TaxAppContext())
            {
                var currentUser = context.Users.FirstOrDefault(x => x.Email == email);
                //if (currentUser == null || currentUser.IsActive == true)
                //    return false;
                //currentUser.IsActive = true;
                context.Users.Update(currentUser);
                await context.SaveChangesAsync();
                return true;
            }
        }



        public async Task<User> GetUserByPhone(string phone)
        {
            using (var context = new TaxAppContext())
            {
                var userData = context.Users.FirstOrDefault(x => x.PhoneNumber == phone);
                context.Dispose();
                return userData;
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using (var context = new TaxAppContext())
            {
                var userData = context.Users.FirstOrDefault(x => x.Email == email);
                context.Dispose();
                return userData;
            }
        }


        public bool SendUserInvitationEmail(string toAddress, string subject, string body)
        {
            MailAddress fromAdd = new MailAddress("nareshguptag1994@gmail.com", "Tax");
            MailAddress toAdd = new MailAddress(toAddress);
            MailMessage msg = new MailMessage(fromAdd, toAdd);
            msg.Subject = subject;
            msg.Body = body;

            System.Net.Mail.SmtpClient smtpclient = new System.Net.Mail.SmtpClient();
            smtpclient.Host = "smtp.gmail.com";

            smtpclient.Port = 587;
            smtpclient.UseDefaultCredentials = false;
            smtpclient.Credentials = new System.Net.NetworkCredential("nareshguptag1994@gmail.com", "ugipnpfeehgzthzn");
            smtpclient.EnableSsl = true;
            msg.IsBodyHtml = true;
            smtpclient.Send(msg);
            return true;
        }
    }
}
