using ApiTreino.Models;
using ApiTreino.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiTreino.Controllers
{

    [Authorize]
    public class AddressController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Address
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(db.Address.Include(a=> a.User).ToList());
        }

        // GET: api/Address/5
        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            if (db.Address.Find(id) == null)
            {
                return BadRequest();
            }
            if (db.Address.Find(id).Id != userId) {
                return Unauthorized();
            }
            return Ok(db.Address.Find(id));
        }

        // POST: api/Address
        
        [HttpPost]
        public async Task<IHttpActionResult> Post(string cep)
        {
            var address = new Address();
            address = await ApiRequest.SendAsync($"{cep}/json");
            address.Id = RequestContext.Principal.Identity.GetUserId();
            db.Address.Add(address);
            db.SaveChanges();

            return Ok();
        }

        // PUT: api/Address/5
        [HttpPut]
        public IHttpActionResult Put([FromBody] Address address)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            var dbAddress = db.Address.AsNoTracking().FirstOrDefault(a => a.AddressId == address.AddressId);
            address.Id = dbAddress.Id;
            if (dbAddress == null) {
                return BadRequest();
            }
            if (dbAddress.Id != userId) {
                return Unauthorized();
            }
            db.Entry(address).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        // DELETE: api/Address/5
        [Route("api/address/{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            var dbAddress = db.Address.FirstOrDefault(a => a.AddressId == id);
            if (dbAddress == null)
            {
                return BadRequest();
            }
            if (dbAddress.Id != userId)
            {
                return Unauthorized();
            }
            db.Address.Remove(db.Address.Find(id));
            db.SaveChanges();
            return Ok();
        }
    }
}
