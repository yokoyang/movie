//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class movie
    {
        public long movie_id { get; set; }
        public string asin { get; set; }
        public float score { get; set; }
        public Nullable<int> time_id { get; set; }
        public string title { get; set; }
        public Nullable<decimal> movie_money { get; set; }
        public string pic_url { get; set; }
        public Nullable<int> amount { get; set; }
    }
}
