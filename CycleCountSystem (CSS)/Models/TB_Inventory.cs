//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CycleCountSystem__CSS_.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_Inventory
    {
        public int Id_Inventory { get; set; }
        public string Id_material { get; set; }
        public string Mat_description { get; set; }
        public string Batch_number { get; set; }
        public string Bin { get; set; }
        public string Qty { get; set; }
        public string ITR { get; set; }
        public Nullable<bool> Calculate { get; set; }
        public Nullable<int> Id_akun { get; set; }
    
        public virtual TB_Akun TB_Akun { get; set; }
    }
}