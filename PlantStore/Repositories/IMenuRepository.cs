using PlantStoreAPI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantStoreAPI.Repositories
{
    public interface IMenuRepository
    {
        Menu GetMenu();
    }
}