using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using LearnOData.Model;
using LearnOData.Data;
using LearnOData.WebApi.Helpers;
using System.Net;

namespace LearnOData.WebApi.Controllers
{
    public class PeopleController : ODataController
    {
        private MoviesDbContext context = new MoviesDbContext();

        public IHttpActionResult Get() {
            return Ok(context.People);
            //return NotFound();
        }

        public IHttpActionResult Get([FromODataUri] int key) {
            var person = context.People.Find(key);
            if (person!=null)
                return Ok(person);
            else
                return NotFound();
        }

        [HttpGet]
        [ODataRoute("People({key})/Email")]
        [ODataRoute("People({key})/FirstName")]
        [ODataRoute("People({key})/LastName")]
        public IHttpActionResult GetPersonProperty([FromODataUri] int key) {
            var person = context.People.Find(key);
            if (person == null)
                return NotFound();

            var propertyToGet = this.Url.Request.RequestUri.Segments.Last();

            if (!person.HasProperty(propertyToGet))
                return NotFound();

            var propertyValue = person.GetValue(propertyToGet);

            if (propertyValue == null)
                return StatusCode(HttpStatusCode.NoContent);

            //return Ok(propertyValue);
            return this.CreateOKHttpActionResult(propertyValue);
        }

        // Getting a collection property
        [HttpGet]
        [EnableQuery]
        [ODataRoute("People({key})/Friends")]
        // [ODataRoute("People({key})/VinylRecords")]
        public IHttpActionResult GetPersonCollectionProperty([FromODataUri] int key) {
            var collectionPropertyToGet = Url.Request.RequestUri.Segments.Last();
            var person = this.context.People.Include(collectionPropertyToGet)
                .FirstOrDefault(p => p.PersonId == key);

            if (person == null) {
                return NotFound();
            }

            if (!person.HasProperty(collectionPropertyToGet)) {
                return NotFound();
            }

            var collectionPropertyValue = person.GetValue(collectionPropertyToGet);

            // return the collection
            return this.CreateOKHttpActionResult(collectionPropertyValue);
        }

        [HttpGet]
        [ODataRoute("People({key})/Movies")]        
        public IHttpActionResult GetMoviesCollectionProperty([FromODataUri] int key) {
            var collectionPropertyToGet = this.Url.Request.RequestUri.Segments.Last();

            var person = context.People.Include(collectionPropertyToGet).FirstOrDefault(p => p.PersonId == key);

            if (person == null)
                return NotFound();

            var collectionPropertyValue = person.GetValue(collectionPropertyToGet);

            return this.CreateOKHttpActionResult(collectionPropertyValue);
        }

        [HttpGet]
        [ODataRoute("People({key})/Email/$value")]
        [ODataRoute("People({key})/FirstName/$value")]
        [ODataRoute("People({key})/LastName/$value")]
        public IHttpActionResult GetPersonPropertyRawValue([FromODataUri] int key) {
            var person = context.People.Find(key);
            if (person == null)
                return NotFound();

            var propertyToGet = this.Url.Request.RequestUri.Segments[this.Url.Request.RequestUri.Segments.Length - 2].TrimEnd('/');

            if (!person.HasProperty(propertyToGet))
                return NotFound();

            var propertyValue = person.GetValue(propertyToGet);

            if (propertyValue == null)
                return StatusCode(HttpStatusCode.NoContent);

            //return Ok(propertyValue);
            return this.CreateOKHttpActionResult(propertyValue.ToString());
        }

        public IHttpActionResult Post(Person person) {
            if (!this.ModelState.IsValid)
                return BadRequest(this.ModelState);

            this.context.People.Add(person);
            this.context.SaveChanges();

            return Created(person);
        }

        public IHttpActionResult Put([FromODataUri] int key, Person person) {
            if (!this.ModelState.IsValid)
                return BadRequest(this.ModelState);

            var currentPerson = this.context.People.Find(key);
            if (currentPerson == null)
                return NotFound();

            // person.PersonId = key;
            person.PersonId = currentPerson.PersonId;
            context.Entry(currentPerson).CurrentValues.SetValues(person);
            this.context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        public IHttpActionResult Patch([FromODataUri] int key, Delta<Person> patch) {
            if (!this.ModelState.IsValid)
                return BadRequest(this.ModelState);

            var currentPerson = this.context.People.Find(key);
            if (currentPerson == null)
                return NotFound();

            // person.PersonId = key;
            //person.PersonId = currentPerson.PersonId;

            patch.TrySetPropertyValue("PersonId", currentPerson.PersonId);
            patch.Patch(currentPerson);                        
            this.context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        public IHttpActionResult Delete([FromODataUri] int key) {

            //var person = context.People.Include("Friends").FirstOrDefault(p => p.PersonId == key);

            var currentPerson = this.context.People.FirstOrDefault(p => p.PersonId == key);

            if (currentPerson == null)
                return NotFound();

            //// this person might be another person's friend, we
            //// need to this person from their friend collections
            //var peopleWithCurrentPersonAsFriend =
            //    this.context.People.Include("Friends")
            //    .Where(p => p.Friends.Select(f => f.PersonId).AsQueryable().Contains(key));

            //foreach (var person in peopleWithCurrentPersonAsFriend.ToList()) {
            //    person.Friends.Remove(currentPerson);
            //}

            this.context.People.Remove(currentPerson);
            this.context.SaveChanges();

            // return No Content
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ODataRoute("People({key})/Friends/$ref")]
        public IHttpActionResult CreateLinkToFriend([FromODataUri] int key, [FromBody] Uri link) {

            var currentPerson = this.context.People.Include("Friends").FirstOrDefault(p => p.PersonId == key);

            if (currentPerson == null)
                return NotFound();

            // we need the key value from the passed-in link Uri
            //int keyOfFriendToAdd = 1; // Request.GetKeyValue<int>(link);

            string lastSegment = link.Segments.LastOrDefault();

            string idstring = lastSegment.Substring("People(".Length, lastSegment.LastIndexOf(')') - "People(".Length);

            int keyOfFriendToAdd = 1; // Request.GetKeyValue<int>(link);

            int.TryParse(idstring, out keyOfFriendToAdd);

            if (currentPerson.Friends.Any(item => item.PersonId == keyOfFriendToAdd)) {
                return BadRequest(string.Format("The person with Id {0} is already linked to the person with Id {1}",
                    key, keyOfFriendToAdd));
            }

            // find the friend
            var friendToLinkTo = this.context.People.FirstOrDefault(p => p.PersonId == keyOfFriendToAdd);
            if (friendToLinkTo == null) {
                return NotFound();
            }

            // add the friend
            currentPerson.Friends.Add(friendToLinkTo);
            this.context.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);            
        }

        protected override void Dispose(bool disposing) {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}