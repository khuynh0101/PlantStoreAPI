using PlantStoreAPI.DataModel;
using PlantStoreAPI.Repositories;
using PlantStoreAPI.Security;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PlantStoreAPI.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        [BasicAuthentication]
        [HttpGet]
        public HttpResponseMessage GetUserContactInformation()
        {
            UserRepository repo = new UserRepository();
            Contact result = repo.GetUserContactInformation();
            if (result == null)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { content = result });
        }

        [BasicAuthentication]
        [HttpPost]
        public HttpResponseMessage UpdateContacts([FromBody] Contact contact)
        {
            UserRepository repo = new UserRepository();
            string result = repo.UpdateContacts(contact.BillingContact, contact.ShippingContact);
            if (result == "Failed")
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { content = result });
        }

        [HttpPost]
        public HttpResponseMessage SignIn()
        {
            SecurityRepository repo = new SecurityRepository();
            try
            {
                bool isSignedIn = repo.SignIn(Request);
                return Request.CreateResponse(HttpStatusCode.OK, new { content = isSignedIn });
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
        }

        [HttpPost]
        public HttpResponseMessage Register([FromBody] User user)
        {
            UserRepository repo = new UserRepository();
            string result = repo.RegisterUser(user);
            if (result == "Failed")
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { content = result });
        }

        [HttpPost]
        public HttpResponseMessage Reset([FromBody] User user)
        {
            user.UserName = WebUtility.UrlDecode(user.UserName);
            UserRepository repo = new UserRepository();
            string result = repo.ResetPassword(user);
            if (result == "Failed")
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { content = result });
        }

        [HttpPost]
        public HttpResponseMessage SendResetLink([FromBody] User user)
        {
            SecurityRepository repo = new SecurityRepository();
            string result = repo.GenerateToken(user);
            if (result == null)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Error occurred.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { content = WebUtility.UrlEncode(result) });
        }
    }
}
