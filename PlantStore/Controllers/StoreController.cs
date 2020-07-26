
using PlantStoreAPI.DataModel;
using PlantStoreAPI.Repositories;
using PlantStoreAPI.Security;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PlantStoreAPI.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class StoreController : ApiController
    {
        public HttpResponseMessage GetNavigationMenu()
        {
            MenuRepository repo = new MenuRepository();
            List<Menu> menus = repo.GetMenu();
            if (menus == null)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, menus);
        }

        public HttpResponseMessage GetAllProducts()
        {
            ProductRepository repo = new ProductRepository();
            List<Product> products = repo.GetAllProducts();
            if (products == null)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, products);
        }

        [HttpPost]
        public HttpResponseMessage SaveOrder([FromBody] OrderInformation orderInformation)
        {
            UserRepository repo = new UserRepository();
            int orderNumber = repo.SaveOrder(orderInformation);
            if (orderNumber < 0)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            else
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new { OrderNumber = orderNumber });
        }

        [HttpGet]
        public HttpResponseMessage RetrieveOrder(int orderId, string email)
        {
            UserRepository repo = new UserRepository();
            OrderInformation order = repo.RetrieveOrder(orderId, email);
            if (order == null)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            else
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new { Order = order });
        }
    }
}
