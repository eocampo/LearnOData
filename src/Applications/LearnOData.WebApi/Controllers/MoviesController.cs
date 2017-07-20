using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using LearnOData.Model;
using LearnOData.Data;

namespace LearnOData.WebApi.Controllers
{
    public class MoviesController : ODataController
    {
        private MoviesDbContext context = new MoviesDbContext();

        [ODataRoute("Movies")]
        public IHttpActionResult GetAllMovies() {
            return Ok(context.Movies);
            //return NotFound();
        }
        
        [HttpGet]
        [ODataRoute("Movies({key})")]
        public IHttpActionResult Get([FromODataUri] int key) {
            var movie = context.Movies.Find(key);
            if (movie != null)
                return Ok(movie);
            else
                return NotFound();
        }

        protected override void Dispose(bool disposing) {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}