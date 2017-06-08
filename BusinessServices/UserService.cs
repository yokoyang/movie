using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;
using Microsoft.AspNet.Identity;

namespace BusinessServices
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;

        public UserService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Mapper.Initialize(cfg => cfg.CreateMap<net_user, UserEntity>());
        }

        //根据用户名称获取用户ID
        public int GetUserByName(string username)
        {
            var user = _unitOfWork.NetUserRepository.Get(u => u.user_name == username);

            return user.iduser_account;
        }

        public int Authenticate(string username, string password)
        {
            var user = _unitOfWork.NetUserRepository.Get(u => u.user_name == username && u.user_password == password);
            if (null != user)
                return user.iduser_account;
            return 0;
        }


        public async Task<UserEntity> FindUser(string userName, string passWord)
        {
            var user = await _unitOfWork.AuthRepository.FindUser(userName, passWord);

            if (user != null)
            {
                try
                {
                    var res = _unitOfWork.NetUserRepository.Get(item => item.user_name == userName);

                    return Mapper.Map<UserEntity>(res);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        public IdentityResult RegisterUser(string userName, string passWord, string phoneNumber)
        {
            var existingUsers = _unitOfWork.NetUserRepository.GetMany(
                item => item.user_name == userName
            );
            IdentityResult authResult;
            // 用户存在
            if (existingUsers.Any())
                return new IdentityResult("用户已存在", "error");
            authResult = _unitOfWork.AuthRepository.RegisterUserSyn(userName, passWord, phoneNumber);
            if (!authResult.Succeeded)
                return authResult;


            // password Hash
            var newUserAccount = new net_user()
            {
                user_name = userName,
                user_password = "*********",
                tel_num = phoneNumber,
            };

            _unitOfWork.NetUserRepository.Insert(newUserAccount);

            try
            {
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                authResult = new IdentityResult(e.Message, "error");
            }

            return authResult;
        }

        public bool UpdateUserAvater(int userId, string avatarPath)
        {
            var user = _unitOfWork.NetUserRepository.GetByID(userId);
            if (null != user)
            {
                _unitOfWork.Save();
            }

            return null != user;
        }

        public int ChargeMoney(string userName, decimal addMoney)
        {
            int result = 1;
            if (addMoney <= 0) throw new ArgumentOutOfRangeException(nameof(addMoney));

            try
            {
                UserEntity _user = FindUser(userName);
                decimal nowMoney = _user.money + addMoney;
                string sqlComand = @"UPDATE net_user set money = " + nowMoney + "where iduser_account = " +
                                   _user.iduser_account;
                using (var context = new WebApiDbEntities())
                {
                    try
                    {
                        result = 0;
                        context.Database.ExecuteSqlCommand(sqlComand);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            return result;
        }

        public UserEntity FindUser(string userName)
        {
            net_user user;
            try
            {
                user = _unitOfWork.NetUserRepository.GetSingle(
                    item => item.user_name == userName
                );
            }
            catch (Exception)
            {
                return null;
            }

            if (null == user) return null;

            return Mapper.Map<net_user, UserEntity>(user);
        }

        public UserEntity FindUser(int userId)
        {
            var user = _unitOfWork.NetUserRepository.GetByID(userId);

            if (null == user) return null;

            return Mapper.Map<net_user, UserEntity>(user);
        }
    }
}