using PlantStoreAPI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantStoreAPI.Repositories
{
    public interface ISecurityRepository
    {
        bool ValidateUser(string username, string password);

        string AddUser(User user);
    }
}