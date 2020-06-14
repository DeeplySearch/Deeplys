using GraduateProject.Models;
using GraduateProject.Models.AddData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GraduateProject.Controllers
{
    [Authorize]
    public class DataController : Controller
    {


        // GET: Data
        public ActionResult Index()
        {
            var Categories = new List<Catygory>()
            { new Catygory(){Id=1,Text="api"},
              new Catygory(){Id=2,Text="CV"},
              new Catygory(){Id=3,Text="Online Work"},
              new Catygory(){Id=4,Text="Program SetUp"},
              new Catygory(){Id=5,Text="Learning Website"},
              new Catygory(){Id=6,Text="Online Tool"}
            };
            ViewBag.list = Categories;
            return View();
        }

        [HttpPost]
        public ActionResult Index(DataInfo data)
        {
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {
                if(data.Name==null)
                {
                    ViewBag.message = "Name Can't be empty";
                    return View();
                }
                else if(data.URL == null)
                {
                    ViewBag.message = "Url Can't be empty";
                    return View();
                }
                else
                {
                    data.UserEmail = User.Identity.Name;
                    d.DataInfoes.AddOrUpdate(data);

                    var u = from s in d.Users
                            select s;
                    u = u.Where(s => s.EmailID == User.Identity.Name);
                    foreach (User user in u)
                        data.UserId = user.UserID;

                    d.SaveChanges();
                    return RedirectToAction("Data");
                }
               

            }
        }


        public ActionResult Data()
        {
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {
                User user = (from u in d.Users
                             where u.EmailID == User.Identity.Name
                             select u).FirstOrDefault();

                var data = from da in d.DataInfoes
                           where da.UserEmail == user.EmailID
                           select da;
                return View(data.ToList());
            }
        }


        public ActionResult Delete(int id)
        {
            using (MyDatabaseEntities myDatabase = new MyDatabaseEntities())
            {
                return View(myDatabase.DataInfoes.Where(x => x.Id == id).FirstOrDefault());
            }
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                using (MyDatabaseEntities dd = new MyDatabaseEntities())
                {
                    DataInfo data = dd.DataInfoes.Where(x => x.Id == id).FirstOrDefault();
                    dd.DataInfoes.Remove(data);
                    dd.SaveChanges();
                    return RedirectToAction("Data");
                    ;
                }

            }
            catch
            {
                return View();
            }

        }

        public ActionResult Edit(int id)
        {
            var Categories = new List<Catygory>()
            { new Catygory(){Id=1,Text="api"},
              new Catygory(){Id=2,Text="CV"},
              new Catygory(){Id=3,Text="Online Work"},
              new Catygory(){Id=4,Text="Program SetUp"},
              new Catygory(){Id=5,Text="Learning Website"},
              new Catygory(){Id=6,Text="Online Tool"}
            };
            ViewBag.list = Categories;
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {
                var data = (from item in d.DataInfoes
                            where item.Id == id
                            select item).First();
                d.DataInfoes.Remove(data);
                d.SaveChanges();
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult Edit(DataInfo data)
        {
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {

                data.UserEmail = User.Identity.Name;
                d.DataInfoes.AddOrUpdate(data);

                d.SaveChanges();
                var DataRecords = d.DataInfoes.ToList().Where(x => x.UserEmail == User.Identity.Name);
                return View("Data", DataRecords);
            }

        }


        [Authorize(Users = "ghazihaddad3@gmail.com")]
        public ActionResult IndexAdmin()
        {
            var Categories = new List<Catygory>()
            { new Catygory(){Id=1,Text="api"},
              new Catygory(){Id=2,Text="CV"},
              new Catygory(){Id=3,Text="Online Work"},
              new Catygory(){Id=4,Text="Program SetUp"},
              new Catygory(){Id=5,Text="Learning Website"},
              new Catygory(){Id=6,Text="Online Tool"}
            };
            ViewBag.list = Categories;
            return View();
        }

        [HttpPost]
        public ActionResult IndexAdmin(DataInfo data)
        {
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {

                d.DataInfoes.AddOrUpdate(data);

                //var u = from s in d.Users
                //        select s;
                //u = u.Where(s => s.EmailID == User.Identity.Name);
                d.SaveChanges();
                return RedirectToAction("Data");

            }
        }

        [Authorize(Users = "ghazihaddad3@gmail.com")]
        public ActionResult DataAdmin()
        {
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {


                var data = from da in d.DataInfoes
                           where da.UserEmail != null && (da.Accepted == false || da.Accepted == null)
                           select da;
                return View(data.ToList());
            }
        }


        [Authorize(Users = "ghazihaddad3@gmail.com")]
        public ActionResult EditAdmin(int id)
        {
            var Categories = new List<Catygory>()
            { new Catygory(){Id=1,Text="api"},
              new Catygory(){Id=2,Text="CV"},
              new Catygory(){Id=3,Text="Online Work"},
              new Catygory(){Id=4,Text="Program SetUp"},
              new Catygory(){Id=5,Text="Learning Website"},
              new Catygory(){Id=6,Text="Online Tool"}
            };
            ViewBag.list = Categories;
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {
                var data = (from item in d.DataInfoes
                            where item.Id == id
                            select item).First();
                d.DataInfoes.Remove(data);
                d.SaveChanges();
                return View(data);
            }
        }


        [HttpPost]
        public ActionResult EditAdmin(DataInfo data)
        {
            using (MyDatabaseEntities d = new MyDatabaseEntities())
            {


                d.DataInfoes.AddOrUpdate(data);

                d.SaveChanges();
                var DataRecords = d.DataInfoes.ToList();
                return View("Data", DataRecords);
            }
        }
        [Authorize(Users = "ghazihaddad3@gmail.com")]
        public ActionResult Accept(int id)
        {
            using (MyDatabaseEntities myDatabase = new MyDatabaseEntities())
            {
                return View(myDatabase.DataInfoes.Where(x => x.Id == id).FirstOrDefault());
            }

        }

        [HttpPost]
        public ActionResult Accept(int id, FormCollection collection)
        {
            try
            {
                using (MyDatabaseEntities dd = new MyDatabaseEntities())
                {
                    DataInfo data = dd.DataInfoes.Where(x => x.Id == id).FirstOrDefault();
                    data.Accepted = true;
                    dd.DataInfoes.Add(data);
                    DataInfo data1 = dd.DataInfoes.Where(x => x.Id == id && x.UserEmail != null).FirstOrDefault();
                    dd.DataInfoes.Remove(data1);
                    dd.SaveChanges();
                    return RedirectToAction("DataAdmin");

                }

            }
            catch
            {
                return View();
            }

        }
    }
}