using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Entities;
using NUnit;
using NUnit.Framework;
using DomainModel.Abstract;
using Moq;
using WebUI.Controllers;
using System.Web.Mvc;

namespace Tests
{
    [TestFixture]
    public class ProductControllerTests
    {
        [Test]
        public void List_Presents_Correct_Page_Of_Products()
        {
            IProductsRepository repository = MockProductsRepository(new Product {Name = "P1"},
                                                                        new Product {Name = "P2"},
                                                                        new Product {Name = "P3"},
                                                                        new Product {Name = "P5"});
            
            ProductsController controller = new ProductsController(repository);
            controller.PageSize = 3;
            var result = controller.List(2);
            var products = result.ViewData.Model as IList<Product>;

            Assert.IsNotNull(result, "should not be null");
            Assert.AreEqual(2,products.Count, "product count should be two");
            
        }

        static IProductsRepository MockProductsRepository(params Product[] prods)
        {
            var mockProductsRepos = new Moq.Mock<IProductsRepository>();

            mockProductsRepos.Setup(x => x.Products).Returns(prods.AsQueryable());

            return mockProductsRepos.Object;
        }
    }
}
