using PlantStoreAPI.DataModel;
using PlantStoreAPI.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace PlantStoreAPI.Repositories
{
    public class MenuRepository
    {
        private readonly PlantStoreDBContext _context;

        public MenuRepository()
        {
            _context = new PlantStoreDBContext();
        }
        public List<Menu> GetMenu()
        {
            List<Menu> menus = null;
            try
            {
                menus = _context.Menus.ToList();
            }
            catch (Exception ex)
            {
                CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error getting menu list", ex));
            }
            return menus;
        }
    }
}