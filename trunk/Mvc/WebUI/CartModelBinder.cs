using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DomainModel.Entities;

namespace WebUI
{
    public class CartModelBinder : IModelBinder
    {
        private const string _cartSessionKey = "_cart";
        
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if(bindingContext.Model!=null)
            {
                throw new InvalidOperationException("cannot update instances");
            }

            Cart cart = (Cart) controllerContext.HttpContext.Session[_cartSessionKey];
            if(cart == null)
            {
                cart = new Cart();
                controllerContext.HttpContext.Session[_cartSessionKey] = cart;
            }

            return cart;
        }


    }
}
