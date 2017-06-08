using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class PublishConcessionEntity
    {
        //[movieId, concessionName, startTime, endTime, price, amount]
        public int movieId { get; set; }

        public string concessionName { get; set; }

        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public Decimal price { get; set; }

        public int amount { get; set; }
    }
}