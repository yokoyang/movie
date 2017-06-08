using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntities;

namespace BusinessServices
{
    public interface IConcessionService
    {
        //参加抽奖
        int JoinGame(int concessionId, string userName);

        //查看抽奖结果
        List<ConcessionResultInfo> ReviewResult(string userName,int state);

        //发布抽奖
        int PublishConcession(PublishConcessionEntity publishConcession);

        //抽奖公示栏
        List<ConcessionEntity> ShowTheConcessions();

        //查看concession详细信息 concessionId
        ConcessionEntity GetConsessionById(int concessionId);

        //支付抽奖
        int PayConcession(string userName,int concessionId);

        //每次查询的时候 && 支付抽奖的时候
        //更新concession_record 表中的数据
        void UpdateConcessionRecord();


    }
}