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
    public class ShopCartService : IShopCartService
    {
        private readonly UnitOfWork _unitOfWork;


        public ShopCartService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<movie, MovieEntity>();
                cfg.CreateMap<net_user, UserEntity>();
                cfg.CreateMap<shop_cart_movie, ShopCartEntity>();
            });
        }

        private long CheckChart(int movieId, int userID)
        {
            long result = 0;
            var chart = _unitOfWork.ShopCartMovieRepository.Get(s => s.idmovie == movieId && s.iduseraccount == userID);
            if (chart != null)
            {
                result = chart.idshop_cart_movie;
            }
            return result;
        }

        //直接购买 需要再把原来购车中的state更新
        public int BuyMovie(int movieId, int amount, string userName)
        {
            var user = _unitOfWork.NetUserRepository.Get(u => u.user_name == userName);


            movie _movie = _unitOfWork.MovieRepository.GetByID(movieId);

            if (_movie == null)
            {
                //ID不正确
                return 3;
            }


            var movieModel = Mapper.Map<movie, MovieEntity>(_movie);

            var userModel = Mapper.Map<net_user, UserEntity>(user);

            int oldNum = movieModel.amount;
            int nowNum = oldNum - amount;
            if (nowNum < 0)
            {
                //库存不足
                return 1;
            }
            decimal nowMoney = userModel.money - amount * movieModel.movie_money;
            if (nowMoney < 0)
            {
                //余额不足
                return 2;
            }
            //检查在购物车中有没有该商品
            long result = CheckChart(movieId, userModel.iduser_account);

            string sqlComand;
            //原来有这条数据 需要做更新操作
            if (result == 0)
            {
                sqlComand = @"INSERT into shop_cart_movie(iduseraccount, idmovie, amount, shop_time, state) values(" +
                            userModel.iduser_account + "," + movieModel.movie_id + "," + amount + ",'" + DateTime.Now +
                            "', 0)";
            }
            //还没有这条数据 需要做插操作
            else
            {
                sqlComand = @"UPDATE shop_cart_movie set shop_time = '" + DateTime.Now +
                            "', state = 0 where idshop_cart_movie = " + result;
            }

            //满足购买条件下执行
            using (var context = new WebApiDbEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //库存减少
                        context.Database.ExecuteSqlCommand(
                            @"UPDATE movie SET amount =" + nowNum +
                            " WHERE movie_id = '" + movieId + "'"
                        );
                        //用户钱减少
                        context.Database.ExecuteSqlCommand(
                            @"UPDATE net_user SET money =" + nowMoney +
                            " WHERE iduser_account = '" + userModel.iduser_account + "'"
                        );
                        //创建一条购物记录
//                        string updateSql =
//                            @"INSERT into shop_cart_movie (iduseraccount,idmovie,amount,shop_time,state) values ("
//                            + userModel.iduser_account + "," + movieModel.movie_id + "," + amount + ",'" +
//                            DateTime.Now + "', 0)";

                        context.Database.ExecuteSqlCommand(sqlComand);
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        return 0;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //回滚
                        dbContextTransaction.Rollback();
                        return 1;
                    }
                }
            }
        }

        private string GetPicUrl(int counter)
        {
            counter = counter % 400;
            var rnd = new Random();
            var id = rnd.Next(600);
            id += counter;
            var _pic = _unitOfWork.PictureRepository.GetByID(id);
            return _pic.picture_url;
        }

        private List<ShopMovieEntity> GetShoppedMovie(List<ShopCartEntity> records)
        {
            //先根据movieId 查询电影信息
            List<ShopMovieEntity> shopMovieEntities = new List<ShopMovieEntity>();
            int conuter = 1;
            foreach (var record in records)
            {
                conuter++;
                ShopMovieEntity shopMovieEntity = new ShopMovieEntity();
                var _movie = _unitOfWork.MovieRepository.GetByID(record.idmovie);
                var moviesModel = Mapper.Map<movie, MovieEntity>(_movie);
                shopMovieEntity.amount = record.amount;
                shopMovieEntity.asin = moviesModel.asin;
                shopMovieEntity.movie_id = moviesModel.movie_id;
                shopMovieEntity.score = moviesModel.score;
                shopMovieEntity.title = moviesModel.title;
                shopMovieEntity.movie_money = moviesModel.movie_money;
                shopMovieEntity.pic_url = GetPicUrl(conuter);
                shopMovieEntity.pic_url = GetPicUrl(conuter);

                shopMovieEntities.Add(shopMovieEntity);
            }
            return shopMovieEntities;
        }

        public List<ShopMovieEntity> QueryChart(string userName, int state)
        {
            var user = _unitOfWork.NetUserRepository.Get(u => u.user_name == userName);
            var userModel = Mapper.Map<net_user, UserEntity>(user);

            string sql =
                "SELECT * FROM shop_cart_movie s_c where s_c.iduseraccount ='" +
                userModel.iduser_account + "' and s_c.state = " + state;

            using (var context = new WebApiDbEntities())
            {
                var records = context.Database.SqlQuery<shop_cart_movie>(sql).ToList();
                if (records.Any())
                {
                    var recordsModel = Mapper.Map<List<shop_cart_movie>, List<ShopCartEntity>>(records);

                    return GetShoppedMovie(recordsModel);
                }
                return null;
            }
        }

        public int DeleteChartMovie(int movieId, string userName)
        {
            int result = -1;
            var user = _unitOfWork.NetUserRepository.Get(u => u.user_name == userName);
            long idshop_cart = CheckChart(movieId, user.iduser_account);

            string sqlComand = @"UPDATE shop_cart_movie set shop_time = '" + DateTime.Now +
                               "', state = 2 where idshop_cart_movie = " + idshop_cart;
            using (var context = new WebApiDbEntities())
            {
                try
                {
                    result = 0;
                    context.Database.ExecuteSqlCommand(sqlComand);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    result = 1;
                    Console.WriteLine(e);
                    throw;
                }
            }
            return result;
        }

        //添加到购物车
        public int AddMovie(int movieId, int amount, string userName)
        {
            var user = _unitOfWork.NetUserRepository.Get(u => u.user_name == userName);
            var userModel = Mapper.Map<net_user, UserEntity>(user);
            movie _movie = _unitOfWork.MovieRepository.GetByID(movieId);
            //该商品不存在了
            if (_movie == null)
            {
                return 2;
            }
            if (_movie.amount < amount)
            {
                return 1;
            }
            //添加到购物车
            string sqlComand =
                @"INSERT into shop_cart_movie(iduseraccount, idmovie, amount, shop_time, state) values(" +
                userModel.iduser_account + "," + _movie.movie_id + "," + amount + ",'" + DateTime.Now +
                "', 1)";

            using (var context = new WebApiDbEntities())
            {
                try
                {
                    context.Database.ExecuteSqlCommand(sqlComand);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return 0;
        }
    }
}