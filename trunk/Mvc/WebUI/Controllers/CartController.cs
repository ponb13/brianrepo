using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainModel.Entities;
using DomainModel.Abstract;

namespace WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductsRepository _productsRepository;

        public CartController(IProductsRepository repo)
        {
            _productsRepository = repo;
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            ViewData["Category"] = cart;
            ViewData["returnUrl"] = returnUrl;

            return View(cart);
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productsRepository.Products.FirstOrDefault(p => p.ProductId == productId);

            cart.AddItem(product, 1);
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productsRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            cart.RemoveLine(product);

            return RedirectToAction("Index", new { returnUrl });
        }

    }
}
