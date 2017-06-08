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
    public class ConcessionService : IConcessionService
    {
        private readonly UnitOfWork _unitOfWork;

        public ConcessionService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<concession, ConcessionEntity>();
                cfg.CreateMap<net_user, UserEntity>();
                cfg.CreateMap<movie, MovieEntity>();
            });
        }

        public int JoinGame(int concessionId, string userName)
        {
            net_user user = _unitOfWork.NetUserRepository.Get(u => u.user_name == userName);
            concession _concession = _unitOfWork.ConcessionRepository.Get(con => con.idconcession == concessionId);
            ConcessionEntity movieModel = Mapper.Map<concession, ConcessionEntity>(_concession);
            var userModel = Mapper.Map<net_user, UserEntity>(user);

            DateTime nowTime = DateTime.Now;
            //尚未开始
            if (nowTime < movieModel.start_time)
            {
                return 1;
            }
            //不好意思来晚了
            if (nowTime > movieModel.end_time)
            {
                return 2;
            }
            //时间上允许->看看这个人有没有参加过这个活动
            var _record =
                _unitOfWork.ConcessionRecordRepository.Get(
                    r => r.iduser_account == userModel.iduser_account && r.idconcession == concessionId);
            if (_record != null)
            {
                //对不起，您已经参加过活动了
                return 3;
            }

            //添加一个尚未开奖的记录
            string sqlComand = @"INSERT INTO concession_record(idconcession, iduser_account, state) values(" +
                               concessionId + ", " + userModel.iduser_account + ", " + "2)";
            using (var context = new WebApiDbEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Database.ExecuteSqlCommand(sqlComand);
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //回滚
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }

            return 0;
        }

        //根据idconcession获得concession详细信息
        private ConcessionEntity getConcessionById(int idconcession)
        {
            try
            {
                var _concession = _unitOfWork.ConcessionRepository.GetByID(idconcession);
                var concessionModel = Mapper.Map<concession, ConcessionEntity>(_concession);
                return concessionModel;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public List<ConcessionResultInfo> ReviewResult(string userName, int state)
        {
            List<ConcessionResultInfo> concessionResultInfos = new List<ConcessionResultInfo>();

            try
            {
                net_user _user = _unitOfWork.NetUserRepository.Get(u => u.user_name == userName);
                //查找到属于该用户的，且满足state的 （ 0 没有中奖 1 中奖 2尚未开奖）
                List<concession_record> concessionRecords = _unitOfWork.ConcessionRecordRepository.GetMany(
                    r => r.iduser_account == _user.iduser_account && r.state == state).ToList();
                //有数据的话
                //再从consession 中进行包装返回
                if (concessionRecords.Any())
                {
                    foreach (var record in concessionRecords)
                    {
                        ConcessionEntity concessionEntity = getConcessionById(record.idconcession);
                        ConcessionResultInfo concessionResultInfo = new ConcessionResultInfo();
                        concessionResultInfo.concessionId = record.idconcession;
                        concessionResultInfo.concessionName = concessionEntity.concession_name;
                        concessionResultInfo.idmoive = concessionEntity.idmoive;
                        var _movie = _unitOfWork.MovieRepository.GetByID(concessionEntity.idmoive);
                        concessionResultInfo.movieName = _movie.title;
                        concessionResultInfo.price = concessionEntity.price;
                        concessionResultInfos.Add(concessionResultInfo);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            return concessionResultInfos;
        }


        //返回1表示 时间错误    2 表示库存不够创建
        public int PublishConcession(PublishConcessionEntity publishConcession)
        {
            //必须大于当前时间
            if (publishConcession.startTime < DateTime.Now)
            {
                return 1;
            }
            //开始发布
            concession _concession = new concession
            {
                idmoive = publishConcession.movieId,
                price = publishConcession.price,
                amount = publishConcession.amount,
                start_time = publishConcession.startTime,
                end_time = publishConcession.endTime,
                concession_name = publishConcession.concessionName
            };

            try
            {
                using (var context = new WebApiDbEntities())
                {
                    try
                    {
                        var _movie = _unitOfWork.MovieRepository.GetByID(publishConcession.movieId);
                        var movieModel = Mapper.Map<movie, MovieEntity>(_movie);

                        int nowNum = movieModel.amount - publishConcession.amount;
                        if (nowNum < 0)
                        {
                            return 2;
                        }
                        //库存减少
                        context.Database.ExecuteSqlCommand(
                            @"UPDATE movie SET amount =" + nowNum +
                            " WHERE movie_id = '" + movieModel.movie_id + "'"
                        );

                        context.SaveChanges();
                        _unitOfWork.ConcessionRepository.Insert(_concession);
                        _unitOfWork.Save();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }


                return 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 2;
            }
        }

        public List<ConcessionEntity> ShowTheConcessions()
        {
            DateTime now = DateTime.Now;
            try
            {
                List<concession> concessions = _unitOfWork.ConcessionRepository
                    .GetMany(c => c.start_time <= now && c.end_time > now).ToList();
                var concessionModel = Mapper.Map<List<concession>, List<ConcessionEntity>>(concessions);
                return concessionModel;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public ConcessionEntity GetConsessionById(int concessionId)
        {
            try
            {
                concession _concession = _unitOfWork.ConcessionRepository.GetByID(concessionId);
                var concessionModel = Mapper.Map<concession, ConcessionEntity>(_concession);
                movie _movie = _unitOfWork.MovieRepository.GetByID(concessionModel.idmoive);
                concessionModel.movieName = _movie.title;
                return concessionModel;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        // 0-成功 1 表示余额不足 2表示您没有中奖 3-尚未开奖  4-已经领取过了
        public int PayConcession(string userName, int concessionId)
        {
            int result = 1;
            try
            {
                net_user _user = _unitOfWork.NetUserRepository.Get(u => u.user_name == userName);
                UserEntity userModel = Mapper.Map<net_user, UserEntity>(_user);
                int userId = userModel.iduser_account;
                concession_record concessionRecord = _unitOfWork.ConcessionRecordRepository.Get(
                    r => r.idconcession == concessionId && r.iduser_account == userId);
                if (concessionRecord.state == 0)
                {
                    return 2;
                }
                if (concessionRecord.state == 2)
                {
                    return 3;
                }
                if (concessionRecord.state == 3)
                {
                    return 4;
                }
                concession _concession = _unitOfWork.ConcessionRepository.GetByID(concessionId);
                //可以领取奖励
                if (concessionRecord.state == 1)
                {
                    decimal nowMoney = userModel.money - _concession.price;
                    if (nowMoney < 0)
                    {
                        return 1;
                    }
                    using (var context = new WebApiDbEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                context.Database.ExecuteSqlCommand(
                                    @"UPDATE net_user SET money =" + nowMoney +
                                    " WHERE iduser_account = '" + userModel.iduser_account + "'"
                                );
                                string sqlComand = @"UPDATE concession_record  set state = 3 where iduser_account = '"
                                                   + userModel.iduser_account + "' and idconcession = '" +
                                                   concessionId + " '";
                                context.Database.ExecuteSqlCommand(sqlComand);
                                context.SaveChanges();
                                dbContextTransaction.Commit();
                                return 0;
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                                dbContextTransaction.Rollback();
                                throw;
                            }
                        }
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

        //生成number个随机数, 最大的上限为scope 下线是1
        private HashSet<int> GenerateRandom(int number, int scope)
        {
            //如果参加人数太少了
            if (scope < number)
            {
                number = scope;
            }

            Random rand = new Random();
            List<int> result = new List<int>();
            HashSet<int> check = new HashSet<int>();
            for (Int32 i = 0; i < number; i++)
            {
                int curValue = rand.Next(1, scope + 1);
                while (check.Contains(curValue))
                {
                    curValue = rand.Next(1, scope + 1);
                }
                result.Add(curValue);
                check.Add(curValue);
            }
            return check;
        }

        //了解到有多少人参加某项活动
        private int GetScope(int idconcession)
        {
            int number = 0;
            string sql = @"select * from concession_record where concession_record = '" + idconcession + "'";

            using (var context = new WebApiDbEntities())
            {
                try
                {
                    number = context.Database.SqlQuery<concession_record>(sql).ToList().Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return number;
        }


        private List<int> TargertRecord()
        {
            List<int> recordIds = new List<int>();
            //找到联系集中所有状态为2 未开奖的记录
            string sql = @"select * from concession_record where state = '2'";
            HashSet<int> concessionIds = new HashSet<int>();

            using (var context = new WebApiDbEntities())
            {
                try
                {
                    var records = context.Database.SqlQuery<concession_record>(sql).ToList();
                    foreach (var record in records)
                    {
                        //找到不重复的抽奖ID，保存到hashSet中
                        concessionIds.Add(record.idconcession);
                    }
                    string selectSql = @"select end_time from concession where idconcession = '";
                    foreach (var id in concessionIds)
                    {
                        DateTime endTime = context.Database.SqlQuery<DateTime>(selectSql + id + "'").ToList().First();
                        if (endTime < DateTime.Now)
                        {
                            //需要被开奖
                            recordIds.Add(id);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return recordIds;
        }

        //amount -> 有多少人能被抽中  scope 参加的人数  
        private void DoUpdate(int idconcession, int amount)
        {
            //更新为0状态 表示没有没有中奖
            //状态为 1 幸运儿
            //先取出所有 id = idconcession 的concession_record，然后对于里面的每一条数据
            //如果它的index == 中奖号码，则为1，反之为0
            string sql = @"select * from concession_record where idconcession = '" + idconcession + "'";


            using (var context = new WebApiDbEntities())
            {
                try
                {
                    List<concession_record> records = context.Database.SqlQuery<concession_record>(sql).ToList();
                    int scope = records.Count;
                    int counter = 0;
                    HashSet<int> lucks = GenerateRandom(amount, scope);
                    foreach (var record in records)
                    {
                        string state;
                        counter++;
                        state = lucks.Contains(counter) ? "1" : "0";

                        string sqlComand = @"UPDATE concession_record set state = '" + state +
                                           "' where idconcession = '" + idconcession + "' and iduser_account = '" +
                                           record.iduser_account + "'";
                        context.Database.ExecuteSqlCommand(sqlComand);
                        context.SaveChanges();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        //只要有用户访问服务器的时候，就对所有参加抽奖的用户的抽奖记录进行扫描
        public void UpdateConcessionRecord()
        {
            List<int> tagertReords = TargertRecord();
            foreach (var record in tagertReords)
            {
                int amount = _unitOfWork.ConcessionRepository.GetByID(record).amount;

                //执行update操作
                DoUpdate(record, amount);
            }
        }
    }
}