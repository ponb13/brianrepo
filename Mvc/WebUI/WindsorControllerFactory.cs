using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core;

namespace WebUI
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private WindsorContainer _container;

        public WindsorControllerFactory()
        {
            _container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

            var controllerTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where typeof (IController).IsAssignableFrom(t)
                                  select t;
            foreach (Type t in controllerTypes)
            {
                _container.AddComponentLifeStyle(t.FullName, t, LifestyleType.Transient);
            }
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return (IController) _container.Resolve(controllerType);
        }
    }
}
