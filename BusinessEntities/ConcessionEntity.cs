using System;

namespace BusinessEntities
{
    public class ConcessionEntity
    {
        public int idconcession { get; set; }
        public string amount { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public long idmoive { get; set; }
        public decimal price { get; set; }
        public string concession_name { get; set; }

        public string movieName { get; set; }
    }
}