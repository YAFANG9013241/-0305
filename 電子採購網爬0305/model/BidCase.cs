using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 電子採購網爬0305.model
{
    public class BidCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int 項次 { get; set; }

        [MaxLength(100)]
        public string 機關名稱 { get; set; }

        public string 標案案號標案名稱 { get; set; }

        public string 傳輸次數 { get; set; }

        [MaxLength(100)]
        public string 招標方式 { get; set; }

        [MaxLength(50)]
        public string 採購性質 { get; set; }

        public DateTime? 公告日期 { get; set; }
        public DateTime? 截止投標 { get; set; }

        public decimal? 預算金額 { get; set; }
    }
}