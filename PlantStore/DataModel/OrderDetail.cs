//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlantStoreAPI.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderDetail
    {
        public int sid { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int ItemAmount { get; set; }
        public decimal ItemTotal { get; set; }
    
        public virtual Order Order { get; set; }
    }
}
