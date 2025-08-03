using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        bool CheckVerificationProcess(AuthenticateRequest model);
        UserSaveModel GetById(int id);
        bool SendUserInvitationEmail(string toAddress, string subject, string body);
        Task<UserModel> GetUsers(UserFilterModel model);
        Task<int> SaveUserData(UserSaveModel model);
        Task<bool> UserActivate(string email);
        Task<bool> UpdateUser(UserSaveModel model);

        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByPhone(string phone);
    }
}
