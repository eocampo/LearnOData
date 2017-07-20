using Microsoft.OData;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
//using Microsoft.OData.UriParser; // .UriParser;

//using System.Net.Http;
//using System.Web.Http.Routing;
//using System.Web.OData.Extensions;
//using System.Web.OData.Routing;
//using Microsoft.OData.Core;
//using Microsoft.OData.Core.UriParser;


//using Microsoft.OData.Core.UriParser;

namespace LearnOData.WebApi.Helpers
{
    /// <summary>
    /// OData Helper methods - slightly adjusted from OData helpers provided by Microsoft
    /// </summary>
    public static class ODataHelpers
    {
        public static bool HasProperty(this object instance, string propertyName) {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            return (propertyInfo != null);
        }

        public static object GetValue(this object instance, string propertyName) {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            if (propertyInfo == null) {
                throw new HttpException("Can't find property with name " + propertyName);
            }
            var propertyValue = propertyInfo.GetValue(instance, new object[] { });

            return propertyValue;
        }

        public static IHttpActionResult CreateOKHttpActionResult(this ODataController controller, object propertyValue) {
            var okMethod = default(MethodInfo);

            // find the ok method on the current controller
            var methods = controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var method in methods) {
                if (method.Name == "Ok" && method.GetParameters().Length == 1) {
                    okMethod = method;
                    break;
                }
            }

            // invoke the method, passing in the propertyValue
            okMethod = okMethod.MakeGenericMethod(propertyValue.GetType());
            var returnValue = okMethod.Invoke(controller, new object[] { propertyValue });
            return (IHttpActionResult)returnValue;
        }


        ///// <summary>
        ///// Helper method to get the odata path for an arbitrary odata uri.
        ///// </summary>
        ///// <param name="request">The request instance in current context</param>
        ///// <param name="uri">OData uri</param>
        ///// <returns>The parsed odata path</returns>
        //public static ODataPath CreateODataPath(this HttpRequestMessage request, Uri uri) {
        //    if (uri == null) {
        //        throw new ArgumentNullException("uri");
        //    }

        //    var newRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        //    var route = request.GetRouteData().Route;

        //    var newRoute = new HttpRoute(
        //        route.RouteTemplate,
        //        new HttpRouteValueDictionary(route.Defaults),
        //        new HttpRouteValueDictionary(route.Constraints),
        //        new HttpRouteValueDictionary(route.DataTokens),
        //        route.Handler);

        //    var routeData = newRoute.GetRouteData(request.GetConfiguration().VirtualPathRoot, newRequest);
        //    if (routeData == null) {
        //        throw new InvalidOperationException("This link is not a valid OData link.");
        //    }

        //    return newRequest.ODataProperties().Path;
        //}

        //public static TKey GetKeyValue<TKey>(this HttpRequestMessage request, Uri uri) {
        //    if (uri == null) {
        //        throw new ArgumentNullException("uri");
        //    }

        //    //get the odata path Ex: ~/entityset/key/$links/navigation
        //    var odataPath = request.CreateODataPath(uri);
        //    var keySegment = odataPath.Segments.OfType<KeyValuePathSegment>.LastOrDefault();    // .OfType<Microsoft.OData.UriParser   System.Web.OData.Routing.ODataSegmentKinds KeyValuePathSegment>().LastOrDefault();
        //    if (keySegment == null) {
        //        throw new InvalidOperationException("This link does not contain a key.");
        //    }
            
        //    var value =  ODataUriUtils.ConvertFromUriLiteral("keySegment.Value", Microsoft.OData.ODataVersion.V4);
        //    return (TKey)value;
        //}


        // https://github.com/OData/WebApi/issues/158

        //public static TKey GetKeyFromUri<TKey>(HttpRequestMessage request, Uri uri) {
        //    if (uri == null) {
        //        throw new ArgumentNullException("uri");
        //    }

        //    var urlHelper = request.GetUrlHelper() ?? new UrlHelper(request);

        //    string serviceRoot = urlHelper.CreateODataLink(
        //        request.ODataProperties().RouteName,
        //        request.ODataProperties().PathHandler, new List<ODataPathSegment>());
        //    var odataPath = request.ODataProperties().PathHandler.Parse(
        //        request.ODataProperties().Model,
        //        serviceRoot, uri.LocalPath);

        //    var keySegment = odataPath.Segments.OfType<KeyValuePathSegment>().FirstOrDefault();
        //    if (keySegment == null) {
        //        throw new InvalidOperationException("The link does not contain a key.");
        //    }

        //    var value = ODataUriUtils.ConvertFromUriLiteral(keySegment.Value, ODataVersion.V4);
        //    return (TKey)value;
        //}
    }
}
