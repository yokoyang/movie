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
    
    public partial class concession_record
    {
        public int idconcession { get; set; }
        public int iduser_account { get; set; }
        public Nullable<int> state { get; set; }
    
        public virtual concession concession { get; set; }
        public virtual net_user net_user { get; set; }
    }
}
