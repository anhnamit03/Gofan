using System;
using System.Collections.Generic;
using System.Text;

namespace GoFan.Domain.Promotions
{
    public class ProductPromotion
    {
        public int Id { get; set; }
        public  int ProductId { get; set; }
        public int PromotionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
