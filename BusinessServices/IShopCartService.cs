using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using BusinessEntities;

namespace BusinessServices
{
    public interface IShopCartService
    {
        //购买商品
        int BuyMovie(int movieId, int amount, string userName);

        //查询我的购物车
        List<ShopMovieEntity> QueryChart(string userName, int state);

        //购物车中删除商品
        int DeleteChartMovie(int movieId,string userName);
        
        //添加到购物车
        int AddMovie(int movieId,int amount, string userName);


    }
}