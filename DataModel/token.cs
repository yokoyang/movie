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
    
    public partial class token
    {
        public int token_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public string auth_token { get; set; }
        public Nullable<System.DateTime> issueed_on { get; set; }
        public Nullable<System.DateTime> expire_on { get; set; }
    }
}