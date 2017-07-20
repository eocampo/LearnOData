using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.OData.Edm;
using System.Web.OData.Builder;
using LearnOData.Model;

namespace LearnOData.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //// Web API routes
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.MapODataServiceRoute("ODataRoute", "odata", GetEdmModel());

            config.EnsureInitialized();
        }

        private static IEdmModel GetEdmModel() {
            var builder = new ODataConventionModelBuilder();
            builder.Namespace = "LearnOData";
            builder.ContainerName= "LearnODataContainer";

            builder.EntitySet<Person>("People");
            builder.EntitySet<Movie>("Movies");

            return builder.GetEdmModel();
        }
    }
}
