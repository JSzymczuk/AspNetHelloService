using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class EntriesController : Controller
    {
        private const string serviceUri = "http://localhost:62263/api/Service/";
        private HttpClient client = new HttpClient();

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(form.Get("Name")), "Name");
                    content.Add(new StringContent(form.Get("Value")), "Value");

                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        var streamContent = new StreamContent(file.InputStream);
                        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "FileContent",
                            FileName = file.FileName
                        };
                        content.Add(streamContent);
                    }

                    HttpResponseMessage response = await client.PostAsync(serviceUri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string message = await response.Content.ReadAsAsync<string>();
                        return Content(message);
                    }
                }
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, e.Message);
            }

            return Content(string.Empty);
        }

        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(string.IsNullOrEmpty(id) ? serviceUri : serviceUri + id);
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsAsync<string>();
                    return Content(message);
                }
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, e.Message);
            }

            return Content(string.Empty);
        }

        [HttpPut]
        public async Task<ActionResult> Put(string id)
        {
            try
            {
                HttpContent content = new StringContent(id);
                HttpResponseMessage response = await client.PutAsync(serviceUri + id, content);
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsAsync<string>();
                    return Content(message);
                }
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, e.Message);
            }

            return Content(string.Empty);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(serviceUri + id);
                if (response.IsSuccessStatusCode)
                {
                    string message = await response.Content.ReadAsAsync<string>();
                    return Content(message);
                }
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, e.Message);
            }

            return Content(string.Empty);
        }
    }
}