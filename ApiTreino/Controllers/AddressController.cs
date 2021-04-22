using ApiTreino.Models;
using ApiTreino.Services;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiTreino.Controllers
{

    [Authorize]
    public class AddressController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Address
        [HttpGet]
        public IHttpActionResult Get()
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            return Ok(db.Address.Where(a => a.Id == userId).Include(a => a.User).ToList());
        }

        // GET: api/Address/5
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            if (db.Address.Find(id) == null)
            {
                return BadRequest();
            }
            if (db.Address.Find(id).Id != userId)
            {
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
            if (dbAddress == null)
            {
                return BadRequest();
            }
            if (dbAddress.Id != userId)
            {
                return Unauthorized();
            }
            db.Entry(address).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        // DELETE: api/Address/5
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

        
        [Route("Excel")]
        [HttpGet]
        public async Task<HttpResponseMessage> Excel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var userId = RequestContext.Principal.Identity.GetUserId();
            var fichas = await db.Address.Where(a => a.Id == userId).ToListAsync();
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "ApiTreino";
                excelPackage.Workbook.Properties.Title = "Ficha de Endereços";
                var sheet = excelPackage.Workbook.Worksheets.Add("Ficha de Endereços");
                sheet.Name = "Ficha de Endereços";

                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Cep";
                sheet.Cells[1, 3].Value = "Logradouro";
                sheet.Cells[1, 4].Value = "Complemento";
                sheet.Cells[1, 5].Value = "Bairro";
                sheet.Cells[1, 6].Value = "Localidade";
                sheet.Cells[1, 7].Value = "Uf";
                sheet.Cells[1, 8].Value = "IBGE";
                sheet.Cells[1, 9].Value = "GIA";
                sheet.Cells[1, 10].Value = "DDD";
                sheet.Cells[1, 11].Value = "Siafi";

                var i = 2;
                foreach (var item in fichas.ToList())
                {

                    sheet.Cells[i, 1].Value = item.Id;
                    sheet.Cells[i, 2].Value = item.Cep;
                    sheet.Cells[i, 3].Value = item.Logradouro;
                    sheet.Cells[i, 4].Value = item.Complemento;
                    sheet.Cells[i, 5].Value = item.Bairro;
                    sheet.Cells[i, 6].Value = item.Localidade;
                    sheet.Cells[i, 7].Value = item.Uf;
                    sheet.Cells[i, 8].Value = item.Ibge;
                    sheet.Cells[i, 9].Value = item.Gia;
                    sheet.Cells[i, 10].Value = item.Ddd;
                    sheet.Cells[i, 11].Value = item.Siafi;
                    i++;
                }

                var headerCells = sheet.Cells[1, 1, 1, sheet.Dimension.Columns];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002f6c"));
                headerCells.Style.Font.Color.SetColor(Color.White);

                sheet.Cells.AutoFitColumns();
                var file = excelPackage.GetAsByteArray();
                // processing the stream.
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(file)
                };
                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "FichaDeEndereco.xlsx"
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.ms-excel");
                return result;

            }
        }
        
    }
}


//var stream = new MemoryStream();
//// processing the stream.
//var result = new HttpResponseMessage(HttpStatusCode.OK)
//{
//    Content = new ByteArrayContent(stream.ToArray())
//};
//result.Content.Headers.ContentDisposition =
//    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
//    {
//        FileName = "CertificationCard.pdf"
//    };
//result.Content.Headers.ContentType =
//    new MediaTypeHeaderValue("application/octet-stream");
//return result;