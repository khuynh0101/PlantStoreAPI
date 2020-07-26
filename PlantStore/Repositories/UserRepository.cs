using PlantStoreAPI.DataModel;
using PlantStoreAPI.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;

namespace PlantStoreAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private PlantStoreDBContext _context = null;

        public UserRepository()
        {
            _context = new PlantStoreDBContext();
        }
        public Contact GetUserContactInformation()
        {
            Contact contact = null;
            string user = Thread.CurrentPrincipal.Identity.Name;
            using (_context)
            {
                try
                {
                    _context.Database.Initialize(force: false);
                    using (var cmd = _context.Database.Connection.CreateCommand())
                    {
                        DbParameter parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@username";
                        parameter.Value = user;
                        cmd.Parameters.Add(parameter);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[SPGetContactInformation]";
                        _context.Database.Connection.Open();
                        var reader = cmd.ExecuteReader();

                        BillingContact billing = ((IObjectContextAdapter)_context)
                .ObjectContext
                .Translate<BillingContact>(reader).SingleOrDefault();

                        reader.NextResult();
                        ShippingContact shipping = ((IObjectContextAdapter)_context)
                            .ObjectContext
                            .Translate<ShippingContact>(reader).SingleOrDefault();

                        contact = new Contact();
                        contact.BillingContact = billing;
                        contact.ShippingContact = shipping;
                    }
                }
                catch (Exception ex)
                {
                    CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error getting user contact information", ex));
                }
            }
            return contact;
        }

        public string UpdateContacts(BillingContact contactBilling, ShippingContact contactShipping)
        {
            try
            {
                using (_context)
                {
                    _context.SPUpdateContactInfo(Thread.CurrentPrincipal.Identity.Name, contactBilling.FirstName, contactBilling.LastName, contactBilling.Address, contactBilling.Apt, contactBilling.City, contactBilling.State, contactBilling.ZipCode, contactBilling.PhoneNumber, "", false);
                    _context.SPUpdateContactInfo(Thread.CurrentPrincipal.Identity.Name, contactShipping.FirstName, contactShipping.LastName, contactShipping.Address, contactShipping.Apt, contactShipping.City, contactShipping.State, contactShipping.ZipCode, contactShipping.PhoneNumber, "", true);
                    _context.SaveChanges();
                    return "Saved";
                }
            }
            catch (Exception ex)
            {
                CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error saving user contact information", ex));
                return "Failed";
            }
        }
        public string RegisterUser(User user)
        {
            string result = "";
            SecurityRepository security = new SecurityRepository();
            try
            {
                result = security.AddUser(user);
            }
            catch (Exception ex)
            {
                CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error registering user", ex));
                result = "Failed";
            }
            return result;
        }

        public string ResetPassword(User user)
        {
            string result = "";
            SecurityRepository security = new SecurityRepository();
            try
            {
                result = security.ResetPassword(user);
            }
            catch (Exception ex)
            {
                CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error resetting password", ex));
                result = "Failed";
            }
            return result;
        }

        public int SaveOrder(OrderInformation orderInformation)
        {
            int orderNumber = new Random().Next(1, 10000);

            try
            {
                using (_context)
                {
                    Order order = new Order() { OrderId = orderNumber, Date = DateTime.Now };
                    order.UserName = orderInformation.Email;
                    foreach (OrderDetail orderDetail in orderInformation.OrderDetails)
                    {
                        orderDetail.OrderId = orderNumber;
                    }
                    orderInformation.BillingContact.OrderId = orderInformation.ShippingContact.OrderId = orderNumber;

                    order.OrderDetails = orderInformation.OrderDetails;
                    order.OrderShippingContacts = orderInformation.ShippingContact;
                    order.OrderBillingContacts = orderInformation.BillingContact;
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                orderNumber = -1;
                CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error saving order information", ex));
            }
            return orderNumber;
        }

        public OrderInformation RetrieveOrder(int orderId, string email)
        {
            OrderInformation orderInformation = null;
            using (_context)
            {
                try
                {
                    var order = (from o in _context.Orders
                                 where o.OrderId == orderId &&
                                 o.UserName == email
                                 select o).FirstOrDefault();

                    orderInformation = new OrderInformation()
                    {
                        BillingContact = new OrderBillingContact()
                        {
                            Address = order.OrderBillingContacts.Address,
                            Apt = order.OrderBillingContacts.Apt,
                            City = order.OrderBillingContacts.City,
                            FirstName = order.OrderBillingContacts.FirstName,
                            LastName = order.OrderBillingContacts.LastName,
                            PhoneNumber = order.OrderBillingContacts.PhoneNumber,
                            State = order.OrderBillingContacts.State,
                            ZipCode = order.OrderBillingContacts.ZipCode
                        },

                        ShippingContact = new OrderShippingContact()
                        {
                            Address = order.OrderShippingContacts.Address,
                            Apt = order.OrderShippingContacts.Apt,
                            City = order.OrderShippingContacts.City,
                            FirstName = order.OrderShippingContacts.FirstName,
                            LastName = order.OrderShippingContacts.LastName,
                            PhoneNumber = order.OrderShippingContacts.PhoneNumber,
                            State = order.OrderShippingContacts.State,
                            ZipCode = order.OrderShippingContacts.ZipCode
                        }
                    };

                    orderInformation.OrderDetails = new List<OrderDetail>();
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        orderInformation.OrderDetails.Add(new OrderDetail()
                        {
                            ItemId = orderDetail.ItemId,
                            ItemAmount = orderDetail.ItemAmount,
                            ItemTotal = orderDetail.ItemTotal
                        });
                    }
                }
                catch (Exception ex)
                {
                    CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error retrieving order", ex));
                }
                return orderInformation;

            }
        }
    }
}