using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;

namespace BusinessServices
{
    public class AdminService : IAdminService
    {
        private readonly UnitOfWork _unitOfWork;

        public AdminService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Mapper.Initialize(cfg => { cfg.CreateMap<admin, AdminEntity>(); });
        }

        public int AdminLogin(string adminName, string passWord)
        {
            int result = 1;
            var _admin =
                _unitOfWork.AdminRepository.Get(ad => ad.admin_name == adminName && ad.admin_password == passWord);
            if (_admin != null)
            {
                result = 0;
            }
            return result;
        }
    }
}