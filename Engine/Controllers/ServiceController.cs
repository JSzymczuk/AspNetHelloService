using Engine.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Engine.Controllers
{
    [EnableCors(origins: "http://localhost:64934", headers: "*", methods: "*")]
    public class ServiceController : ApiController
    {
        AppDbContext db = new AppDbContext();
        
        [HttpGet]
        public HttpResponseMessage Get()
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (var entry in db.Entries.ToList())
            {
                sb.AppendFormat("{0},", ToJSon(entry));
            }
            sb.Append("]");
            return Request.CreateResponse(HttpStatusCode.OK, sb.ToString());
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            long outId;
            if (long.TryParse(id, out outId))
            {
                Entry entry = db.Entries.FirstOrDefault(e => e.Id == outId);
                if (entry != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ToJSon(entry));
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Entry not found.");
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync()
        {
            try
            {
                // Jeśli przesłano formularz innego typu niż multipart:
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.UnsupportedMediaType, "Unsupported Media Type.");
                }

                // Wczytaj przesłane dane
                var provider = await Request.Content.ReadAsMultipartAsync();

                Entry entry = new Entry { DateCreated = DateTime.Now };

                // Przeanalizuj dane pole po polu i uzupełnij tworzony obiekt
                foreach (var stream in provider.Contents)
                {
                    string fieldName = GetFieldName(stream);
                    
                    if (fieldName == "Name")
                    {
                        entry.Name = await stream.ReadAsStringAsync();
                    }
                    else if (fieldName == "Value")
                    {
                        entry.Value = int.Parse(await stream.ReadAsStringAsync());
                    }
                    else if (fieldName == "FileContent")
                    {
                        entry.FileContent = await stream.ReadAsByteArrayAsync();
                    }
                }

                db.Entries.Add(entry);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, ToJSon(entry));
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
        
        [HttpPut]
        public HttpResponseMessage Put(string id)
        {
            // Roboczo PUT służy obecnie do przesyłania obrazu do klienta
            long outId;
            if (long.TryParse(id, out outId))
            {
                Entry entry = db.Entries.FirstOrDefault(e => e.Id == outId);
                if (entry != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Helpers.Utils.ToImageSource(entry.FileContent));
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Entry not found.");
            //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new NotImplementedException().Message);
        }

        [HttpDelete]
        public HttpResponseMessage Delete(string id)
        {
            long outId;
            if (long.TryParse(id, out outId))
            {
                Entry entry = db.Entries.Find(outId);
                if (entry != null)
                {
                    db.Entries.Remove(entry);
                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, "Entry deleted successfully.");
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Entry not found.");
        }

        private static string GetFieldName(HttpContent httpContent)
        {
            return httpContent.Headers.ContentDisposition.Name
                .TrimStart('\"').TrimEnd('\"');
        }

        private static string ToJSon(Entry entry)
        {
            return string.Format("{{\"Id\":\"{0}\",\"Name\":\"{1}\",\"Value\":\"{2}\"}}",
                        entry.Id, entry.Name, entry.Value);
        }
    }
}