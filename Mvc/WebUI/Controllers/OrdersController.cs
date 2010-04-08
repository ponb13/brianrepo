using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainModel.Abstract;
using DomainModel.Entities;

namespace WebUI.Controllers
{
    public class OrdersController : Controller
    {
        private IOrdersRepository _ordersRepository; 

        public OrdersController(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }
        
        public ViewResult List()
        {
            return View(this._ordersRepository.Orders);
        }
    }
}
