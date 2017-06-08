using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public interface IAdminService
    {
        //管理员登录

        int AdminLogin(string adminName, string passWord);

    }
}