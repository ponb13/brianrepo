using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel.Concrete;
using DomainModel.Entities;

namespace WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private IProductsRepository productsRepository;
        public int PageSize = 4;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        //public int PageSize
        //{
        //    get; 
        //    set;
        //}

        public ViewResult List(int page)
        {
            int numProducts = productsRepository.Products.Count();
            ViewData["TotalPages"] = (int)Math.Ceiling((double)numProducts / this.PageSize);
            ViewData["CurrentPage"] = page;

            return View(this.productsRepository.Products.Skip(((page-1)* this.PageSize))
                                                        .Take(PageSize)
                                                        .ToList());
        }
    }
}
