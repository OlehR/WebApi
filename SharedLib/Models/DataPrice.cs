using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Models
{
    public class DataPrice
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Article { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal QuantityOpt { get; set; }
        public decimal Rest { get; set; }
        public int ActionType { get; set; }
        public decimal PriceBase { get; set; }
        public decimal MinPercent { get; set; }
        public decimal PriceMin { get; set; }
        public decimal PriceIndicative { get; set; }
        public string PromotionName { get; set; }
        public decimal PriceMain { get; set; }
        public decimal Sum { get; set; }
        public string BarCodes { get; set; }
    }

}
