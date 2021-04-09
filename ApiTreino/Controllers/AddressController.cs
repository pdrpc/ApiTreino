using ApiTreino.Models;
using ApiTreino.Services;
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
    public class AddressController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Address
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(db.Address.ToList());
        }

        // GET: api/Address/5
        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            return Ok(db.Address.Find(id));
        }

        // POST: api/Address
        
        [HttpPost]
        public async Task<IHttpActionResult> Post(string cep)
        {
            var address = new Address();
            address = await ApiRequest.SendAsync($"{cep}/json");
            db.Address.Add(address);
            db.SaveChanges();

            return Ok();
        }

        // PUT: api/Address/5
        [HttpPut]
        public void Put([FromBody] Address address)
        {
            db.Entry(address).State = EntityState.Modified;
            db.SaveChanges(); ;
        }

        // DELETE: api/Address/5
        [Route("api/address/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
            db.Address.Remove(db.Address.Find(id));
            db.SaveChanges();
        }
    }
}
