using PlantStoreAPI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantStoreAPI.Repositories
{
    public interface IUserRepository
    {
        Contact GetUserContactInformation();
        string RegisterUser(User user);
        string UpdateContacts(BillingContact contactBilling, ShippingContact contactShipping);

        string ResetPassword(User user);

        int SaveOrder(OrderInformation orderInformation);
        OrderInformation RetrieveOrder(int orderId, string email);
    }
}