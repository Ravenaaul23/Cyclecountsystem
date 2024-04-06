using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CycleCountSystem__CSS_.Models
{
    public class CombineViewModel
    {
        public IEnumerable<TB_Cyclecount> CycleCounts { get; set; }
        public IEnumerable<TB_Inventory> Inventories { get; set; }
        public TB_Inventory LastCalculationInventory { get; set; }
        public TB_Inventory Inventorys { get; set; }
        public TB_Cyclecount Cycles { get; set; }
        public TB_User UserModel { get; set; }
        public TB_Role ModelRole { get; set; }
        public List<TB_User> ListUser{ get; set; }
        public List<TB_Role> ListRole { get; set; }
        public List<string> EmailAddresses { get; set; }
        public string LastCalculationId { get; set; }
        public string LastCalculationBatch { get; set; }
        public string LastCalculationBin { get; set; }
        public string LastCalculationPercent { get; set; }
        public List<string> UserNames { get; set; }

        // Properti tambahan
        public int Id { get; set; }
        public decimal DataAktual { get; set; } // Ubah tipe data sesuai kebutuhan
        public string DataAktualString { get; set; } 
        public string SelisihQty { get; set; }
        public string LastCalculationInventoryQty { get; set; }
        public bool ButtonStatus { get; set; }
        public DateTime DateCount { get; set; }
    }
}