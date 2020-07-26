using PlantStoreAPI.DataModel;
using PlantStoreAPI.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantStoreAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        PlantStoreDBContext _context = null;

        public ProductRepository() {
            _context = new PlantStoreDBContext();
        }
        public List<Product> GetAllProducts()
        {
            List<Product> products = null;
            using (_context)
            {
                try
                {
                    products = _context.SPGetProducts((int?)null).ToList();
                }
                catch (Exception ex)
                {
                    CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error getting products", ex));
                }
            }
            return products;
        }

        public List<Product> GetProductsById(int id)
        {
            throw new NotImplementedException();
        }
    }
}