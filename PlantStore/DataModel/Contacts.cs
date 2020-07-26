using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantStoreAPI.DataModel
{
    public class Contact
    {
        public BillingContact BillingContact { get; set; }
        public ShippingContact ShippingContact { get; set; }
    }
}