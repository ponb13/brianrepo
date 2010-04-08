using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using DomainModel.Abstract;
using DomainModel.Entities;

namespace WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductsRepository _productRepository;

        public NavController(IProductsRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ViewResult Menu()
        {
            List<NavLink> navLinks = new List<NavLink>();

            navLinks.Add(new CategoryLink(null));

            var categories = _productRepository.Products.Select(p => p.Category);

            foreach (var category in categories.Distinct().OrderBy(p => p))
            {
                navLinks.Add(new CategoryLink(category));
            }

            return View(navLinks);
        }
    }

    public class NavLink
    {
        public string Text { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }

    public class CategoryLink : NavLink
    {
        public CategoryLink(string category)
        {
            Text = category ?? "Home";
            RouteValues = new RouteValueDictionary(new {controller = "Products", action="List", category=category});
        }
    }
}
