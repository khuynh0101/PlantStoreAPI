using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlantStoreAPI.Repositories;

namespace UnitTestPlantStore
{
    [TestClass]
    public class UnitTestStore
    {
        [TestMethod]
        public void TestGetMenu_ShouldReturnMenuItems()
        {
            MenuRepository menuRepo = new MenuRepository();
            Assert.IsTrue(menuRepo.GetMenu().Count > 0);
        }

        [TestMethod]
        public void TestGetAllProducts_ShouldReturnAllProducts()
        {
            ProductRepository productRepo = new ProductRepository();
            Assert.IsTrue(productRepo.GetAllProducts().Count > 0);
        }
    }
}
