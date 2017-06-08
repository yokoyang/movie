using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BusinessServices
{
    public interface IUserService
    {
        int Authenticate(string username, string password);
        IdentityResult RegisterUser(string userName, string passWord, string phoneNumber);
        Task<UserEntity> FindUser(string userName, string passWord);
        UserEntity FindUser(string userName);
        UserEntity FindUser(int userId);

        bool UpdateUserAvater(int userId, string avatarPath);

        //充值 money
        int ChargeMoney(string userName,decimal addMoney);
    }
}
