using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraduateProject.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraduateProject.Models;
using System.Web.Mvc;
using System.Net.Http;
using System.Data.Entity.Migrations;

namespace GraduateProject.Controllers.Tests
{

    [TestClass()]
    public class DataControllerTests
    {
        MyDatabaseEntities db = new MyDatabaseEntities();

        [TestMethod]
        public void Index()
        {
            DataInfo data = new DataInfo();
            data.Name = "indexTest";
            data.URL = "ww";
            data.UserEmail = "wesamhihi3@gmail.com";
            db.DataInfoes.AddOrUpdate(data);

            var u = from s in db.Users
                    select s;
            u = u.Where(s => s.EmailID == "wesamhihi3@gmail.com");
            foreach (User user in u)
                data.UserId = user.UserID;

            db.SaveChanges();
            // Assert
            Assert.IsNotNull(data);

        }

        [TestMethod()]
        public void Data()
        {

            User user = (from u in db.Users
                         where u.EmailID == "wesamhihi3@gmail.com"
                         select u).FirstOrDefault();

            var data = from da in db.DataInfoes
                       where da.UserEmail == user.EmailID
                       select da;

            Assert.IsNotNull(data);

        }

        [TestMethod()]
        public void Delete()
        {
            var data = db.DataInfoes.Where(x => x.Id == 94).FirstOrDefault();
            db.DataInfoes.Remove(data);
            db.SaveChanges();

            //Console.WriteLine(data.Name);
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void Edit()
        {

            var data = (from item in db.DataInfoes
                        where item.Id == 90
                        select item).FirstOrDefault();

            db.DataInfoes.Remove(data);
            db.SaveChanges();

            data.Name = "testedit";
            db.DataInfoes.AddOrUpdate(data);

            db.SaveChanges();

            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void IndexAdmin()
        {
            DataInfo data = new DataInfo();
            data.Name = "indexTest";
            data.URL = "ww";
            db.DataInfoes.AddOrUpdate(data);
            data.UserEmail = "ghazihaddad3@gmail.com";

            db.SaveChanges();
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void DataAdmin()
        {
            var data = from da in db.DataInfoes
                       where da.UserEmail != null && (da.Accepted == false || da.Accepted == null)
                       select da;
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void EditAdmin()
        {
            var data = (from item in db.DataInfoes
                        where item.Id == 1085
                        select item).First();
            db.DataInfoes.Remove(data);
            db.SaveChanges();

            data.Name = "editAdmin";
            db.DataInfoes.AddOrUpdate(data);

            db.SaveChanges();
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void Accept()
        {

            DataInfo data = db.DataInfoes.Where(x => x.Id == 1088).FirstOrDefault();
            data.Accepted = true;
            db.DataInfoes.Add(data);

            DataInfo data1 = db.DataInfoes.Where(x => x.Id == 1088 && x.UserEmail != null).FirstOrDefault();
            db.DataInfoes.Remove(data1);
            db.SaveChanges();

            Assert.IsNotNull(data);
        }
    }
}