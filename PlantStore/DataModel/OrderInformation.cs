using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantStoreAPI.DataModel
{
    public class OrderInformation
    {
        public string Email { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public OrderBillingContact BillingContact { get; set; }

        public OrderShippingContact ShippingContact { get; set; }

    }
}