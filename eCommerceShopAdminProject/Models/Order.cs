using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceShopAdminProject.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ProductInOrder> Products { get; set; }
    }
}
