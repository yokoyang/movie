using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class ConcessionResultInfo
    {
        public int concessionId { get; set; }
        public string concessionName { get; set; }
        public decimal price { get; set; }
        public long idmoive { get; set; }
        public string movieName { get; set; }
    }
}