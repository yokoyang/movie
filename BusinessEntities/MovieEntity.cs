using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class MovieEntity
    {
                public long movie_id { get; set; }
        
                public string asin { get; set; }
                public float score { get; set; }
                public string title { get; set; }
                public decimal movie_money { get; set; }
        
                public int time_id { get; set; }
                public string pic_url { get; set; }
                public int amount { get; set; }

        //
        //
        //        public long movie_id { get; set; }
        //        public string asin { get; set; }
        //        public float score { get; set; }
        //        public Nullable<int> time_id { get; set; }
        //        public string title { get; set; }
        //        public Nullable<decimal> movie_money { get; set; }
        //        public string pic_url { get; set; }
        //        public Nullable<int> amount { get; set; }
    }
}