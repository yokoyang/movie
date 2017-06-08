using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class ShopCartEntity
    {
        public int iduseraccount { get; set; }
        public int idmovie { get; set; }
        public int amount { get; set; }
        public int type { get; set; }

        //唯一编号
        public long idshop_cart_movie { get; set; }

        public int state { get; set; }
    }
}