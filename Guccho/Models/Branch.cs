//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Guccho.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Branch
    {
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public int bID { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public int fk_oID { get; set; }
    
        public virtual Organization Organization { get; set; }
    }
}
