using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Engine.Controllers
{
    public class EntriesController : Controller
    {
        AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            return View(db.Entries.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                Entry entry = new Entry
                {
                     DateCreated = DateTime.Now,
                     Name = collection.Get("Name"),
                     Value = int.Parse(collection.Get("Value"))
                };

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files.Get(0);
                    using (var reader = new System.IO.BinaryReader(file.InputStream))
                    {
                        entry.FileContent = reader.ReadBytes(file.ContentLength);
                    }
                }

                db.Entries.Add(entry);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(long? id)
        {
            if (id.HasValue)
            {
                Entry entry = db.Entries.Find(id);
                if (entry != null)
                {
                    db.Entries.Remove(entry);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
